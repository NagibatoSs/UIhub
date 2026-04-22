using Microsoft.CodeAnalysis.Diagnostics;

namespace UIhub.Analyze
{
    public abstract class UIAnalyzer
    {
        public abstract string Name { get; }

        public abstract AnalysisResult Analyze(List<UiElement> elements);

    }
}
