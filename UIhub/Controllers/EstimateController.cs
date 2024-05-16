using Microsoft.AspNetCore.Mvc;
using System.Text;
using UIhub.Models.ViewModels;
using UIhub.Models;
using UIhub.Service;
using UIhub.Data;

namespace UIhub.Controllers
{
    public class EstimateController : Controller
    {
        private readonly IEstimate _estimateService;
        public EstimateController(IEstimate estimateService)
        {
            _estimateService = estimateService;
        }
        [HttpPost]
        public async Task<IActionResult> SetScaleEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewScaleEstimateViewModel.Count; i++)
            {
                var modelScale = model.NewScaleEstimateViewModel[i];
                var estimateScale = (_estimateService.GetEstimate(model.PostViewModel.EstimatesScale[i].Id)) as EstimateScale;
                if (modelScale.SelectedValue == "1")
                    estimateScale.Count_1++;
                if (modelScale.SelectedValue == "2")
                    estimateScale.Count_2++;
                if (modelScale.SelectedValue == "3")
                    estimateScale.Count_3++;
                if (modelScale.SelectedValue == "4")
                    estimateScale.Count_4++;
                if (modelScale.SelectedValue == "5")
                    estimateScale.Count_5++;
                _estimateService.UpdateEstimate(estimateScale).Wait();
            }
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SetVotingEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewVotingEstimateViewModel.Count; i++)
            {
                var modelVoting = model.NewVotingEstimateViewModel[i];
                var estimateVote = _estimateService.GetEstimateVoting(model.PostViewModel.EstimatesVoting[i].Id);
                estimateVote.VotingObjects[int.Parse(modelVoting.SelectedValue)].VoteCount++;
                _estimateService.UpdateEstimate(estimateVote).Wait();
            }
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SetRangingEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewRangingEstimateViewModel.Count; i++)
            {
                var modelRanging = model.NewRangingEstimateViewModel[i];
                var estimateRange = _estimateService.GetEstimateRanging(model.PostViewModel.EstimatesRanging[i].Id);
                StringBuilder order = new StringBuilder();
                for (int j = 0; j < estimateRange.RangingObjects.Count; j++)
                {
                    var b = model.PostViewModel.EstimatesRanging[i].RangingObjects[j].NumberInSequence;
                    var numberByIndex = int.Parse(b) - 1;
                    estimateRange.RangingObjects[j].NumberInSequence = numberByIndex.ToString();
                    order.Append(estimateRange.RangingObjects[j].NumberInSequence);
                }
                var sequence = new RangingSequence() { NumbersOrder = order.ToString() };
                estimateRange.Sequences.Add(sequence);
                _estimateService.UpdateEstimate(estimateRange).Wait();
            }
            //_replyService.Create(reply).Wait();
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }
    }
}
