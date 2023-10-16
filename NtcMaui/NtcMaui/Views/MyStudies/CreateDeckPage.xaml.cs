using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

//might eventually need to copy this page and make a different one just for dealing with creatingUSerDeckGroup and just creating a UserDeck.
//most definetly since this code deals with DeckGroupDecks and if there is just a UserDeck I won't need this and I can't deal with nulls when applying Query attributes.
public partial class CreateDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }

    private async void GoToBuildDeckPage(object sender, EventArgs e)
    {
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

        //need a method to create the DeckGroupDeck part to combine the DeckGroup with the Created Deck.
        DeckGroupDeck deckGroupDeck = new DeckGroupDeck();
        deckGroupDeck.DeckGroupId = SelectedDeckGroup.DeckGroupId;
        deckGroupDeck.DeckId = Deck.DeckId;

        await SaveDeckGroupDeckAsync(deckGroupDeck);

        //pass in Deck so then Users can eventually add Flashcards to the deck.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", Deck }
                };
        //Finishing up making a DeckGroup so now it will take the user to Build Deck
            await Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);
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

    public async Task SaveDeckGroupDeckAsync(DeckGroupDeck deckGroupDeck)
    {
        //either will be api/userDeck or maybe just Deck?
        //for now i won't run anything but will just keep deck. (won't do a post essentially just comment it out)
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroupDeck>(deckGroupDeck, Constants._serializerOptions);
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
    
    public DeckGroup SelectedDeckGroup { get; set; }

    public Deck Deck { get; set; }

    public UserDeck UserDeck { get; set; }
}