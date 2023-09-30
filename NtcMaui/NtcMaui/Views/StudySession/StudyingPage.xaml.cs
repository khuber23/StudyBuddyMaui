using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySession;

public partial class StudyingPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    int index = 0;
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
        //Adding another flashcard just for testing.
        DeckFlashCard deckFlashCard = new DeckFlashCard();
        FlashCard flashCard = new FlashCard();
        flashCard.FlashCardAnswer = "True";
        flashCard.FlashCardQuestion = "The First President Of the United States was George Washington. True of False?";
        deckFlashCard.FlashCard = flashCard;
        FlashCards = ChosenUserDeckGroup.DeckGroup.DeckGroupDeck.Deck.DeckFlashCards;
        FlashCards.Add(deckFlashCard);
        //need to get the flashcard at first instance. 
        //then after thsi when users swipe left or right we go to the next flashcard
        string firstquestionText = FlashCards[0].FlashCard.FlashCardQuestion;
        FlashcardText.Text = firstquestionText;
    }

    async void OnTapRecognized(object sender, TappedEventArgs args)
    {
        //old code for testing.
        if (FlashcardText.Text == FlashCards[index].FlashCard.FlashCardQuestion)
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = FlashCards[index].FlashCard.FlashCardAnswer;
        }
        else if (FlashcardText.Text == FlashCards[index].FlashCard.FlashCardAnswer)
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
        }
    }

    public User LoggedInUser { get; set; }
    public UserDeckGroup ChosenUserDeckGroup { get; set; }
    public List<DeckFlashCard> FlashCards { get; set; }

    public List<FlashCard> CorrectFlashCards = new List<FlashCard>();

    public List<FlashCard> IncorrectFlashCards = new List<FlashCard>();

    public FlashCard CurentFlashCard { get; set; }

    //when user swipes left it should take the current index card and add it to a group of wrong cards for example
    //and move onto the next card in the index.

    //when a user swipes right the it should take current index and add it to right group of cards and move onto next card index.
    // Both need to have something in place to check to for the last card and finish.
    private async void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                // Handle the swipe
                IncorrectFlashCards.Add(FlashCards[index].FlashCard);
                //when user is on the last card.
                if (index == (FlashCards.Count - 1))
                {
                    //store both card sets for right and wrong and continue to session stats Page.
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards }
                };
                   await Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
                }

                else
                {
                    index++;
                    FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
                }
                
                break;
            case SwipeDirection.Right:
                CorrectFlashCards.Add(FlashCards[index].FlashCard);
                //when user is on the last card.
                if (index == (FlashCards.Count - 1))
                {
                    //store both card sets for right and wrong and continue to session stats Page.
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards }
                };
                  await  Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
                }
                else
                {
                    index++;
                    FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
                }
                break;
        }
    }

    private async void DontUnderstandClick(object sender, EventArgs e)
    {
        IncorrectFlashCards.Add(FlashCards[index].FlashCard);
        //when user is on the last card.
        if (index == (FlashCards.Count -1))
        {
            //store both card sets for right and wrong and continue to session stats Page.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards }
                };
             await Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
        }
        else
        {
            index++;
            FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
        }
       
        
    }

    private async void UnderstandClick(object sender, EventArgs e)
    {
        CorrectFlashCards.Add(FlashCards[index].FlashCard);
        //when user is on the last card.
        if (index == (FlashCards.Count - 1))
        {
            //store both card sets for right and wrong and continue to session stats Page.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards }
                };
           await Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
        }
        else
        {
            index++;
            FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
        }
    }
}