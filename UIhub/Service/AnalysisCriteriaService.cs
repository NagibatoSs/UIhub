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
    }
}
