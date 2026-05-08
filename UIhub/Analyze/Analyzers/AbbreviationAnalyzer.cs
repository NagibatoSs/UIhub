using System.Text.RegularExpressions;
using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class AbbreviationAnalyzer : UIAnalyzer
    {
        public override string Code => "ABBREVIATION";

        // Аббревиатуры - 2+ заглавных букв подряд
        private static readonly Regex AbbreviationPattern =
            new Regex(@"\b[A-ZА-Я]{2,}\b", RegexOptions.Compiled);

        // Сокращения — слово с точкой внутри или в конце (кроме конца предложения)
        private static readonly Regex AbbrevShortPattern =
            new Regex(@"\b\w+\.\w*\b", RegexOptions.Compiled);

        private static readonly HashSet<string> TextClasses = new()
        {
            "header", "button", "menuItem", "label"
        };

        public AbbreviationAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService) { }

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var recommendation = criteria?.Recommendation ?? "";
            var standardReference = criteria?.StandardReference ?? "";
            var name = criteria?.Name ?? Code;

            var result = new AnalysisResult
            {
                AnalyzerName = name,
                Code = Code,
                Recomendation = recommendation,
                StandardReference = standardReference
            };

            var textElements = elements
                .Where(e => TextClasses.Contains(e.Class)
                    && !string.IsNullOrWhiteSpace(e.Ocr?.Text))
                .ToList();

            if (!textElements.Any())
            {
                result.Metric = BuildMetric(name, true, "нет данных");
                return result;
            }

            foreach (var el in textElements)
            {
                var found = FindAbbreviations(el.Ocr.Text);
                foreach (var abbr in found)
                {
                    result.Items.Add(new AnalysisItem
                    {
                        ElementIds = new List<int> { el.Id },
                        Message = $"{ElementClassNames.Translate(el.Class)} содержит сокращение или аббревиатуру: «{abbr}»"
                    });
                }
            }

            result.Metric = BuildMetric(name,!result.Items.Any(), result.Items.Any() ? $"{result.Items.Count} нарушений" : "нарушений нет");

            return result;
        }

        private List<string> FindAbbreviations(string text)
        {
            var found = new HashSet<string>();

            foreach (Match m in AbbreviationPattern.Matches(text))
                found.Add(m.Value);

            foreach (Match m in AbbrevShortPattern.Matches(text))
                found.Add(m.Value);

            return found.ToList();
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
