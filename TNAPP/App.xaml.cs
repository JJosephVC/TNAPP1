using TNAPP.Views;

namespace TNAPP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Forzar tema claro
            Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
