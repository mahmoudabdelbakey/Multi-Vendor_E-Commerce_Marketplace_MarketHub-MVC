namespace MarketHub.MVC.ViewModels
{
    public class HomeViewModel
    {
        public string Title { get; set; } = "Welcome to MarketHub";

        public string Subtitle { get; set; } =
            "Discover products from multiple stores in one place.";

        public IReadOnlyList<HomeCategoryViewModel> Categories { get; set; }
            = Array.Empty<HomeCategoryViewModel>();
    }
}
