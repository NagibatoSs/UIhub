namespace UIhub.Models.ViewModels
{
    public class PostsListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Created { get; set; }
        public int Views { get; set; }
        public int EstimateCount {  get; set; }
        public bool isTop {  get; set; }
    }
}
