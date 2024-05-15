using UIhub.Models;

namespace UIhub.Data
{
    public interface IEstimate
    {
        Task UpdateEstimate(Estimate estimateScale);
        IEnumerable<EstimateScale> GetAllEstimateScale();
        EstimateVoting? GetEstimateVoting(int id);
        EstimateRanging? GetEstimateRanging(int id);
        Estimate GetEstimate(int id);
        //Task UpdateEstimateScaleCount2(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount3(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount4(int estimateId, int newCount);
        //Task UpdateEstimateScaleCount5(int estimateId, int newCount);
    }
}
