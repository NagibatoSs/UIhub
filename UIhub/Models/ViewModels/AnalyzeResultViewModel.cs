using UIhub.Analyze;

namespace UIhub.Models.ViewModels
{
    public class AnalyzeResultViewModel
    {
        public List<AnalyzedItem> Items { get; set; }
        public List<ImageAnalysisResult> AnalysisResults { get; set; } = new();
    }

    public class ImageAnalysisResult
    {
        public string FileName { get; set; } = string.Empty;

        public List<AnalysisResult> Results { get; set; } = new();
    }
}
