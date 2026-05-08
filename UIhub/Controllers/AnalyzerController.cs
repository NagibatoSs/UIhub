using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using UIhub.Analyze;
using UIhub.Analyze.Analyzers;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;

namespace UIhub.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAnalysis _analysisService;
        private readonly IAnalysisCriteria _criteriaService;
        private readonly UserManager<User> _userManager;
        private readonly ImageDrawingService _imageDrawingService;

        public AnalyzerController(IHttpClientFactory httpClientFactory,IAnalysis analysisService,
            IAnalysisCriteria criteriaService,UserManager<User> userManager, ImageDrawingService imageDrawingService)
        {
            _httpClientFactory = httpClientFactory;
            _analysisService = analysisService;
            _criteriaService = criteriaService;
            _userManager = userManager;
            _imageDrawingService = imageDrawingService;
        }
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
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
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
            new OverlapAnalyzer(_criteriaService),
            new SmallClickableElementAnalyzer(_criteriaService),
            new ClickableSpacingAnalyzer(_criteriaService),
            new ContrastAnalyzer(_criteriaService),
            new TextContrastAnalyzer(_criteriaService),
            new AlignmentAnalyzer(_criteriaService),
            new ActiveMenuItemAnalyzer(_criteriaService),
            new VisualHierarchyAnalyzer(_criteriaService),
            new AbbreviationAnalyzer(_criteriaService),
            new LanguageMixAnalyzer(_criteriaService),
        };

                var analysisResults = new List<ImageAnalysisResult>();

                foreach (var item in result.Items)
                {
                    var perImageResults = analyzers
                        .Select(a => a.Analyze(item.Elements))
                        .ToList();

                    analysisResults.Add(new ImageAnalysisResult
                    {
                        FileName = item.FileName,
                        Results = perImageResults
                    });
                }

                result.AnalysisResults = analysisResults;

                // Сохраняем в БД
                await SaveAnalysisAsync(result, model.Files);

                return View("Result", result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Исключение: {ex.Message}");
                return View("Index");
            }
        }

        private async Task SaveAnalysisAsync(AnalyzeResultViewModel result, List<IFormFile> files)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return;

            var analysis = _analysisService.CreateAnalysis(userId);

            foreach (var imageResult in result.AnalysisResults)
            {
                var originalItem = result.Items.FirstOrDefault(i => i.FileName == imageResult.FileName);

                // Путь к оригинальному файлу
                var originalPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", analysis.Id.ToString(), "originals", imageResult.FileName);

                // Копируем оригинал в папку анализа
                Directory.CreateDirectory(Path.GetDirectoryName(originalPath));
                var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageResult.FileName);
                System.IO.File.Copy(sourcePath, originalPath, overwrite: true);

                var filePath = $"/uploads/{analysis.Id}/originals/{imageResult.FileName}";

                var analysisFile = _analysisService.AddFile(
                    analysis.Id,
                    imageResult.FileName,
                    originalItem?.Image.Width ?? 0,
                    originalItem?.Image.Height ?? 0,
                    filePath
                );

                foreach (var analyzerResult in imageResult.Results)
                {
                    var criteria = _criteriaService.GetByCode(analyzerResult.Code);
                    if (criteria == null) continue;

                    // Рисуем рамки если есть нарушения
                    string resultImagePath = null;
                    if (analyzerResult.Items.Any())
                    {
                        var boxes = analyzerResult.Items
                            .SelectMany(i => i.ElementIds)
                            .Distinct()
                            .Select(id => originalItem?.Elements.FirstOrDefault(e => e.Id == id))
                            .Where(el => el != null)
                            .Select(el => (el.Bbox.X, el.Bbox.Y, el.Bbox.Width, el.Bbox.Height))
                            .ToList();

                        if (boxes.Any())
                        {
                            var resultFileName = $"{Path.GetFileNameWithoutExtension(imageResult.FileName)}_{analyzerResult.Code}.png";
                            var resultFullPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot", "uploads", analysis.Id.ToString(), "results", resultFileName);

                            _imageDrawingService.DrawViolations(originalPath, boxes, resultFullPath);
                            resultImagePath = $"/uploads/{analysis.Id}/results/{resultFileName}";
                        }
                    }

                    var criteriaResult = _analysisService.AddCriteriaResult(
                        analysisFile.Id,
                        criteria.Id,
                        analyzerResult.Metric?.Value ?? "",
                        analyzerResult.Metric?.IsOk ?? true,
                        resultImagePath
                    );

                    foreach (var issue in analyzerResult.Items)
                    {
                        _analysisService.AddIssue(criteriaResult.Id, issue.Message);
                    }
                }
            }
        }

    }
}
