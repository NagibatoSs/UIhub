using UIhub.Models;

namespace UIhub.Data
{
    public interface IAnalysis
    {
        Analysis CreateAnalysis(string userId);
        AnalysisFile AddFile(int analysisId, string fileName, int width, int height, string filePath);
        CriteriaResult AddCriteriaResult(int fileId, int criteriaId, string metricValue, bool isOk, string resultImagePath);
        void AddIssue(int criteriaResultId, string message);
        IEnumerable<Analysis> GetUserAnalyses(string userId);
        Analysis GetAnalysisById(int id);
    }
}
