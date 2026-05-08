using UIhub.Models;

namespace UIhub.Data
{
    public interface IAnalysisCriteria
    {
        IEnumerable<AnalysisCriteria> GetAll();
        AnalysisCriteria GetById(int id);
        AnalysisCriteria GetByCode(string code);
    }
}
