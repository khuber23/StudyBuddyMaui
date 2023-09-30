using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySession;

public partial class SessionStatsPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public SessionStatsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        //add incorrect and correct flashcards
        LoggedInUser = query["Current User"] as User;
        CorrectFlashCards = query["Correct Cards"] as List<FlashCard>;
        IncorrectFlashCards = query["Incorrect Cards"] as List<FlashCard>;
        OnPropertyChanged("Current User");

    }

    public User LoggedInUser { get; set; }

    public List<FlashCard> CorrectFlashCards { get; set; }

    public List<FlashCard> IncorrectFlashCards { get; set; }
}