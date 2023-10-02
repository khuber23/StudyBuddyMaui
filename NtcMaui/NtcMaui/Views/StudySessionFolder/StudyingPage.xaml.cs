using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

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
        StartSessionTime = DateTime.Now;

        //need to make a list of the cards.
        FlashCards = ChosenUserDeckGroup.DeckGroup.DeckGroupDeck.Deck.DeckFlashCards;
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

    public DateTime StartSessionTime { get; set; }

    public DateTime EndSessionTime { get; set; }

    public List<FlashCard> CorrectFlashCards = new List<FlashCard>();

    public List<FlashCard> IncorrectFlashCards = new List<FlashCard>();

    public StudySession StudySession { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public StudySessionFlashCard StudySessionFlashCard {get; set;}

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
            //now we need to make the StudySession here and eventually call a post to post it.
            EndSessionTime = DateTime.Now;

            StudySession = new StudySession();
            //this might fail since the datbase has a time of datetime2
            StudySession.StartTime = StartSessionTime;
            StudySession.EndTime = EndSessionTime;
            StudySession.UserId = LoggedInUser.UserId;
            StudySession.DeckGroupId = ChosenUserDeckGroup.DeckGroupId;
            StudySession.DeckId = ChosenUserDeckGroup.DeckGroup.DeckGroupDeck.Deck.DeckId;
            StudySessions =  await GetAllStudySessions();

            //post the StudySession.
            //after posting you need to retrieve it to upload the study session flashcard.
            //then make Post for Study session flashcards based on the StudySession and the cards.

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

    //Post for Study Session
    public async Task SaveStudySessionAsync(StudySession studySession)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySession", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<StudySession>(studySession, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tStudySession successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //Post for Study Session Flashcards
    public async Task SaveStudySessionFlashcardAsync(StudySessionFlashCard studySessionFlashCard)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<StudySessionFlashCard>(studySessionFlashCard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tStudySessionFlashCard successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //gets the Study Sessions
    public async Task<List<StudySession>> GetAllStudySessions()
    {
        List<StudySession> studySessions = new List<StudySession>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySession", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
               studySessions = JsonSerializer.Deserialize<List<StudySession>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return studySessions;
    }
}