﻿namespace NtcMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.SignAndCreate.SignIn), typeof(Views.SignAndCreate.SignIn));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.CreateAccount), typeof(Views.SignAndCreate.CreateAccount));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.Success), typeof(Views.SignAndCreate.Success));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.HomePage), typeof(Views.SignAndCreate.HomePage));
            Routing.RegisterRoute(nameof(Views.MyStudies.DeckGroupPage), typeof(Views.MyStudies.DeckGroupPage));

        }
    }
}