using UIhub.Data;

namespace UIhub.Analyze.Analyzers
{
    public class AlignmentAnalyzer : UIAnalyzer
    {
        private const double SpacingVarianceFactor = 0.5;
        private const int MaxGroupSpacing = 100;

        public override string Code => "ALIGNMENT";

        private static readonly Dictionary<string, HashSet<string>> CompatibleClasses = new()
        {
            { "button",   new HashSet<string> { "button" } },
            { "menuItem", new HashSet<string> { "menuItem" } },
            { "checkbox", new HashSet<string> { "checkbox", "input" } },
            { "input",    new HashSet<string> { "input", "checkbox", "label" } },
            { "label",    new HashSet<string> { "label", "input", "checkbox" } },
            { "header",   new HashSet<string> { "header" } },
            { "text",     new HashSet<string> { "text" } },
        };

        public AlignmentAnalyzer(IAnalysisCriteria criteriaService) : base(criteriaService)
        {
        }

        public override AnalysisResult Analyze(List<UiElement> elements)
        {
            var criteria = _criteriaService.GetByCode(Code);
            var axisTolerance = (int)(criteria?.ThresholdValue ?? 10);
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

            if (elements == null || elements.Count < 2)
                return result;

            var groups = FindGroups(elements);

            foreach (var group in groups.Where(g => g.Count >= 2))
                CheckAlignment(group, result, axisTolerance);

            result.Metric = BuildMetric(name, result, axisTolerance);
            return result;
        }


        private List<List<UiElement>> FindGroups(List<UiElement> elements)
        {
            var groups = new List<List<UiElement>>();
            groups.AddRange(FindHorizontalGroups(elements));
            groups.AddRange(FindVerticalGroups(elements));
            return groups;
        }

        private List<List<UiElement>> FindHorizontalGroups(List<UiElement> elements)
        {
            var avgHeight = elements.Average(e => e.Bbox.Height);
            var rows = GroupByAxis(
                elements.OrderBy(e => e.Geometry.CenterY).ToList(),
                e => e.Geometry.CenterY,
                avgHeight / 2);

            return rows
                .Where(r => r.Count >= 2)
                .SelectMany(r => SplitBySpacing(
                    r.OrderBy(e => e.Geometry.Left).ToList(),
                    (a, b) => b.Geometry.Left - a.Geometry.Right))
                .ToList();
        }

        private List<List<UiElement>> FindVerticalGroups(List<UiElement> elements)
        {
            var avgWidth = elements.Average(e => e.Bbox.Width);
            var cols = GroupByAxis(elements.OrderBy(e => e.Geometry.CenterX).ToList(), e => e.Geometry.CenterX, avgWidth / 2);

            return cols
                .Where(c => c.Count >= 2)
                .SelectMany(c => SplitBySpacing(
                    c.OrderBy(e => e.Geometry.Top).ToList(),
                    (a, b) => b.Geometry.Top - a.Geometry.Bottom))
                .ToList();
        }

        private List<List<UiElement>> GroupByAxis(List<UiElement> elements, Func<UiElement, double> axisSelector, double tolerance)
        {
            var groups = new List<List<UiElement>>();

            foreach (var el in elements)
            {
                var existing = groups.FirstOrDefault(g =>
                    Math.Abs(axisSelector(g.First()) - axisSelector(el)) <= tolerance
                    && AreCompatible(g.First().Class, el.Class));

                if (existing != null)
                    existing.Add(el);
                else
                    groups.Add(new List<UiElement> { el });
            }

            return groups;
        }

        private bool AreCompatible(string class1, string class2)
        {
            if (CompatibleClasses.TryGetValue(class1, out var compatible))
                return compatible.Contains(class2);
            return class1 == class2;
        }

        private List<List<UiElement>> SplitBySpacing(
            List<UiElement> sorted,
            Func<UiElement, UiElement, int> spacingSelector)
        {
            var spacings = Enumerable.Range(0, sorted.Count - 1)
                .Select(i => spacingSelector(sorted[i], sorted[i + 1]))
                .ToList();

            var avgSpacing = spacings.Any() ? spacings.Average() : 0;
            var result = new List<List<UiElement>>();
            var current = new List<UiElement> { sorted[0] };

            for (int i = 1; i < sorted.Count; i++)
            {
                var spacing = spacings[i - 1];

                if (spacing > MaxGroupSpacing || spacing > avgSpacing * (1 + SpacingVarianceFactor))
                {
                    if (current.Count >= 2) result.Add(current);
                    current = new List<UiElement>();
                }
                current.Add(sorted[i]);
            }

            if (current.Count >= 2) result.Add(current);
            return result;
        }

        private void CheckAlignment(List<UiElement> group, AnalysisResult result, int axisTolerance)
        {
            if (IsHorizontalGroup(group))
                CheckAxis(group, result, e => e.Geometry.CenterY, "горизонтали", axisTolerance);
            else
                CheckAxis(group, result, e => e.Geometry.CenterX, "вертикали", axisTolerance);
        }

        private bool IsHorizontalGroup(List<UiElement> group)
        {
            var ySpread = group.Max(e => e.Geometry.CenterY) - group.Min(e => e.Geometry.CenterY);
            return ySpread < group.Average(e => e.Bbox.Height);
        }

        private void CheckAxis(List<UiElement> group, AnalysisResult result, Func<UiElement, double> axisSelector, string axisName, int axisTolerance)
        {
            var avg = group.Average(axisSelector);
            var groupLabel = string.Join(", ", group.Select(e => $"#{e.Id}({e.Class})"));

            foreach (var el in group)
            {
                var deviation = Math.Abs(axisSelector(el) - avg);
                if (deviation > axisTolerance)
                {
                    result.Items.Add(new AnalysisItem
                    {
                        ElementIds = group.Select(e => e.Id).ToList(),
                        GroupLabel = groupLabel,
                        Message = $"{ElementClassNames.Translate(el.Class)} не выровнен по {axisName} " +
                                  $"(отклонение {deviation:F0}px)"
                    });
                }
            }
        }

        private AnalysisMetric BuildMetric(string name, AnalysisResult result, int axisTolerance)
        {
            return new AnalysisMetric
            {
                Name = name,
                Value = result.Items.Any() ? $"{result.Items.Count} нарушений" : "нарушений нет",
                Threshold = $"отклонение ≤ {axisTolerance}px",
                IsOk = !result.Items.Any()
            };
        }
    }
}