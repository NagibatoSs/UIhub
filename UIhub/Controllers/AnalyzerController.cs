using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using UIhub.Analyze;
using UIhub.Analyze.Analyzers;
using UIhub.Models;
using UIhub.Models.ViewModels;

namespace UIhub.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AnalyzerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Analyzer/Index — страница с формой загрузки
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(AnalyzeInputViewModel model)
        {
            if (model.Files == null || !model.Files.Any())
            {
                ModelState.AddModelError("", "Выберите хотя бы один файл");
                return View("Index");
            }
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in model.Files)
            {
                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            try
            {
                var client = _httpClientFactory.CreateClient("PythonApi");
                using var form = new MultipartFormDataContent();

                foreach (var file in model.Files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(fileContent, "files", file.FileName);
                }

                var response = await client.PostAsync("/analyze", form);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Ошибка Python API: {error}");
                    return View("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AnalyzeResultViewModel>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var analyzers = new List<UIAnalyzer>
                {
                    new OverlapAnalyzer(),
                    //new SmallClickableElementAnalyzer(),
                    //new ClickableSpacingAnalyzer()
                };
                var analysisResults = new List<object>();

                foreach (var item in result.Items)
                {
                    var elements = item.Elements ?? new List<UiElement>();

                    var perImageResults = analyzers
                        .Select(a => a.Analyze(elements))
                        .ToList();

                    analysisResults.Add(new
                    {
                        FileName = item.FileName,
                        Results = perImageResults
                    });
                }
                ViewBag.AnalysisResults = analysisResults;
                return View("Result", result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Исключение: {ex.Message}");
                return View("Index");
            }
        }

    }
}
