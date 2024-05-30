using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Service;

namespace UIhub.Rating
{
    public static class RatingData
    {
        public static int TopCost { get; } = 100;
        public static int PointsByLike { get; } = 5;
        public static int PointsByEstimate { get; } = 1;
        public static int RatingForLike { get; } = 50;
    }
}
