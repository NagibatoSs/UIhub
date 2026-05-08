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
            };

            var headers = GetHeadersWithFontSize(elements);

            if (headers.Count < 2)
            {
                result.Metric = BuildMetric(name,true, "нет данных");
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
            if (current.Ocr.FontSize > prev.Ocr.FontSize)
            {
                result.Items.Add(new AnalysisItem
                {
                    ElementIds = new List<int> { prev.Id, current.Id },
                    Message = $"Заголовок крупнее " +
                              $"предыдущего ({prev.Ocr.FontSize}px)"
                });
            }
            else if (current.Ocr.FontSize == prev.Ocr.FontSize && !hasSeparator)
            {
                result.Items.Add(new AnalysisItem
                {
                    ElementIds = new List<int> { prev.Id, current.Id },
                    Message = $"Заголовок имеет тот же размер " +
                              $"что и предыдущий - {prev.Ocr.FontSize}px) " +
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