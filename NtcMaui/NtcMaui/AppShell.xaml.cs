namespace NtcMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Sign In routes
            Routing.RegisterRoute(nameof(Views.SignAndCreate.SignIn), typeof(Views.SignAndCreate.SignIn));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.CreateAccount), typeof(Views.SignAndCreate.CreateAccount));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.Success), typeof(Views.SignAndCreate.Success));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.HomePage), typeof(Views.SignAndCreate.HomePage));
            Routing.RegisterRoute(nameof(Views.SignAndCreate.ForgotPassword), typeof(Views.SignAndCreate.ForgotPassword));


            //my study routes
            Routing.RegisterRoute(nameof(Views.MyStudies.DeckGroupPage), typeof(Views.MyStudies.DeckGroupPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.DeckPage), typeof(Views.MyStudies.DeckPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.DashboardPage), typeof(Views.MyStudies.DashboardPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.DashboardStudyPage), typeof(Views.MyStudies.DashboardStudyPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.DashboardHistoryPage), typeof(Views.MyStudies.DashboardHistoryPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.FlashcardPage), typeof(Views.MyStudies.FlashcardPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.CreateDeckGroupPage), typeof(Views.MyStudies.CreateDeckGroupPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.BuildDeckGroupPage), typeof(Views.MyStudies.BuildDeckGroupPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.CreateDeckPage), typeof(Views.MyStudies.CreateDeckPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.BuildDeckPage), typeof(Views.MyStudies.BuildDeckPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.CreateFlashcardPage), typeof(Views.MyStudies.CreateFlashcardPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.CreateDeckPageNoDeckGroup), typeof(Views.MyStudies.CreateDeckPageNoDeckGroup));
            Routing.RegisterRoute(nameof(Views.MyStudies.BuildDeckPageOnlyDeck), typeof(Views.MyStudies.BuildDeckPageOnlyDeck));
            Routing.RegisterRoute(nameof(Views.MyStudies.AddFlashcardToDeckPage), typeof(Views.MyStudies.AddFlashcardToDeckPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.ImportDeckPage), typeof(Views.MyStudies.ImportDeckPage));
            Routing.RegisterRoute(nameof(Views.MyStudies.ExportDeckPage), typeof(Views.MyStudies.ExportDeckPage));

            //Study Session Routes
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.MyStudiesSessionPage), typeof(Views.StudySessionFolder.MyStudiesSessionPage));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.StudyingPage), typeof(Views.StudySessionFolder.StudyingPage));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.SessionStatsPage), typeof(Views.StudySessionFolder.SessionStatsPage));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.StudyPriorityPage), typeof(Views.StudySessionFolder.StudyPriorityPage));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.StudyingPageFromStudyPriority), typeof(Views.StudySessionFolder.StudyingPageFromStudyPriority));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.MyStudiesSessionPageOnlyUserDecks), typeof(Views.StudySessionFolder.MyStudiesSessionPageOnlyUserDecks));
            Routing.RegisterRoute(nameof(Views.StudySessionFolder.StudyingPageNoDeckGroups), typeof(Views.StudySessionFolder.StudyingPageNoDeckGroups));

            //editing routes
            Routing.RegisterRoute(nameof(Views.Edit.EditDeckGroupPage), typeof(Views.Edit.EditDeckGroupPage));
            Routing.RegisterRoute(nameof(Views.Edit.EditDeckPage), typeof(Views.Edit.EditDeckPage));
            Routing.RegisterRoute(nameof(Views.Edit.EditFlashCardPageWithDeckGroup), typeof(Views.Edit.EditFlashCardPageWithDeckGroup));
            Routing.RegisterRoute(nameof(Views.Edit.EditFlashCardOnlyDeck), typeof(Views.Edit.EditFlashCardOnlyDeck));

            //Sharing Routes
            Routing.RegisterRoute(nameof(Views.Share.ShareDeckPage), typeof(Views.Share.ShareDeckPage));
            Routing.RegisterRoute(nameof(Views.Share.ShareDeckGroupPage), typeof(Views.Share.ShareDeckGroupPage));
            Routing.RegisterRoute(nameof(Views.Share.ShareDeckWithUserPage), typeof(Views.Share.ShareDeckWithUserPage));
            Routing.RegisterRoute(nameof(Views.Share.ShareDeckGroupWithUserPage), typeof(Views.Share.ShareDeckGroupWithUserPage));

            //Admin Routes
            Routing.RegisterRoute(nameof(Views.Admin.AdminHomePage), typeof(Views.Admin.AdminHomePage));
            Routing.RegisterRoute(nameof(Views.Admin.AdminFlashCardPage), typeof(Views.Admin.AdminFlashCardPage));
            Routing.RegisterRoute(nameof(Views.Admin.AdminDeckPage), typeof(Views.Admin.AdminDeckPage));
            Routing.RegisterRoute(nameof(Views.Admin.AdminDeckGroupPage), typeof(Views.Admin.AdminDeckGroupPage));
            Routing.RegisterRoute(nameof(Views.Admin.AdminEditDeckPage), typeof(Views.Admin.AdminEditDeckPage));
        }
    }
}