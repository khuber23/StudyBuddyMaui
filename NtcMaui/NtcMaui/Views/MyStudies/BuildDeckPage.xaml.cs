using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class BuildDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public BuildDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    //might need to eventually add an if else statement for if a user is just making a flashcard for a deck of if there is also a deck group involved...
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(CreateFlashcardPage), navigationParameter);
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

    public User LoggedInUser { get; set; }
}