using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;

namespace NtcMaui.Views.MyStudies;

public partial class BuildDeckPageOnlyDeck : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public BuildDeckPageOnlyDeck()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;

        OnPropertyChanged("Current User");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //this I want to eventually be all the Deck Flashcards. Work on that later.
        //FlashcardListView.ItemsSource = await GetAllFlashcards();
        FlashcardListView.ItemsSource = SelectedDeck.DeckFlashCards;
        BuildDeckNameLabel.Text = $"Building: {SelectedDeck.DeckName}";
    }

    //button Click For CreateFlashcardPage
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        if (SelectedDeck.ReadOnly == true)
        {
            ErrorLabel.Text = $"You can't add to {SelectedDeck.DeckName} as it isn't editable";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    { "Current Deck", SelectedDeck}
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
                SelectedFlashCard = e.SelectedItem as DeckFlashCard;
                if (SelectedDeck.ReadOnly == true)
                {
                    ErrorLabel.Text = $"{SelectedDeck.DeckName} isn't editable";
                    ErrorLabel.IsVisible = true;
                    //it's weird but this is the work around for dealing with re-selecting options for the item.
                    SelectedFlashCard = null;
                    FlashcardListView.SelectedItem = null;
                }
                else
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current FlashCard", SelectedFlashCard.FlashCard},
                    //need this to be able to go back to this page after editing flashcard
                    {"Current Deck", SelectedDeck }
                };
                    Shell.Current.GoToAsync(nameof(EditFlashCardOnlyDeck), navigationParameter);
                }
                
            }
            else
            {
                FlashcardListView.SelectedItem = null;
            }
        }
    }

    public User LoggedInUser { get; set; }

    public DeckFlashCard SelectedFlashCard { get; set; }

    public Deck SelectedDeck { get; set; }


}