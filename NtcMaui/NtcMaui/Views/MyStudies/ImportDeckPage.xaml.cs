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
        CurrentDeckGroupLabel.Text = $" Selected DeckGroup: {SelectedDeckGroup.DeckGroupName}";
        DeckPicker.ItemsSource = await Constants.GetAllUserDecksById(LoggedInUser.UserId);
        DeckGroupDecks = await Constants.GetAllDeckGroupDecks();
        SelectedDeckGroup.DeckGroupDecks = DeckGroupDecks.Where(deckGroup => deckGroup.DeckGroupId == SelectedDeckGroup.DeckGroupId).ToList();
        //should get all the DeckGroupDecks related to the Selected DeckGroup  (used for sharing and updating shared Decks)
        DeckGroupDecks = DeckGroupDecks.Where(deck => deck.DeckGroup.DeckGroupId == SelectedDeckGroup.DeckGroupId || deck.DeckGroup.DeckGroupName == SelectedDeckGroup.DeckGroupName).ToList();
    }

    private async void DeckPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this deckname to eventually find a deck in the next btn.
            ErrorLabel.IsVisible = false;
            DeckName = picker.Items[selectedIndex];
            SelectedDeck = await Constants.GetDeckByDeckName(DeckName);
            DeckNameLabel.Text = $"The deck you chose {SelectedDeck.DeckName}";
            FlashcardDetails.ItemsSource = SelectedDeck.DeckFlashCards;
        }
    }

    private async void AddDeckToDeckGroupBtn_Clicked(object sender, EventArgs e)
    {
        bool failed = false;
        ErrorLabel.IsVisible = false;
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
                    await Constants.SaveDeckGroupDeckAsync(newDeckGroupDeck);
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
                        await Constants.SaveDeckGroupDeckAsync(newDeckGroupDeck);
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

    public User LoggedInUser { get; set; }

    public string DeckName { get; set; }

    public Deck SelectedDeck { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }

    public List<DeckGroupDeck> DeckGroupDecks { get; set; }

 
}