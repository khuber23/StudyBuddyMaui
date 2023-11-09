using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Edit;

public partial class EditDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        List<Deck> decks = await GetAllDecks();
        DecksToEdit = decks.Where(d => d.DeckName == SelectedDeck.DeckName).ToList();

        DeckNameEntry.Text = SelectedDeck.DeckName;
        DeckDescriptionEntry.Text = SelectedDeck.DeckDescription;
    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        foreach(Deck deck in DecksToEdit)
        {
            SelectedDeck = deck;
            SelectedDeck.DeckName = DeckNameEntry.Text;
            SelectedDeck.DeckDescription = DeckDescriptionEntry.Text;
            SelectedDeck.IsPublic = IsPublic;
            //the first item should be the one similiar to the one needing to be edited...so anything after are the shared dekcs.
            if(DecksToEdit.First().DeckId == SelectedDeck.DeckId)
            {
                SelectedDeck.IsPublic = IsPublic;
            }
            else
            {
                SelectedDeck.IsPublic = false;
            }
            await PutDeckAsync(SelectedDeck);
        }
        
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    private void IsPublicCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            IsPublic = true;
        }
        else
        {
            IsPublic = false;
        }
    }

    /// <summary>
    /// Does a Put command on the deck
    /// </summary>
    /// <param name="deck">the deck you are updating</param>
    /// <returns>updated deck</returns>
    public async Task PutDeckAsync(Deck deck)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck/%7Bid%7D?deckid={SelectedDeck.DeckId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tdeck successfully updated.");
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

    public bool IsPublic { get; set; }

    public User LoggedInUser { get; set; }

    public Deck SelectedDeck { get; set; }

    //gets a list of all the decks that can be editable (used for shared decks mostly to updated stuff for shared)
    public List<Deck> DecksToEdit { get; set; }


}