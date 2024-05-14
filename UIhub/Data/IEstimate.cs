using UIhub.Models;

namespace UIhub.Data
{
    public interface IEstimate
    {
        Task UpdateEstimateScale(EstimateScale estimateScale);
        IEnumerable<EstimateScale> GetAllEstimateScale();
        EstimateScale GetEstimateScale(int id);
        //Task UpdateEstimateScaleCount2(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount3(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount4(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount5(int estimateId, int newCount);
    }
}
