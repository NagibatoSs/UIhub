using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class ActiveMenuItemAnalyzer : UIAnalyzer
    {
        public ActiveMenuItemAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService) { }

        public override string Code => "ACTIVE_MENU";

        private double GetBrightness(List<double> rgb) => 0.299 * rgb[0] + 0.587 * rgb[1] + 0.114 * rgb[2];

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var threshold = criteria?.ThresholdValue ?? 10.0;
            var recommendation = criteria?.Recommendation ?? "";
            var name = criteria?.Name ?? Code;
            var standardReference = criteria?.StandardReference ?? "";

            var result = new AnalysisResult
            {
                AnalyzerName = name,
                Code = Code,
                Recomendation = recommendation,
                StandardReference = standardReference,
                Description = criteria?.Description ?? ""
            };

            var menuItems = GetMenuItems(elements);
            if (menuItems.Count < 2)
            {
                result.Metric = BuildMetric(name,true, "нет меню", threshold);
                return result;
            }

            var hasActiveItem = HasVisuallyDistinctItem(menuItems, threshold);

            if (!hasActiveItem)
            {
                result.Items.Add(new AnalysisItem
                {
                    ElementIds = menuItems.Select(e => e.Id).ToList(),
                    Message = "Ни один пункт меню визуально не выделен как активный"
                });
            }

            result.Metric = BuildMetric(name, hasActiveItem,
                hasActiveItem ? "активный пункт найден" : "активный пункт не найден", threshold);
            return result;
        }

        private List<UiElement> GetMenuItems(List<UiElement> elements)
        {
            return elements
                .Where(e => e.Class == "menuItem" && e.Visual != null)
                .ToList();
        }

        private bool HasVisuallyDistinctItem(List<UiElement> menuItems, double threshold)
        {
            var itemsWithOcr = menuItems
                .Where(e => e.Ocr?.TextColorRgb != null)
                .ToList();

            if (itemsWithOcr.Count >= 2)
            {
                var brightnessValues = itemsWithOcr
                    .Select(e => GetBrightness(e.Ocr.TextColorRgb))
                    .ToList();
                var avg = brightnessValues.Average();
                return brightnessValues.Any(b => Math.Abs(b - avg) >= threshold);
            }

            var colorValues = menuItems
                .Where(e => e.Visual?.MeanColorRgb != null)
                .Select(e => GetBrightness(e.Visual.MeanColorRgb))
                .ToList();

            if (!colorValues.Any()) return false;

            var avgColor = colorValues.Average();
            return colorValues.Any(b => Math.Abs(b - avgColor) >= threshold);
        }

        private AnalysisMetric BuildMetric(string name, bool isOk, string value, double threshold)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = value,
                Threshold = $"разница яркости ≥ {threshold}",
                IsOk = isOk
            };
        }
    }
}
