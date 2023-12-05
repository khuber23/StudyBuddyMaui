using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class NewDeckFlashCard : DeckFlashCard
{
    public string Image { get; set; }
}

//eventually need to make a view flashcard page to see the details and eventually edit like images and stuff.
public partial class BuildDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public BuildDeckPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroupDeck = query["Current Deck"] as DeckGroupDeck;

        OnPropertyChanged("Current User");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //this I want to eventually be all the Deck Flashcards. Work on that later.
        DeckFlashCards = SelectedDeckGroupDeck.Deck.DeckFlashCards;
        
        List<DeckFlashCard> NewDeckFlashCards = new List<DeckFlashCard>();
        foreach (DeckFlashCard card in DeckFlashCards)
        {
            NewDeckFlashCard newCard = new NewDeckFlashCard();
            newCard.FlashCard = card.FlashCard;
            newCard.Deck = card.Deck;
            newCard.Image = LoggedInUser.ProfilePicture;
            NewDeckFlashCards.Add(newCard);
        }
        FlashcardListView.ItemsSource = NewDeckFlashCards;
        BuildDeckNameLabel.Text = $"Building {SelectedDeckGroupDeck.Deck.DeckName} Deck";
    }

    //button Click For CreateFlashcardPage
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        if (SelectedDeckGroupDeck.Deck.ReadOnly == true)
        {
            ErrorLabel.Text = $"You can't add to {SelectedDeckGroupDeck.Deck.DeckName} as it isn't editable";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    { "Current Deck", SelectedDeckGroupDeck.Deck},
                };
            Shell.Current.GoToAsync(nameof(CreateFlashcardPage), navigationParameter);
        }
        
    }

    private void FlashcardListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        { 
           if (EditingCheckBox.IsChecked == true)
            {
                SelectedDeckFlashCard = e.SelectedItem as DeckFlashCard;
                if (SelectedDeckGroupDeck.Deck.ReadOnly == true)
                {
                    ErrorLabel.Text = $"{SelectedDeckGroupDeck.Deck.DeckName} isn't editable";
                    ErrorLabel.IsVisible = true;
                    //it's weird but this is the work around for dealing with re-selecting options for the item.
                    SelectedDeckFlashCard = null;
                    FlashcardListView.SelectedItem = null;
                }
                else
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current FlashCard", SelectedDeckFlashCard.FlashCard},
                    //need this to be able to go back to this page after editing flashcard
                    {"Current Deck", SelectedDeckGroupDeck }
                };
                    Shell.Current.GoToAsync(nameof(EditFlashCardPageWithDeckGroup), navigationParameter);
                }
                
            }
            else
            {
                FlashcardListView.SelectedItem = null;
            }
        }
    }

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

    public DeckGroupDeck SelectedDeckGroupDeck { get; set; }

    public DeckFlashCard SelectedDeckFlashCard { get; set; }

    public List<DeckFlashCard> DeckFlashCards { get; set; }


}