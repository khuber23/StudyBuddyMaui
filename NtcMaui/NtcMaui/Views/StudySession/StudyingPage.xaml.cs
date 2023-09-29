using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySession;

public partial class StudyingPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public StudyingPage()
	{
		InitializeComponent();
	}

    //might also add in the SelectedUserDeckGroup.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        ChosenUserDeckGroup = query["ChosenStudy"] as UserDeckGroup;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        //need to make a list of the cards.
        FlashCards = ChosenUserDeckGroup.DeckGroup.DeckGroupDeck.Deck.DeckFlashCards;
        //need to get the flashcard at first instance. 
        //then after thsi when users swipe left or right we go to the next flashcard
     string firstquestionText =  FlashCards[0].FlashCard.FlashCardQuestion;
        FlashcardText.Text = firstquestionText;
    }

    async void OnTapRecognized(object sender, TappedEventArgs args)
    {
        //old code for testing.
        //if (FlashcardText.Text == "Here is some filler text")
        //{
        //    await Flashcard.RotateXTo(180, 500);
        //    Flashcard.RotationX = 0;
        //    FlashcardText.Text = "text has changed";
        //}
        //else if (FlashcardText.Text == "text has changed")
        //{
        //    await Flashcard.RotateXTo(180, 500);
        //    Flashcard.RotationX = 0;
        //    FlashcardText.Text = "Here is some filler text";
        //}
    }

    public User LoggedInUser { get; set; }
    public UserDeckGroup ChosenUserDeckGroup { get; set; }
    public List<DeckFlashCard> FlashCards { get; set; }
}