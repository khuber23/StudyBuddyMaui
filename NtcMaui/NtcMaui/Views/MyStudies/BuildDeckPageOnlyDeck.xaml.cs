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

    //button Click For CreateFlashcardPage
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    { "Current Deck", SelectedDeck}
                };
        Shell.Current.GoToAsync(nameof(CreateFlashcardPage), navigationParameter);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //this I want to eventually be all the Deck Flashcards. Work on that later.
        //FlashcardListView.ItemsSource = await GetAllFlashcards();
        FlashcardListView.ItemsSource = SelectedDeck.DeckFlashCards;
        BuildDeckNameLabel.Text = $"Building: {SelectedDeck.DeckName}";
    }

    private void FlashcardListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            if (EditingCheckBox.IsChecked == true)
            {
                SelectedFlashCard = e.SelectedItem as DeckFlashCard;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current FlashCard", SelectedFlashCard.FlashCard},
                    //need this to be able to go back to this page after editing flashcard
                    {"Current Deck", SelectedDeck }
                };
                Shell.Current.GoToAsync(nameof(EditFlashCardOnlyDeck), navigationParameter);
                SelectedFlashCard = null;
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