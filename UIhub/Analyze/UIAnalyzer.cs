using Microsoft.CodeAnalysis.Diagnostics;
using UIhub.Data;

namespace UIhub.Analyze
{
    public abstract class UIAnalyzer
    {
        protected readonly IAnalysisCriteria _criteriaService;

        protected UIAnalyzer(IAnalysisCriteria criteriaService)
        {
            _criteriaService = criteriaService;
        }
        public abstract string Code { get; }

        public abstract AnalysisResult Analyze(List<UiElement> elements);

    }
}
