using NtcMaui.Views.SignAndCreate;

namespace NtcMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}