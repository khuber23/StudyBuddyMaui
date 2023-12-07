using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

public partial class MyStudiesSessionPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public MyStudiesSessionPage()
	{
		InitializeComponent();
	}

    //when i eventually start using this page i can use the item selected thingy of the list view to get the item that was selected
    //in this case being a deckgroup/deck. Then i can use it to eventually find all the flashcards in the thing and make a list of flashcards.
    //to get them to display we might need a mehtod/way of tracking the index of the cards in a list. with [0] and maybe like a counter.
    //then when it's done or when the counter == the last index of the cards add a button to finish.
    //also need a way of using the swipe methods for left and right

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        UserDeckGroups = await Constants.GetAllUserDeckGroupByUserId(LoggedInUser.UserId);
        foreach (UserDeckGroup userdeckgroup in UserDeckGroups)
        {
            //
            userdeckgroup.DeckGroup.DeckGroupDecks = await Constants.GetDeckGroupDecksByDeckGroupId(userdeckgroup.DeckGroupId);
        }

        MyStudiesListView.ItemsSource = UserDeckGroups;
    }

    //going to have to re-do this part to deal with probably just user decks.
    //Eventually this code will be used for if a user choosen A UserDeckGroup and can probably do all of the Decks within.
    private void BeginSession(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        if (ChosenDeckGroupDeck != null)
        {
            var navigationParameter = new Dictionary<string, object>
            {
            { "Current User", LoggedInUser },
            {"Chosen Deck", ChosenDeckGroupDeck }
            };
            //might need to make seperate pages eventually for StudyPage with just a deck eventually.
            Shell.Current.GoToAsync(nameof(StudyingPage), navigationParameter);
        }
        else
        {
            ErrorLabel.IsVisible = true;
        }
    }

    private void GoToStudyPriorityPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
    }

    private void GoToMyStudiesSessionPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }

    private void GoToMyStudiesSessionPageNoDeckGroups(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPageOnlyUserDecks), navigationParameter);
    }


    //will assign a deckGroupDeck when a user clicks on it.
    private void DeckGroupDeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            ChosenDeckGroupDeck = e.SelectedItem as DeckGroupDeck;
            ChosenDeckLabel.IsVisible = true;
            ChosenDeckLabel.Text = $"Current Deck Chosen: {ChosenDeckGroupDeck.Deck.DeckName}";
         }

    }

    public User LoggedInUser { get; set; }

    public UserDeckGroup ChosenUserDeckGroup { get; set; }

    public List<UserDeck> DecksNotInDeckGroup { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }

    public DeckGroupDeck ChosenDeckGroupDeck { get; set; }

    public List<DeckFlashCard> DeckFlashCards { get; set; }

}