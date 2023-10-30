using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

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
        StudySessionFlashCards = await GetAllStudySessionFlashCards();
        AllStudyFlashcardsListView.ItemsSource = StudySessionFlashCards; 
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

    //will get the list of all StudySessionFlashcards
    public async Task<List<StudySessionFlashCard>> GetAllStudySessionFlashCards()
    {
        List<StudySessionFlashCard> flashcards = new List<StudySessionFlashCard>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/maui/full/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                flashcards = JsonSerializer.Deserialize<List<StudySessionFlashCard>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return flashcards;
    }

    public User LoggedInUser { get; set; }

    public List<StudySessionFlashCard> StudySessionFlashCards { get; set; }
}