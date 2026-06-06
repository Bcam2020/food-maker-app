using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Threading.Tasks;

namespace assignment_2425.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StadiumTopBar : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(StadiumTopBar), string.Empty);
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public StadiumTopBar()
        {
            InitializeComponent();
            BindingContext = this;
        }
        private async void OnSettingsClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(assignment_2425.SettingsPage));
        }
    }
}
