using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

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
        CurrentFlashCardLabel.Text = $"{SelectedFlashCard.FlashCardQuestion}";
        CurrentFlashcardAnswer.Text = $"{SelectedFlashCard.FlashCardAnswer}";
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

    private async void AddFlashcardToDeckBtn_Clicked(object sender, EventArgs e)
    {
        bool failed = false;
        ErrorLabel.IsVisible = false;
        Deck selectedDeck = await GetDeckByDeckName(DeckName);
        if (selectedDeck == null || selectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            //check to see if the flashcard already exists and if it does shoot an error, else continue like normal
            foreach(DeckFlashCard flashcard in selectedDeck.DeckFlashCards)
            {
                if (flashcard.FlashCardId == SelectedFlashCard.FlashCardId)
                {
                    failed = true;
                    break;
                }
            }
            //then add the Selected flashcard to DeckFlashcards Post.
            if (failed)
            {
                ErrorLabel.Text = "Flashcard you are trying to add already exists in the deck. Please choose another deck or choose a different flashcard";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                DeckFlashCard deckFlashCard = new DeckFlashCard();
                deckFlashCard.FlashCardId = SelectedFlashCard.FlashCardId;
                deckFlashCard.DeckId = selectedDeck.DeckId;
                await SaveDeckFlashCardAsync(deckFlashCard);

                //then go back to the flashcards page
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                await Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
            }
        }
    }

    //gets the Deckname when a user chooses an option
    private void DeckPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this deckname to eventually find a deck in the next btn.
            ErrorLabel.IsVisible = false;
            DeckName = picker.Items[selectedIndex];
        }
    }

    /// <summary>
    /// gets the deck based on deck name
    /// </summary>
    /// <returns>a deck</returns>
    public async Task<Deck> GetDeckByDeckName(string DeckName)
    {
        Deck deck = new Deck();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck/deckname/{DeckName}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                deck = JsonSerializer.Deserialize<Deck>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return deck;
    }

    //posts the DeckFlashcard
    public async Task SaveDeckFlashCardAsync(DeckFlashCard deckFlashCard)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckFlashCard", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckFlashCard>(deckFlashCard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tdeckFlashCard successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public string DeckName { get; set; }

    public User LoggedInUser { get; set; }

    public List<UserDeck> UserDecks { get; set; }

    public UserDeck SelectedUserDeck { get; set; }

    public FlashCard SelectedFlashCard { get; set; }


	// Tabs for navigation 
	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
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
}