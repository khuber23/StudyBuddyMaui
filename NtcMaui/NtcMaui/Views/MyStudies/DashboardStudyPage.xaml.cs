using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
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
        IncorrectCards = await GetIncorrectStudySessionsById(LoggedInUser.UserId);
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

    //when user clicks a flashcard it should take him straight to study priority page.
    private void FlashCardSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
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