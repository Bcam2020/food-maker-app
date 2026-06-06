using Microsoft.Maui.Controls;

namespace assignment_2425.Controls
{
    public partial class StadiumBottomBar : ContentView
    {
        public StadiumBottomBar()
        {
            InitializeComponent();
        }
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent is VisualElement parentElement)
            {
                parentElement.SizeChanged += ParentElement_SizeChanged;
                this.WidthRequest = parentElement.Width * 0.9;
            }
        }
        private void ParentElement_SizeChanged(object sender, System.EventArgs e)
        {
            if (sender is VisualElement parentElement)
            {
                this.WidthRequest = parentElement.Width * 0.9;
            }
        }
        private async void OnHomeClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        private async void OnAddRecipeClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//AddRecipePage");
        }
        private async void OnCommunityClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommunityPage");
        }
        private async void OnProfileClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
        private async void OnFavoritesClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//FavoritesPage");
        }
    }
}
