using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class FlashcardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public FlashcardPage()
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

        FlashcardListView.ItemsSource = await GetAllFlashcards();
    }

    public async Task<List<FlashCard>> GetAllFlashcards()
    {
        List<FlashCard> flashCards = new List<FlashCard>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Flashcard", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                flashCards = JsonSerializer.Deserialize<List<FlashCard>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return flashCards;
    }

    private void GoToDashboardPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
       List<FlashCard> flashCards = await GetAllFlashcards();

        if (flashCards != null)
        {
            if (e != null)
            {
                FlashcardListView.ItemsSource = null;
                FlashcardListView.ItemsSource = flashCards.Where(f => f.FlashCardQuestion.StartsWith(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
        }
       
    }
}