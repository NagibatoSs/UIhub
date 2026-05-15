using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class ContrastAnalyzer : UIAnalyzer
    {
        private static readonly HashSet<string> ClickableClasses = new()
        {
            "button", "checkbox"
        };

        public override string Code => "CONTRAST_UI";

        public ContrastAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService)
        {
        }

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var threshold = criteria?.ThresholdValue ?? 3.0;
            var recommendation = criteria?.Recommendation ?? "";
            var name = criteria?.Name ?? Code;
            var standardReference = criteria?.StandardReference ?? "";

            var result = new AnalysisResult
            {
                AnalyzerName = name,
                Code = Code,
                Recomendation = recommendation,
                StandardReference = standardReference,
                Description = criteria?.Description ?? "",
            };

            var clickable = GetClickableElements(elements);

            if (!clickable.Any())
            {
                result.Metric = BuildMetric(name, true, "нет данных", threshold);
                return result;
            }

            double minContrast = double.MaxValue;

            foreach (var el in clickable)
            {
                if (el.Visual?.MeanColorRgb == null || el.Visual?.BackgroundColorRgb == null) continue;

                var contrast = ComputeContrastRatio(el.Visual.MeanColorRgb, el.Visual.BackgroundColorRgb);

                if (contrast < minContrast)
                    minContrast = contrast;

                if (contrast < threshold)
                {
                    result.Items.Add(new AnalysisItem
                    {
                        ElementIds = new List<int> { el.Id },
                        Message = $"Элемент {ElementClassNames.Translate(el.Class)} имеет контраст {contrast:F2} (норма ≥ {threshold})"
                    });
                }
            }

            var value = minContrast == double.MaxValue ? "нет данных" : $"{minContrast:F2}";
            result.Metric = BuildMetric(name, minContrast >= threshold, value, threshold);
            return result;
        }


        private List<UiElement> GetClickableElements(List<UiElement> elements)
        {
            return elements.Where(e => ClickableClasses.Contains(e.Class)).ToList();
        }

        private static double ComputeContrastRatio(List<double> rgb1, List<double> rgb2)
        {
            var l1 = RelativeLuminance(rgb1);
            var l2 = RelativeLuminance(rgb2);
            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(List<double> rgb)
        {
            double r = Linearize(rgb[0] / 255.0);
            double g = Linearize(rgb[1] / 255.0);
            double b = Linearize(rgb[2] / 255.0);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double Linearize(double c)
        {
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        private AnalysisMetric BuildMetric(string name, bool isOk, string value, double threshold)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = value,
                Threshold = $"≥ {threshold}",
                IsOk = isOk
            };
        }
    }
}