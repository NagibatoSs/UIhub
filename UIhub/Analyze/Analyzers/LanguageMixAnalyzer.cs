using System.Text.RegularExpressions;
using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class LanguageMixAnalyzer : UIAnalyzer
    {
        private static readonly Regex CyrillicPattern = new Regex(@"[а-яёА-ЯЁ]+", RegexOptions.Compiled);
        private static readonly Regex LatinPattern = new Regex(@"[a-zA-Z]+", RegexOptions.Compiled);

        private static readonly HashSet<string> TextClasses = new()
        {
            "header", "button", "menuItem", "label"
        };
        public override string Code => "LANGUAGE_MIX";

        public LanguageMixAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService)
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

            var textElements = GetTextElements(elements);

            if (!textElements.Any())
            {
                result.Metric = BuildMetric(name,true, "нет данных");
                return result;
            }

            var dominantLanguage = DetectDominantLanguage(textElements);

            foreach (var el in textElements)
            {
                var mixedWords = FindMixedWords(el.Ocr.Text, dominantLanguage);

                if (mixedWords.Any())
                {
                    result.Items.Add(new AnalysisItem
                    {
                        ElementIds = new List<int> { el.Id },
                        Message = $"Элемент {ElementClassNames.Translate(el.Class)} содержит слова на другом языке: " +
                                  $"{string.Join(", ", mixedWords.Select(w => $"«{w}»"))}"
                    });
                }
            }

            result.Metric = BuildMetric(
                name,
                !result.Items.Any(),
                result.Items.Any() ? $"{result.Items.Count} нарушений" : "нарушений нет"
            );

            return result;
        }


        private List<UiElement> GetTextElements(List<UiElement> elements)
        {
            return elements
                .Where(e => TextClasses.Contains(e.Class)
                    && !string.IsNullOrWhiteSpace(e.Ocr?.Text))
                .ToList();
        }

        private string DetectDominantLanguage(List<UiElement> elements)
        {
            int cyrillicCount = 0;
            int latinCount = 0;

            foreach (var el in elements)
            {
                cyrillicCount += CyrillicPattern.Matches(el.Ocr.Text).Count;
                latinCount += LatinPattern.Matches(el.Ocr.Text).Count;
            }

            return cyrillicCount >= latinCount ? "ru" : "en";
        }

        private List<string> FindMixedWords(string text, string dominantLanguage)
        {
            var mixed = new List<string>();
            var pattern = dominantLanguage == "ru" ? LatinPattern : CyrillicPattern;

            foreach (Match m in pattern.Matches(text))
            {
                if (m.Value.Length >= 2)
                    mixed.Add(m.Value);
            }

            return mixed;
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