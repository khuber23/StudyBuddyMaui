using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private async void GoToBuildDeckPage(object sender, EventArgs e)
    {
        Deck = new Deck();
        Deck.DeckName = DeckNameEntry.Text;
        Deck.DeckDescription = DeckDescriptionEntry.Text;
        //await SaveDeckAsync(Deck);
        ////get all the Decks and re-find the one so we have an ID...since when posting it the Id would be 0.
        //List<Deck> decks = await GetAllDecks();
        //foreach (Deck deck in decks)
        //{
        //    if (deck.DeckName == Deck.DeckName && deck.DeckDescription == Deck.DeckDescription)
        //    {
        //        Deck = deck;
        //        break;
        //    }
        //}
        //after getting the right Deck With an ID from the database we make it a userDeck and post it to database.
        UserDeck userDeck = new UserDeck();
        userDeck.DeckId = Deck.DeckId;
        userDeck.UserId = LoggedInUser.UserId;
        //await SaveUserDeckAsync(userDeck); 
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        //do an if statement on the go to. So if they have a UserDeckGroup selected that isn't null! then send em to BuildDeckGroup
        //ask about that...where do they go since there is to ways of navigating to this page.
        //maybe make 2 seperate pages? one for when user clicks to add a deck from Deck and another from BuildDeckGroup
            await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    public async Task SaveDeckAsync(Deck deck)
    {
        //either will be api/userDeck or maybe just Deck?
        //for now i won't run anything but will just keep deck. (won't do a post essentially just comment it out)
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //remember the 1 to 1 relationship later on.
    public async Task SaveUserDeckAsync(UserDeck userDeck)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        //wait to see what they want from it and explain what you are thinking/what he envisions. you could have been right in the beginning 
        //with the idea of creating new ones from there and saving them or just choosing 1 like your new idea. --past Brody
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<UserDeck>(userDeck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //is going to get all of the Deck
    public async Task<List<Deck>> GetAllDecks()
    {
        List<Deck> decks = new List<Deck>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                decks = JsonSerializer.Deserialize<List<Deck>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return decks;
    }

    public User LoggedInUser { get; set; }
    
    public UserDeckGroup SelectedUserDeckroup { get; set; }

    public Deck Deck { get; set; }

    public UserDeck UserDeck { get; set; }
}