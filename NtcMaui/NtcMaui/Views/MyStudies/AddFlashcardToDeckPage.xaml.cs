using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class AddFlashcardToDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AddFlashcardToDeckPage()
	{
		InitializeComponent();


	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedFlashCard = query["Selected FlashCard"] as FlashCard;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        CurrentFlashCardLabel.Text = $"Current Flashcard you wish to add: {SelectedFlashCard.FlashCardQuestion}";
        UserDecks = await GetAllDecks();
        DeckPicker.ItemsSource = await GetAllDecks();
    }

    public async Task<List<UserDeck>> GetAllDecks()
    {
        List<UserDeck> decks = new List<UserDeck>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck/maui/user/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                decks = JsonSerializer.Deserialize<List<UserDeck>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return decks;
    }

    public User LoggedInUser { get; set; }

    public List<UserDeck> UserDecks { get; set; }

    public UserDeck SelectedUserDeck { get; set; }

    public FlashCard SelectedFlashCard { get; set; }



    private void AddFlashcardToDeckBtn_Clicked(object sender, EventArgs e)
    {
    }

    private void DeckPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //if this is stuck as a string I can sit there and maybe make a endpoint to find the deck as based on the string
           var test = picker.Items[selectedIndex];
        }
    }
}