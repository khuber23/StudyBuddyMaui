using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class BuildDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public BuildDeckGroupPage()
	{
		InitializeComponent();
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        //change this to get all the decks in whatever deckGroup they selected.
        //need to wait until api is changed but essentially need an endpoint in DeckGroup
        //as of 5:30 10/16/2023 this will get all of the Deck based on DeckGroup Id which will only belong to 1 user as of this moment.
        DeckListView.ItemsSource = await GetAllDecksbyDeckGroup();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }



    //button click event for Building off of DeckGroup
    private void GoToCreateDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //needs to be DeckGroup for later for connecting Everything.
            {"Current DeckGroup", SelectedDeckGroup }
                };
        Shell.Current.GoToAsync(nameof(CreateDeckPage), navigationParameter);
    }

    public async Task<List<DeckGroupDeck>> GetAllDecksbyDeckGroup()
    {
        List<DeckGroupDeck> deckGroupDecks = new List<DeckGroupDeck>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck/maui/{SelectedDeckGroup.DeckGroupId}", string.Empty));
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

    public DeckGroup SelectedDeckGroup { get; set; }

    public UserDeck SelectedDeck { get; set; }

    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        //eventually change this to be for editing or just button clicks later, for now not worrying about it
        if (e.SelectedItem != null)
        {
            SelectedDeck = e.SelectedItem as UserDeck;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Selected UserDeck", SelectedDeck }
                };
            Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);

        }
    }
}