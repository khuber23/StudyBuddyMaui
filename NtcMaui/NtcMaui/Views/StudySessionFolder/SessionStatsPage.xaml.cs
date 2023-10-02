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
        RunTime.Text = $"Session Run Time: {hours} hours, {minutes} minutes and {seconds} seconds";
        TotalCardsText.Text = $"Total Flashcards: {TotalCards}";
        CorrectFlashcardCount.Text = $"Correct Flashcards: {correctCards}";
        IncorrectFlashcardCount.Text = $"Incorrect Flashcards: {incorrectCards}";
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