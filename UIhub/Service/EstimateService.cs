using Microsoft.EntityFrameworkCore;
using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class EstimateService: IEstimate
    {
        private readonly ApplicationDbContext _context;
        public EstimateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateEstimate(Estimate estimate)
        {
            _context.Update(estimate);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<EstimateScale> GetAllEstimateScale()
        {
            return _context.EstimateScales;
        }
        public EstimateScale? GetEstimateScale(int id)
        {
            return _context.EstimateScales
                .Where(e => e.Id == id)
                .FirstOrDefault();
        }
        public Estimate GetEstimate(int id)
        {
            return _context.Estimates
                .Where(e => e.Id == id)
                .FirstOrDefault();
        }
        public EstimateVoting? GetEstimateVoting(int id)
        {
            return _context.EstimateVotings
                .Where(e => e.Id == id)
                .Include (e => e.VotingObjects)
                .FirstOrDefault();
        }
        public EstimateRanging? GetEstimateRanging(int id)
        {
            return _context.EstimateRangings
                .Where(e => e.Id == id)
                .Include(e=> e.RangingObjects)
                .Include(e => e.Sequences)
                .FirstOrDefault();
        }
        //public async Task Create(EstimateScale postReply)
        //{
        //    _context.Add(postReply);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateEstimateScaleCount1(int estimateId, int newCount)
        //{
        //    var entity = _context.EstimateScales.Where(e => e.Id == estimateId).FirstOrDefault();
        //    await _context.Uo
        //}

        //public Task UpdateEstimateScaleCount2(int estimateId, int newCount)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateEstimateScaleCount3(int estimateId, int newCount)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateEstimateScaleCount4(int estimateId, int newCount)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateEstimateScaleCount5(int estimateId, int newCount)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
