using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using Microsoft.Maui.Controls;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.StudySessionFolder;

public partial class StudyPriorityPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public StudyPriorityPage()
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
        if (IncorrectCards.Count == 0 ||  IncorrectCards == null) 
        { 
            DoStudySessionBtn.IsVisible = false;
            ChosenCardLabel.Text = "You have zero incorrect Flashcards. Good Job!";
        }
        else
        {
            DoStudySessionBtn.IsVisible = true;
            ChosenCardLabel.Text = string.Empty;
        }
    }

    private void GoToStudyPriorityPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
    }

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}

	private void GoToMyStudiesSessionPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }

    private void FlashCardSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            //with this we can get the Id of the study session
            SelectedFlashCard = e.SelectedItem as StudySessionFlashCard;

            //if person would re-choose a different flashcard we need to clear the CardsToStudy list otherwise it just keeps re-adding the same card.
            CardsToStudy.Clear();
            
            //so when a user clicks on a flashcard it can go throgh and find similiar incorrect flashcards to eventually study/re-study based on Id
            foreach (StudySessionFlashCard flashCard in IncorrectCards)
            {
                if (flashCard.StudySessionId == SelectedFlashCard.StudySessionId)
                {
                    CardsToStudy.Add(flashCard);
                }
            }
            ChosenCardLabel.Text = $"Current Flashcard chosen: {SelectedFlashCard.FlashCard.FlashCardQuestion}";
        }
    }

    private void BeginStudySession(object sender, EventArgs e)
    {   
        if (SelectedFlashCard != null)
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Study Session", SelectedFlashCard.StudySession },
                    {"Cards to Study", CardsToStudy }
                };
            Shell.Current.GoToAsync(nameof(StudyingPageFromStudyPriority), navigationParameter);
        }
        else
        {
            ChosenCardLabel.Text = "Please choose a flashcard to re-do your Study Session with.";
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

	public User LoggedInUser { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public List<StudySessionFlashCard> IncorrectCards { get; set; }

    public List<StudySessionFlashCard> CardsToStudy { get; set; } = new List<StudySessionFlashCard>();

    public StudySessionFlashCard SelectedFlashCard { get; set; }


}