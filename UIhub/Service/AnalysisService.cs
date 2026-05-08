using Microsoft.EntityFrameworkCore;
using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class AnalysisService : IAnalysis
    {
        private readonly ApplicationDbContext _context;

        public AnalysisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Analysis CreateAnalysis(string userId)
        {
            var analysis = new Analysis
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Analysis.Add(analysis);
            _context.SaveChanges();
            return analysis;
        }

        public AnalysisFile AddFile(int analysisId, string fileName, int width, int height, string filePath)
        {
            var file = new AnalysisFile
            {
                AnalysisId = analysisId,
                FileName = fileName,
                ImageWidth = width,
                ImageHeight = height,
                FilePath = filePath
            };
            _context.AnalysisFiles.Add(file);
            _context.SaveChanges();
            return file;
        }

        public CriteriaResult AddCriteriaResult(int fileId, int criteriaId, string metricValue, bool isOk, string resultImagePath)
        {
            var result = new CriteriaResult
            {
                AnalysisFileId = fileId,
                AnalysisCriteriaId = criteriaId,
                MetricValue = metricValue,
                IsOk = isOk,
                ResultImagePath = resultImagePath
            };
            _context.CriteriaResults.Add(result);
            _context.SaveChanges();
            return result;
        }

        public void AddIssue(int criteriaResultId, string message)
        {
            _context.CriteriaIssues.Add(new CriteriaIssue
            {
                CriteriaResultId = criteriaResultId,
                Message = message
            });
            _context.SaveChanges();
        }

        public IEnumerable<Analysis> GetUserAnalyses(string userId) =>
            _context.Analysis
                .Include(a => a.Files)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

        public Analysis GetAnalysisById(int id) =>
            _context.Analysis
                .Include(a => a.Files)
                .ThenInclude(f => f.Results)
                .ThenInclude(r => r.Issues)
                .Include(a => a.Files)
                .ThenInclude(f => f.Results)
                .ThenInclude(r => r.AnalysisCriteria)
                .FirstOrDefault(a => a.Id == id);
    }
}
