using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class NewDeckGroupDeck : DeckGroupDeck
{
    public string Image { get; set; }
}

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
        DeckGroupDecks = await GetAllDecksbyDeckGroup();
        TestDecks = new List<DeckGroupDeck>();
        foreach(DeckGroupDeck deckGroupDeck in DeckGroupDecks)
        {
            NewDeckGroupDeck test = new NewDeckGroupDeck();
            test.DeckGroup = deckGroupDeck.DeckGroup;
            test.Deck = deckGroupDeck.Deck;
            test.Image = LoggedInUser.ProfilePicture;
            TestDecks.Add(test);
        }
        DeckListView.ItemsSource = TestDecks;
        DeckGroupNameLabel.Text = $"Building {SelectedDeckGroup.DeckGroupName} Deck Group";
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
        if (SelectedDeckGroup.ReadOnly == true)
        {
            ErrorLabel.Text = $"You can't add to {SelectedDeckGroup.DeckGroupName} as it isn't editable";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //needs to be DeckGroup for later for connecting Everything.
            {"Current DeckGroup", SelectedDeckGroup }
                };
            Shell.Current.GoToAsync(nameof(CreateDeckPage), navigationParameter);
        }
    }

    //will take the user to an importDeckPage
    private void ImportDeckBtn_Clicked(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //needs to be DeckGroup for later for connecting Everything.
            {"Current DeckGroup", SelectedDeckGroup }
                };
        Shell.Current.GoToAsync(nameof(ImportDeckPage), navigationParameter);
    }


    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        //eventually change this to be for editing or just button clicks later, for now not worrying about it
        if (e.SelectedItem != null)
        {
            //if checked go to edit Deck Page
            if (EditingCheckBox.IsChecked == true)
            {
                SelectedDeckGroupDeck = e.SelectedItem as DeckGroupDeck;
                if (SelectedDeckGroupDeck.Deck.ReadOnly == true)
                {
                    ErrorLabel.Text = $"{SelectedDeckGroupDeck.Deck.DeckName} isn't editable";
                    ErrorLabel.IsVisible = true;
                    //it's weird but this is the work around for dealing with re-selecting options for the item.
                    SelectedDeckGroupDeck = null;
                    DeckListView.SelectedItem = null;
                }
                else
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current Deck", SelectedDeckGroupDeck.Deck}
                };
                    Shell.Current.GoToAsync(nameof(EditDeckPage), navigationParameter);
                }   
            }
            else
            {
                //its a deckGroupDeck
                SelectedDeckGroupDeck = e.SelectedItem as DeckGroupDeck;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", SelectedDeckGroupDeck }
                };
                Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);
            }

        }
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

	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
	}

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}

	private void GoToDashboardPage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
	}

	private void GoToFlashcardPage(object sender, EventArgs e)
	{
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


	public User LoggedInUser { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }

    public DeckGroupDeck SelectedDeckGroupDeck { get; set; }

    public List<DeckGroupDeck> DeckGroupDecks { get; set; }

    public List<DeckGroupDeck> TestDecks { get; set; }
}