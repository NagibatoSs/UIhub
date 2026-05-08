using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class OverlapAnalyzer : UIAnalyzer
    {
        public override string Code => "OVERLAP";

        public OverlapAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService) { }

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
                StandardReference = standardReference
            };

            result.Items = FindIssues(elements);

            result.Metric = BuildMetric(name,result);

            return result;
        }


        private List<AnalysisItem> FindIssues(List<UiElement> elements)
        {
            var issues = new List<AnalysisItem>();

            for (int i = 0; i < elements.Count; i++)
            {
                for (int j = i + 1; j < elements.Count; j++)
                {
                    var first = elements[i];
                    var second = elements[j];

                    if (AreOverlapping(first.Bbox, second.Bbox))
                    {
                        issues.Add(new AnalysisItem
                        {
                            ElementIds = new List<int> { first.Id, second.Id },
                            Message = $"Элементы {ElementClassNames.Translate(first.Class)} и {ElementClassNames.Translate(second.Class)} пересекаются."
                        });
                    }
                }
            }

            return issues;
        }

        private bool AreOverlapping(BBox a, BBox b)
        {
            int aRight = a.X + a.Width;
            int aBottom = a.Y + a.Height;
            int bRight = b.X + b.Width;
            int bBottom = b.Y + b.Height;

            bool noOverlap =
                aRight <= b.X ||
                bRight <= a.X ||
                aBottom <= b.Y ||
                bBottom <= a.Y;

            return !noOverlap;
        }

        private AnalysisMetric BuildMetric(string name, AnalysisResult result)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = result.Items.Any() ? $"{result.Items.Count}" : "пересечений нет",
                Threshold = "",
                IsOk = !result.Items.Any()
            };
        }
    }
}