using UIhub.Models;

namespace UIhub.Data
{
    public interface IAssessmentUpload
    {
        Task Create(IFormFile csFile, IFormFile jsonFile);
    }
}
