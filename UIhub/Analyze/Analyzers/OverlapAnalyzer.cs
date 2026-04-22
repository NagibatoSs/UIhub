using Microsoft.CodeAnalysis.Diagnostics;

namespace UIhub.Analyze.Analyzers
{
    public class OverlapAnalyzer : UIAnalyzer
    {
        public override string Name => "OverlapAnalyzer";

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var result = new AnalysisResult
            {
                AnalyzerName = Name
            };

            for (int i = 0; i < elements.Count; i++)
            {
                for (int j = i + 1; j < elements.Count; j++)
                {
                    var first = elements[i];
                    var second = elements[j];

                    if (AreOverlapping(first.Bbox, second.Bbox))
                    {
                        result.Items.Add(new AnalysisItem
                        {
                            ElementIds = new List<int> { first.Id, second.Id },
                            Message = $"Элементы {first.Id} ({first.Class}) и {second.Id} ({second.Class}) пересекаются."
                        });
                    }
                }
            }

            return result;
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
    }
}
