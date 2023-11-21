using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class ImportDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ImportDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        CurrentDeckGroupLabel.Text = $"{SelectedDeckGroup.DeckGroupName}";
        DeckPicker.ItemsSource = await Constants.GetAllUserDecksById(LoggedInUser.UserId);
        DeckGroupDecks = await GetAllDeckGroupDecks();
        SelectedDeckGroup.DeckGroupDecks = DeckGroupDecks.Where(deckGroup => deckGroup.DeckGroupId == SelectedDeckGroup.DeckGroupId).ToList();
        //should get all the DeckGroupDecks related to the Selected DeckGroup  (used for sharing and updating shared Decks)
        DeckGroupDecks = DeckGroupDecks.Where(deck => deck.DeckGroup.DeckGroupId == SelectedDeckGroup.DeckGroupId || deck.DeckGroup.DeckGroupName == SelectedDeckGroup.DeckGroupName).ToList();
    }

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

    private async void AddDeckToDeckGroupBtn_Clicked(object sender, EventArgs e)
    {
        bool failed = false;
        ErrorLabel.IsVisible = false;
        SelectedDeck = await GetDeckByDeckName(DeckName);
        if (SelectedDeck == null || SelectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            foreach (DeckGroupDeck deckGroupdeck in SelectedDeckGroup.DeckGroupDecks)
            {
                if (deckGroupdeck.DeckId == SelectedDeck.DeckId)
                {
                    failed = true;
                    break;
                }
            }
            if (failed)
            {
                ErrorLabel.Text = "The deck you are trying to add already exists in the deck group. Please choose another deck or pick a different deck group.";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                //so basically if the user has a brand new deckgroup without any decks in it run this code.
                if (DeckGroupDecks.Count == 0)
                {
                    DeckGroupDeck newDeckGroupDeck = new DeckGroupDeck();
                    newDeckGroupDeck.DeckId = SelectedDeck.DeckId;
                    newDeckGroupDeck.DeckGroupId = SelectedDeckGroup.DeckGroupId;
                    await SaveDeckGroupDeckAsync(newDeckGroupDeck);
                }
                //if the user has already made a deck within the deckgroup and it also checks for shared instances.
                else
                {
                    //we need this to just get the small instanced by DeckGroupId, otherwise we would accidentally/import the deck for every deckgroupdeck in there.
                    var filteredList = DeckGroupDecks.GroupBy(deckgroupDeck => deckgroupDeck.DeckGroupId).Select(test => test.First()).ToList();
                    //we need to make sure anything shared gets updated as well.
                    foreach (DeckGroupDeck deckGroupDeck in filteredList)
                    {
                        DeckGroupDeck newDeckGroupDeck = new DeckGroupDeck();
                        newDeckGroupDeck.DeckId = SelectedDeck.DeckId;
                        newDeckGroupDeck.DeckGroupId = deckGroupDeck.DeckGroupId;
                        await SaveDeckGroupDeckAsync(newDeckGroupDeck);
                    }
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
            }
        }
    }

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

    //posts the DeckGroupDeck
    public async Task SaveDeckGroupDeckAsync(DeckGroupDeck deckGroupDeck)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroupDeck>(deckGroupDeck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tdeckGroupDeck successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
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

    //is going to get all of the Deck
    public async Task<List<DeckGroupDeck>> GetAllDeckGroupDecks()
    {
        List<DeckGroupDeck> deckGroupDecks = new List<DeckGroupDeck>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                deckGroupDecks = JsonSerializer.Deserialize<List<DeckGroupDeck>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return deckGroupDecks;
    }

    public User LoggedInUser { get; set; }

    public string DeckName { get; set; }

    public Deck SelectedDeck { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }

    public List<DeckGroupDeck> DeckGroupDecks { get; set; }

 
}