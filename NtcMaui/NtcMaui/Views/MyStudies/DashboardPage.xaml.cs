using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class DashboardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    //need these for the progress bar
    int correctCount;
    int incorrectCount;

    public DashboardPage()
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
        if (StudySessionFlashCards.Count == 0 || StudySessionFlashCards == null)
        {
            //run a different code if no studySessionCards exist eventually if user goes to dashboard page with nothing made yet
            //or tested out yet.
        }
        else
        {
            AllStudyFlashcardsListView.ItemsSource = StudySessionFlashCards;
            TotalCount = StudySessionFlashCards.Count;

            foreach (var item in StudySessionFlashCards)
            {
                if (item.IsCorrect)
                {
                    CorrectCount++;
                }

            }
            decimal progress = Math.Round(((decimal)CorrectCount / TotalCount), 2);
            StatsProgressBar.Progress = (double)progress;
            ProgressLabel.Text = $"You are at {Math.Round(progress * 100)}% on your studying";
        }
    }

    protected override void OnDisappearing()
    {
        StatsProgressBar.Progress = 0;
        CorrectCount = 0;
        TotalCount = 0;
    }

    private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
	}

	private  void GoToDashboardPage(object sender, EventArgs e)
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

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}

	public User LoggedInUser { get; set; }

    public List<StudySessionFlashCard> StudySessionFlashCards { get; set; }

    public int TotalCount { get; set; }

    public int CorrectCount { get; set; }
}