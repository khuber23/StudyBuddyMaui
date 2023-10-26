using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

public partial class StudyingPageNoDeckGroups : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    int index = 0;

    public StudyingPageNoDeckGroups()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        ChosenUserDeck = query["Chosen Deck"] as UserDeck;
        OnPropertyChanged("Current User");
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

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        StartSessionTime = DateTime.Now;

        //need to make a list of the cards.
        FlashCards = ChosenUserDeck.Deck.DeckFlashCards;
        //need to get the flashcard at first instance. 
        //then after thsi when users swipe left or right we go to the next flashcard
        string firstquestionText = FlashCards[0].FlashCard.FlashCardQuestion;
        FlashcardText.Text = firstquestionText;
    }

    private async void DontUnderstandClick(object sender, EventArgs e)
    {
        IncorrectFlashCards.Add(FlashCards[index].FlashCard);
        //when user is on the last card.
        if (index == (FlashCards.Count - 1))
        {
            //commenting these out as there is a current issue with task 86
            ////now we need to make the StudySession here and eventually call a post to post it

            //original code
            EndSessionTime = DateTime.Now;

            StudySession = new StudySession();
            StudySession.StartTime = StartSessionTime;
            StudySession.EndTime = EndSessionTime;
            StudySession.UserId = LoggedInUser.UserId;
            StudySession.DeckGroupId = null;
            StudySession.DeckId = ChosenUserDeck.DeckId;
            //do a check for if it is complete.
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == ChosenUserDeck.Deck.DeckFlashCards.Count)
            {
                StudySession.IsCompleted = true;
            }
            else
            {
                StudySession.IsCompleted = false;
            }

            //post the StudySession.
            await SaveStudySessionAsync(StudySession);

            //after posting you need to retrieve it to upload the study session flashcard.
            StudySessions = await GetAllStudySessions();

            foreach (StudySession studySession in StudySessions)
            {
                if (studySession.EndTime == StudySession.EndTime
                    && studySession.StartTime == StudySession.StartTime
                    && studySession.UserId == StudySession.UserId
                    && studySession.DeckGroupId == StudySession.DeckGroupId
                    && studySession.DeckId == StudySession.DeckId)
                {
                    //so if they are equal it will re-get the current StudySession along with it's Id from the Database.
                    StudySession = studySession;
                    break;
                }
            }


            ////then make Post for Study session flashcards based on the StudySession and the cards.
            ////for each CorrectCard/IcnorrectCard list if the list has a count greater than 1
            ////make a new StudySession flashcard with the current sessionId and cardId

            foreach (FlashCard flashCard in CorrectFlashCards)
            {
                if (CorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = StudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = true;
                    //test to see if it works
                    await SaveStudySessionFlashcardAsync(StudySessionFlashCard);
                }
            }

            foreach (FlashCard flashCard in IncorrectFlashCards)
            {
                if (IncorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = StudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = false;
                    //test to see if it works
                    await SaveStudySessionFlashcardAsync(StudySessionFlashCard);
                }
            }
            //store both card sets for right and wrong and continue to session stats Page.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards },
                {"Study Session", StudySession }
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
            //commenting these out as there is a current issue with task 86
            //now we need to make the StudySession here and eventually call a post to post it.
            EndSessionTime = DateTime.Now;

            StudySession = new StudySession();
            //this might fail since the datbase has a time of datetime2
            StudySession.StartTime = StartSessionTime;
            StudySession.EndTime = EndSessionTime;
            StudySession.UserId = LoggedInUser.UserId;
            //test to first see this will be null when uploading
            StudySession.DeckGroupId = null;
            StudySession.DeckId = ChosenUserDeck.DeckId;
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == ChosenUserDeck.Deck.DeckFlashCards.Count)
            {
                StudySession.IsCompleted = true;
            }
            else
            {
                StudySession.IsCompleted = false;
            }

            //post the StudySession.
            await SaveStudySessionAsync(StudySession);

            //after posting you need to retrieve it to upload the study session flashcard.
            StudySessions = await GetAllStudySessions();

            foreach (StudySession studySession in StudySessions)
            {
                if (studySession.EndTime == StudySession.EndTime
                    && studySession.StartTime == StudySession.StartTime
                    && studySession.UserId == StudySession.UserId
                    && studySession.DeckGroupId == StudySession.DeckGroupId
                    && studySession.DeckId == StudySession.DeckId)
                {
                    //so if they are equal it will re-get the current StudySession along with it's Id from the Database.
                    StudySession = studySession;
                    break;
                }
            }


            //then make Post for Study session flashcards based on the StudySession and the cards.
            //for each CorrectCard/IcnorrectCard list if the list has a count greater than 1
            //make a new StudySession flashcard with the current sessionId and cardId

            foreach (FlashCard flashCard in CorrectFlashCards)
            {
                if (CorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = StudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = true;
                    //Api error currently
                    await SaveStudySessionFlashcardAsync(StudySessionFlashCard);
                }
            }

            foreach (FlashCard flashCard in IncorrectFlashCards)
            {
                if (IncorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = StudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = false;
                    //Api error currently
                    await SaveStudySessionFlashcardAsync(StudySessionFlashCard);
                }
            }

            //store both card sets for right and wrong and continue to session stats Page.
            //eventually store the Session as well after method fully works.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards },
                {"Study Session", StudySession }
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


    public UserDeck ChosenUserDeck { get; set; }

    public User LoggedInUser { get; set; }

    public List<DeckFlashCard> FlashCards { get; set; }

    public DateTime StartSessionTime { get; set; }

    public DateTime EndSessionTime { get; set; }

    public List<FlashCard> CorrectFlashCards = new List<FlashCard>();

    public List<FlashCard> IncorrectFlashCards = new List<FlashCard>();

    public StudySession StudySession { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public StudySessionFlashCard StudySessionFlashCard { get; set; }

    public FlashCard CurentFlashCard { get; set; }
}
