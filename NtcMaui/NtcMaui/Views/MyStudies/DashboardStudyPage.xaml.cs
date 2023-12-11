using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;
using NtcMaui.Views.StudySessionFolder;

namespace NtcMaui.Views.MyStudies;

public partial class DashboardStudyPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public DashboardStudyPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        IncorrectCards = await Constants.GetIncorrectStudySessionsById(LoggedInUser.UserId);
        StudySessionFlashCardsListView.ItemsSource = IncorrectCards;
        //do something/disable button if the incorrectCards count = 0
        if (IncorrectCards.Count == 0 || IncorrectCards == null)
        {
            NoCardsToStudyLabel.IsVisible = true;

            NoCardsToStudyLabel.Text = "You have no Cards to study. Good Job";
        }
        else
        {
            NoCardsToStudyLabel.IsVisible = false;
        }
    }

	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
	}

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}


	//when user clicks a flashcard it should take him straight to study priority page.
	private void FlashCardSelected(object sender, TappedEventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
    }

    private void GoToDashboardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    //goes to the Dashboard history page
    private void GoToDashboardHistoryPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardHistoryPage), navigationParameter);
    }

    private void GoToDashboardStudyPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardStudyPage), navigationParameter);
    }
    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }

    public List<StudySessionFlashCard> IncorrectCards { get; set; }
}