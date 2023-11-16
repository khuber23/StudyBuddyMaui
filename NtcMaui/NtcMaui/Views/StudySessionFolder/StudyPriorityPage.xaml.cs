using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using Microsoft.Maui.Controls;

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
        IncorrectCards = await GetIncorrectStudySessionsById(LoggedInUser.UserId);
        StudySessionFlashCardsListView.ItemsSource = IncorrectCards;
        //do something/disable button if the incorrectCards count = 0
        if (IncorrectCards.Count == 0 ||  IncorrectCards == null) 
        { 
            DoStudySessionBtn.IsVisible = false;
            ChosenCardLabel.Text = "You have no incorrect flashcards. Good Job!";
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

    private void GoToMyStudiesSessionPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }

    public async Task<List<StudySession>> GetFullStudySessionsById(int userId)
    {
        List<StudySession> studySessions = new List<StudySession>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySession/full/{userId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                studySessions = JsonSerializer.Deserialize<List<StudySession>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return studySessions;
    }

    public async Task<List<StudySessionFlashCard>> GetIncorrectStudySessionsById(int userId)
    {
        List<StudySessionFlashCard> studySessionFlashCards = new List<StudySessionFlashCard>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/maui/incorrect/{userId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                studySessionFlashCards = JsonSerializer.Deserialize<List<StudySessionFlashCard>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return studySessionFlashCards;
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
            ChosenCardLabel.Text = $"You have chosen {SelectedFlashCard.FlashCard.FlashCardQuestion} and any subsecquent flashcards to re-study. Click Begin Session to re-study these cards.";
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
            ChosenCardLabel.Text = "Please choose a flashcard to re-do your studysessions with";
        }

    }

    public User LoggedInUser { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public List<StudySessionFlashCard> IncorrectCards { get; set; }

    public List<StudySessionFlashCard> CardsToStudy { get; set; } = new List<StudySessionFlashCard>();

    public StudySessionFlashCard SelectedFlashCard { get; set; }


}