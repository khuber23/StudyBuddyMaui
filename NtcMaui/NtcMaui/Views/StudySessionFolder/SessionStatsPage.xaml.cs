using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

public partial class SessionStatsPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public SessionStatsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TimeSpan span = StudySession.EndTime - StudySession.StartTime;
        double seconds = Math.Abs(span.Seconds);
        double minutes = Math.Abs(span.Minutes);
        double hours = Math.Abs(span.Hours);
        TotalCards = (IncorrectFlashCards.Count + CorrectFlashCards.Count);
        int correctCards = CorrectFlashCards.Count;
        int incorrectCards = IncorrectFlashCards.Count;
        RunTime.Text = $"Session Total Time: {hours} hours, {minutes} minutes and {seconds} seconds";
        TotalCardsText.Text = $"Total Cards: {TotalCards}";
        CorrectFlashcardCount.Text = $"# Correct: {correctCards}";
        IncorrectFlashcardCount.Text = $"# Incorrect: {incorrectCards}";

		decimal progress = Math.Round(((decimal)correctCards / TotalCards), 2);
		StatsProgressBar.Progress = (double)progress;
		ProgressLabel.Text = $"You have {Math.Round(progress * 100)}% Correct Cards in this Deck!";
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        //add incorrect and correct flashcards
        //Eventually keep the Session as well.
        LoggedInUser = query["Current User"] as User;
        CorrectFlashCards = query["Correct Cards"] as List<FlashCard>;
        IncorrectFlashCards = query["Incorrect Cards"] as List<FlashCard>;
        StudySession = query["Study Session"] as StudySession;
        OnPropertyChanged("Current User");

    }

    public async void GoToStudySession(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }

    public StudySession StudySession { get; set; }

    public List<FlashCard> CorrectFlashCards { get; set; }

    public List<FlashCard> IncorrectFlashCards { get; set; }

    public int TotalCards { get; set; }
}