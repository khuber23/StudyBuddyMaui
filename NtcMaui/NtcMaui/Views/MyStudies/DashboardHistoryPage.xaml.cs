using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class DashboardHistoryPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public DashboardHistoryPage()
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
        StudySessionFlashCards = await Constants.GetAllStudySessionFlashCards(LoggedInUser.UserId);
        AllStudyFlashcardsListView.ItemsSource = StudySessionFlashCards; 
    }

	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
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

    public List<StudySessionFlashCard> StudySessionFlashCards { get; set; }
}