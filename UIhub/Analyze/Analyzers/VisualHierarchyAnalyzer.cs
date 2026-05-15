using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class VisualHierarchyAnalyzer : UIAnalyzer
    {
        private static readonly HashSet<string> SeparatorClasses = new()
        {
            "text", "input", "checkbox", "button"
        };

        public override string Code => "HIERARCHY";

        public VisualHierarchyAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService)
        {
        }

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
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

            var headers = GetHeadersWithFontSize(elements);

            if (headers.Count < 2)
            {
                result.Metric = BuildMetric(name,true, "нарушений нет");
                return result;
            }

            CheckHierarchy(elements, result);

            result.Metric = BuildMetric(name,
                !result.Items.Any(),
                result.Items.Any() ? $"{result.Items.Count} нарушений" : "нарушений нет"
            );

            return result;
        }


        private List<UiElement> GetHeadersWithFontSize(List<UiElement> elements)
        {
            return elements
                .Where(e => e.Class == "header" && e.Ocr?.FontSize > 0)
                .OrderBy(e => e.Geometry.Top)
                .ToList();
        }

        private void CheckHierarchy(List<UiElement> allElements, AnalysisResult result)
        {
            var sorted = allElements
                .Where(e => (e.Class == "header" && e.Ocr?.FontSize > 0)
                            || SeparatorClasses.Contains(e.Class))
                .OrderBy(e => e.Geometry.Top)
                .ToList();

            UiElement prevHeader = null;
            bool hasSeparatorBetween = false;

            foreach (var el in sorted)
            {
                if (SeparatorClasses.Contains(el.Class))
                {
                    hasSeparatorBetween = true;
                    continue;
                }

                if (prevHeader != null)
                    CheckHeaderPair(prevHeader, el, hasSeparatorBetween, result);

                prevHeader = el;
                hasSeparatorBetween = false;
            }
        }

        private void CheckHeaderPair(UiElement prev, UiElement current, bool hasSeparator, AnalysisResult result)
        {
            if (current.Ocr.FontSize > prev.Ocr.FontSize && !hasSeparator)
            {
                var shortText = current.Ocr.Text?.Length > 15  
                    ? current.Ocr.Text.Substring(0, 15) + "..."
                    : current.Ocr.Text;
                var shortPrevText = prev.Ocr.Text?.Length > 15
                    ? prev.Ocr.Text.Substring(0, 15) + "..."
                    : prev.Ocr.Text;
                result.Items.Add(new AnalysisItem
                {
                    ElementIds = new List<int> { prev.Id, current.Id },
                    Message = $"Заголовок \"{shortText}\" крупнее " +
                              $"предыдущего \"{shortPrevText}\" ({prev.Ocr.FontSize}px) и между ними нет контента"
                });
            }
            else if (current.Ocr.FontSize == prev.Ocr.FontSize && !hasSeparator)
            {
                var shortText = current.Ocr.Text?.Length > 15
                    ? current.Ocr.Text.Substring(0, 15) + "..."
                    : current.Ocr.Text;
                var shortPrevText = prev.Ocr.Text?.Length > 15
                    ? prev.Ocr.Text.Substring(0, 15) + "..."
                    : prev.Ocr.Text;
                result.Items.Add(new AnalysisItem
                {
                    ElementIds = new List<int> { prev.Id, current.Id },
                    Message = $"Заголовок \"{shortText}\" имеет тот же размер " +
                              $"что и предыдущий - \"{shortPrevText}\" ({prev.Ocr.FontSize}px) " +
                              $"и между ними нет контента"
                });
            }
        }

        private AnalysisMetric BuildMetric(string name, bool isOk, string value)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = value,
                Threshold = "",
                IsOk = isOk
            };
        }
    }
}