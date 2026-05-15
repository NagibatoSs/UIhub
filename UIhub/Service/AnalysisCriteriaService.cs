using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class AnalysisCriteriaService : IAnalysisCriteria
    {
        private readonly ApplicationDbContext _context;

        public AnalysisCriteriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AnalysisCriteria> GetAll() => _context.AnalysisCriterias;

        public AnalysisCriteria GetById(int id) =>
            _context.AnalysisCriterias.FirstOrDefault(c => c.Id == id);

        public AnalysisCriteria GetByCode(string code) =>
            _context.AnalysisCriterias.FirstOrDefault(c => c.Code == code);

        public void Update(AnalysisCriteria criteria)
        {
            var existing = _context.AnalysisCriterias.FirstOrDefault(c => c.Id == criteria.Id);
            if (existing == null) return;

            existing.Name = criteria.Name;
            existing.Description = criteria.Description;
            existing.Recommendation = criteria.Recommendation;
            existing.ThresholdValue = criteria.ThresholdValue;
            existing.StandardReference = criteria.StandardReference;

            _context.SaveChanges();
        }
    }
}
