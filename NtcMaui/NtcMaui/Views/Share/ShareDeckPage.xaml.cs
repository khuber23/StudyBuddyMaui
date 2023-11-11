using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.Share;

public partial class ShareDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckPage()
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
        UserDeckGroups = await GetAllUserDeckGroups();
        UserDecks = await GetAllDecks();
        List<UserDeck> DecksNotInDeckGroup = new List<UserDeck>();
        List<int> deckIds = new List<int>();
        foreach (UserDeckGroup userdeckgroup in UserDeckGroups)
        {
            //test
            foreach (DeckGroupDeck deckGroupDeck in userdeckgroup.DeckGroup.DeckGroupDecks)
            {
                deckIds.Add(deckGroupDeck.DeckId);
            }
        }

        UserDecks.Where(userdeck => deckIds.Contains(userdeck.DeckId)).ToList().ForEach(userdeck => UserDecks.Remove(userdeck));
        DecksNotInDeckGroup = UserDecks;
        //basically sets the items source to be all the Decks not found in the UserDeckGroups related to a user
        if (DecksNotInDeckGroup.Count == 0)
        {
            //throw an error saying you have no decks not in a deck group or if they just don't have decks.
            ErrorLabel.Text = "You have no decks not in a deck group or you have not created decks. Please create a deck or share a deckgroup. Thanks";
            ErrorLabel.IsVisible = true;
        }
        DeckPicker.ItemsSource = DecksNotInDeckGroup;
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

    //will take them to the next page dealing with Copying a deck to a user
    private async void CopyBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        Deck selectedDeck = await GetDeckByDeckName(DeckName);
        if (selectedDeck == null || selectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeck = selectedDeck;
            string chosenShareType = "Copy";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current Deck" , SelectedDeck},
                {"Share Type", chosenShareType }
                };
            await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
        }
    }

    //will take them to the next page dealing with cloning a deck to a user
    private async void CloneBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        Deck selectedDeck = await GetDeckByDeckName(DeckName);
        if (selectedDeck == null || selectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeck = selectedDeck;
            string chosenShareType = "Clone";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current Deck" , SelectedDeck},
                {"Share Type", chosenShareType }
                };
            await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
        }
    }


    //might be an issue later but...i think it's fine
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

    public async Task<List<UserDeckGroup>> GetAllUserDeckGroups()
    {
        List<UserDeckGroup> userDeckGroups = new List<UserDeckGroup>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/maui/user/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                userDeckGroups = JsonSerializer.Deserialize<List<UserDeckGroup>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return userDeckGroups;
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

    public string DeckName { get; set; }

    public Deck SelectedDeck { get; set; }

    public List<UserDeck> UserDecks { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }


}