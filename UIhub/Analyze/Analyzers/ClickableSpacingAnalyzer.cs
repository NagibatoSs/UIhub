using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class ClickableSpacingAnalyzer : UIAnalyzer
    {
        private static readonly HashSet<string> ClickableClasses = new()
        {
            "button", "checkbox", "input", "menuItem"
        };

        public override string Code => "CLICK_SPACING";

        public ClickableSpacingAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService)
        {
        }

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var minSpacing = (int)(criteria?.ThresholdValue ?? 8);
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

            var clickable = GetClickableElements(elements);

            if (clickable.Count < 2)
            {
                result.Metric = BuildMetric(name,true, "нет данных", minSpacing);
                return result;
            }

            var minDistance = FindMinDistance(clickable, minSpacing, result);

            result.Metric = BuildMetric(name,
                minDistance == double.MaxValue || minDistance >= minSpacing,
                minDistance == double.MaxValue ? "нет данных" : $"{minDistance:F1}px",
                minSpacing
            );

            return result;
        }

        private List<UiElement> GetClickableElements(List<UiElement> elements)
        {
            return elements.Where(e => ClickableClasses.Contains(e.Class)).ToList();
        }

        private double FindMinDistance(List<UiElement> clickable, int minSpacing, AnalysisResult result)
        {
            var minDistance = double.MaxValue;

            for (int i = 0; i < clickable.Count; i++)
            {
                for (int j = i + 1; j < clickable.Count; j++)
                {
                    var first = clickable[i];
                    var second = clickable[j];
                    var distance = ComputeDistance(first.Bbox, second.Bbox);

                    if (distance <= 0)
                        continue;

                    if (distance < minDistance)
                        minDistance = distance;

                    if (distance < minSpacing)
                    {
                        result.Items.Add(new AnalysisItem
                        {
                            ElementIds = new List<int> { first.Id, second.Id },
                            Message = $"Элементы {ElementClassNames.Translate(first.Class)} и {ElementClassNames.Translate(second.Class)} " +
                                      $"расположены слишком близко: {distance:F1}px"
                        });
                    }
                }
            }

            return minDistance;
        }

        private double ComputeDistance(BBox a, BBox b)
        {
            int dx = Math.Max(0, Math.Max(a.X - (b.X + b.Width), b.X - (a.X + a.Width)));
            int dy = Math.Max(0, Math.Max(a.Y - (b.Y + b.Height), b.Y - (a.Y + a.Height)));
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private AnalysisMetric BuildMetric(string name, bool isOk, string value, int minSpacing)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = value,
                Threshold = $"≥ {minSpacing}px",
                IsOk = isOk
            };
        }
    }
}