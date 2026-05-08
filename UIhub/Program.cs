using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UIhub.Analyze;
using UIhub.Data;
using UIhub.Models;
using UIhub.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI().
    AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IPost, PostService>();
builder.Services.AddScoped<IPostReply, PostReplyService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IUserRank, UserRankService>();
builder.Services.AddScoped<IEstimate, EstimateService>();
builder.Services.AddScoped<IAnalysisCriteria, AnalysisCriteriaService>();
builder.Services.AddScoped<IAnalysis, AnalysisService>();
builder.Services.AddScoped<ImageDrawingService>();

builder.Services.AddHttpClient("PythonApi", client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.AnalysisCriterias.Any())
    {
        context.AnalysisCriterias.AddRange(
            new AnalysisCriteria { Code = "MIN_CLICK_SIZE", Name = "Минимальный размер кликабельных элементов", Recommendation = "Увеличьте размер кликабельных элементов до 44×44px", ThresholdValue = 44, StandardReference = "Apple HIG, WCAG 2.5.5" },
            new AnalysisCriteria { Code = "CLICK_SPACING", Name = "Расстояние между кликабельными элементами", Recommendation = "Увеличьте расстояние между активными элементами до 8px", ThresholdValue = 8, StandardReference = "Apple HIG, Google Material Design" },
            new AnalysisCriteria { Code = "OVERLAP", Name = "Перекрытие элементов", Recommendation = "Исправьте позиционирование перекрывающихся элементов", ThresholdValue = null, StandardReference = "Эвристика Нильсена 4, 5" },
            new AnalysisCriteria { Code = "CONTRAST_UI", Name = "Контрастность UI-элементов", Recommendation = "Повысьте контраст UI-элементов до 3.0", ThresholdValue = 3.0, StandardReference = "WCAG 1.4.11" },
            new AnalysisCriteria { Code = "CONTRAST_TEXT", Name = "Контрастность текста", Recommendation = "Повысьте контраст текста до 4.5", ThresholdValue = 4.5, StandardReference = "WCAG 1.4.3" },
            new AnalysisCriteria { Code = "ALIGNMENT", Name = "Неправильное выравнивание элементов группы", Recommendation = "Выровняйте элементы группы по единой оси", ThresholdValue = 10, StandardReference = "Гештальт-принцип выравнивания, Эвристика Нильсена 4" },
            new AnalysisCriteria { Code = "ACTIVE_MENU", Name = "Отсутствие указания текущего раздела в навигации", Recommendation = "Выделите активный пункт меню цветом или подчёркиванием", ThresholdValue = 10, StandardReference = "Эвристика Нильсена 1, WCAG 2.1" },
            new AnalysisCriteria { Code = "HIERARCHY", Name = "Нарушение иерархии заголовков", Recommendation = "Соблюдайте иерархию — каждый следующий заголовок должен быть меньше или равен предыдущему", ThresholdValue = null, StandardReference = "WCAG 1.3.1, Google UX Writing Guidelines" },
            new AnalysisCriteria { Code = "ABBREVIATION", Name = "Аббревиатуры и сокращения", Recommendation = "Избегайте аббревиатур в интерфейсных элементах или расшифровывайте их", ThresholdValue = null, StandardReference = "WCAG 3.1.4, Mailchimp Content Style Guide" },
            new AnalysisCriteria { Code = "LANGUAGE_MIX", Name = "Смешение языков в интерфейсе", Recommendation = "Используйте один язык. Если слово является брендом или техническим термином — допустимо", ThresholdValue = null, StandardReference = "Эвристика Нильсена 2, ISO 9241-110" }
        );
        context.SaveChanges();
    }
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ОШИБКА: {ex}");
        throw;
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHttpsRedirection();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Post}/{action=MainPage}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Post}/{action=MainPage}/{id?}");
});
app.MapRazorPages();

app.Run();
