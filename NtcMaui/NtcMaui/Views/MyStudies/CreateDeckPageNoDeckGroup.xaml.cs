using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

//very similiar to the CreateDeckPage, just doesn't deal with DeckGroups and just focuses on User and user Deck. Basically doesn't deal with DeckGroupDeck
public partial class CreateDeckPageNoDeckGroup : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckPageNoDeckGroup()
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
        //Creates the Deck that the user will make
        Deck = new Deck();
        Deck.DeckName = DeckNameEntry.Text;
        Deck.DeckDescription = DeckDescriptionEntry.Text;
        await SaveDeckAsync(Deck);

        //get all the Decks and re-find the one so we have an ID...since when posting it the Id would be 0.
        List<Deck> decks = await GetAllDecks();
        foreach (Deck deck in decks)
        {
            if (deck.DeckName == Deck.DeckName && deck.DeckDescription == Deck.DeckDescription)
            {
                Deck = deck;
                break;
            }
        }
        //after getting the right Deck With an ID from the database we make it a userDeck and post it to database.
        UserDeck userDeck = new UserDeck();
        userDeck.DeckId = Deck.DeckId;
        userDeck.UserId = LoggedInUser.UserId;
        //creates the User Deck
        await SaveUserDeckAsync(userDeck);

        //pass in Deck so then Users can eventually add Flashcards to the deck.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", Deck }
                };
        //Finishing up making a DeckGroup so now it will take the user to Build Deck
        await Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);
    }

    //Posts the Deck
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

    //is going to get all of the Decks
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

    //remember the 1 to 1 relationship later on.
    //Posts the UserDeck
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

    public Deck Deck { get; set; }

    public UserDeck UserDeck { get; set; }

    public User LoggedInUser { get; set; }
}