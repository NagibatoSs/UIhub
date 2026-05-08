using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class SmallClickableElementAnalyzer : UIAnalyzer
    {
        private static readonly HashSet<string> ClickableClasses = new()
        {
            "button", "checkbox", "input", "menuItem"
        };
        public override string Code => "MIN_CLICK_SIZE";
        public SmallClickableElementAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService) { }
        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var minSize = (int)(criteria?.ThresholdValue ?? 44);
            var recommendation = criteria?.Recommendation ?? "";
            var name = criteria?.Name ?? Code;
            var standardReference = criteria?.StandardReference ?? "";

            var result = new AnalysisResult
            {
                AnalyzerName = name,
                Code = Code,
                Recomendation = recommendation,
                StandardReference = standardReference,
            };

            var clickable = GetClickableElements(elements);

            if (!clickable.Any())
            {
                result.Metric = BuildMetric(name, true, "нет данных", minSize);
                return result;
            }

            var minElement = FindSmallestElement(clickable);
            var isOk = minElement.Bbox.Width >= minSize && minElement.Bbox.Height >= minSize;

            result.Metric = BuildMetric(name, isOk, $"{minElement.Bbox.Width}×{minElement.Bbox.Height}px", minSize);
            result.Items = FindIssue(clickable, minSize);

            return result;
        }


        private List<UiElement> GetClickableElements(List<UiElement> elements)
        {
            return elements.Where(e => ClickableClasses.Contains(e.Class)).ToList();
        }

        private UiElement FindSmallestElement(List<UiElement> elements)
        {
            return elements.OrderBy(e => e.Bbox.Width * e.Bbox.Height).First();
        }

        private List<AnalysisItem> FindIssue(List<UiElement> elements, int minSize) =>
            elements
                .Where(e => e.Bbox.Width < minSize || e.Bbox.Height < minSize)
                .Select(e => new AnalysisItem
                {
                    ElementIds = new List<int> { e.Id },
                    Message = $"{ElementClassNames.Translate(e.Class)} имеет размер {e.Bbox.Width}×{e.Bbox.Height}px"
                })
                .ToList();

        private AnalysisMetric BuildMetric(string name, bool isOk, string value, int minSize)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = value,
                Threshold = $"≥ {minSize}×{minSize}px",
                IsOk = isOk
            };
        }      
    }
}
