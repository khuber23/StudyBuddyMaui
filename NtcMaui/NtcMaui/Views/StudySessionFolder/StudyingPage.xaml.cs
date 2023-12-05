using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using Google.Api.Gax.ResourceNames;
using Google.Apis.Translate.v2.Data;
using Google.Cloud.Translation.V2;
using System.Collections;
using System.Text.Json.Nodes;

namespace NtcMaui.Views.StudySessionFolder;

public partial class StudyingPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    int index = 0;
    public StudyingPage()
    {
        InitializeComponent();
    }

    //might also add in the SelectedUserDeckGroup.
    //might make this code/studyingpage just work for 1 userDeck. Eventually make it so they can choose to do a DeckGroup and do all decks.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        ChosenDeckGroupDeck = query["Chosen Deck"] as DeckGroupDeck;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        StartSessionTime = DateTime.Now;

       IEnumerable<Locale> locales = await TextToSpeech.Default.GetLocalesAsync();
        Locales = locales.ToList();
        LangPicker.ItemsSource = Locales;
        LanguageSelected = false;

        //need to make a list of the cards.
        FlashCards = ChosenDeckGroupDeck.Deck.DeckFlashCards;
        //need to get the flashcard at first instance. 
        //then after thsi when users swipe left or right we go to the next flashcard
        string firstquestionText = FlashCards[0].FlashCard.FlashCardQuestion;
        FlashcardText.Text = firstquestionText;
        if (FlashCards[0].FlashCard.FlashCardQuestionImage != null)
        {
            FlashCardImage.IsVisible = true;
            FlashCardImage.Source = FlashCards[0].FlashCard.FlashCardQuestionImage;
        }
        else
        {
            FlashCardImage.IsVisible = false;
            FlashCardImage.Source = null;
        }
    }

    protected override void OnDisappearing()
    {
        //need a way to check if user is going to previous page or the next page.
        if (BackButtonPressed == false && CompletedSession == false)
        {
            //due to weird error, testing something, I am going to add all the flashcards left into the InCorrectFlashCards List.
            foreach (DeckFlashCard flashcard in FlashCards)
            {
                if (!IncorrectFlashCards.Contains(flashcard.FlashCard) && !CorrectFlashCards.Contains(flashcard.FlashCard))
                {
                    IncorrectFlashCards.Add(flashcard.FlashCard);
                }

            }
            SaveStudySession();
        }
    }

    private async void SaveStudySession()
    {
        EndSessionTime = DateTime.Now;

        StudySession = new StudySession();
        StudySession.StartTime = StartSessionTime;
        StudySession.EndTime = EndSessionTime;
        StudySession.UserId = LoggedInUser.UserId;
        StudySession.DeckGroupId = ChosenDeckGroupDeck.DeckGroupId;
        StudySession.DeckId = ChosenDeckGroupDeck.DeckId;
        //might check this later but if they exit early the study Session would not be complete.
        StudySession.IsCompleted = false;
        Constants.SaveStudySessionAsync(StudySession);



        //re - get that StudySession to save the studySessionFlashCards
        //eventually also change the foreach to a Linq query for faster use.
        StudySessions = await Constants.GetAllStudySessions();

        StudySession = StudySessions.FirstOrDefault(studySession => studySession.EndTime == StudySession.EndTime
                && studySession.StartTime == StudySession.StartTime
                && studySession.UserId == StudySession.UserId
                && studySession.DeckGroupId == StudySession.DeckGroupId
                && studySession.DeckId == StudySession.DeckId);

        foreach (FlashCard flashCard in CorrectFlashCards)
        {
            if (CorrectFlashCards.Count > 0)
            {
                StudySessionFlashCard = new StudySessionFlashCard();
                StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                StudySessionFlashCard.StudySessionId = StudySession.StudySessionId;
                StudySessionFlashCard.IsCorrect = true;
                //DO NOT ADD AWAIT IN FRONT IT WILL NOT WORK OTHERWISE
                Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
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
                //DO NOT ADD AWAIT IN FRONT IT WILL NOT WORK OTHERWISE
                Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
            }
        }
    }

    //code that just is there to check if a user clicked the back button.
    protected override bool OnBackButtonPressed()
    {
        // Do something here 
        BackButtonPressed = true;
        return base.OnBackButtonPressed();
    }

    async void OnTapRecognized(object sender, TappedEventArgs args)
    {
        //old code for testing.
        if (FlashcardText.Text == FlashCards[index].FlashCard.FlashCardQuestion)
            {
                await Flashcard.RotateXTo(180, 500);
                Flashcard.RotationX = 0;
                FlashcardText.Text = FlashCards[index].FlashCard.FlashCardAnswer;
            if (FlashCards[index].FlashCard.FlashCardAnswerImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = FlashCards[index].FlashCard.FlashCardAnswerImage;
            }
            else
            {
                FlashCardImage.IsVisible = false;
                FlashCardImage.Source = null;
            }
        }
            else if (FlashcardText.Text == FlashCards[index].FlashCard.FlashCardAnswer)
            {
                await Flashcard.RotateXTo(180, 500);
                Flashcard.RotationX = 0;
                FlashcardText.Text = FlashCards[index].FlashCard.FlashCardQuestion;
            if (FlashCards[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = FlashCards[index].FlashCard.FlashCardQuestionImage;
            }
            else
            {
                FlashCardImage.IsVisible = false;
                FlashCardImage.Source = null;
            }
        }
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
            StudySession.DeckGroupId = ChosenDeckGroupDeck.DeckGroupId;
            StudySession.DeckId = ChosenDeckGroupDeck.DeckId;
            //do a check for if it is complete.
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == ChosenDeckGroupDeck.Deck.DeckFlashCards.Count)
            {
                StudySession.IsCompleted = true;
            }
            else
            {
                StudySession.IsCompleted = false;
            }

            //post the StudySession.
            await Constants.SaveStudySessionAsync(StudySession);

            //after posting you need to retrieve it to upload the study session flashcard.
            StudySessions = await Constants.GetAllStudySessions();

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
                    await Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
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
                    await Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
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
            if (FlashCards[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = FlashCards[index].FlashCard.FlashCardQuestionImage;
            }
            else
            {
                FlashCardImage.Source = null;
                FlashCardImage.IsVisible = false;
            }
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
            StudySession.DeckGroupId = ChosenDeckGroupDeck.DeckGroupId;
            StudySession.DeckId = ChosenDeckGroupDeck.DeckId;
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == ChosenDeckGroupDeck.Deck.DeckFlashCards.Count)
            {
                StudySession.IsCompleted = true;
            }
            else
            {
                StudySession.IsCompleted = false;
            }

            //post the StudySession.
            await Constants.SaveStudySessionAsync(StudySession);

            //after posting you need to retrieve it to upload the study session flashcard.
            StudySessions = await Constants.GetAllStudySessions();

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
                    await Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
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
                    await Constants.SaveStudySessionFlashcardAsync(StudySessionFlashCard);
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
            if (FlashCards[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = FlashCards[index].FlashCard.FlashCardQuestionImage;
            }
            else
            {
                FlashCardImage.Source = null;
                FlashCardImage.IsVisible = false;
            }
        }
    }

    private void LangPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            LanguageSelected = true;
            ChosenVoice = picker.Items[selectedIndex];
        }
    }

    private async void SoundButton_Clicked(object sender, EventArgs e)
    {
        //eventually use these to deal with different languages. For now I just want it to read

        if (LanguageSelected == false)
        {
            SpeechOptions options = new SpeechOptions()
            {
                //this should be a default when first loading.
                Locale = Locales.FirstOrDefault()
            };
            //eventually add a comma and options to deal with different languages and tests
            await TextToSpeech.Default.SpeakAsync(FlashcardText.Text, options);
        }
        else
        {
            SpeechOptions options = new SpeechOptions()
            {
                //this should be a default when first loading.
                Locale = Locales.FirstOrDefault(x => x.Name == ChosenVoice)
            };
            //eventually add a comma and options to deal with different languages and tests
            if (options.Locale.Language.StartsWith("es"))
            {
                //translate from english to spanish
                string TranslatedText = TranslateText(FlashcardText.Text, options.Locale.Language);
                await TextToSpeech.Default.SpeakAsync(TranslatedText, options);
            }
            else if (options.Locale.Language.StartsWith("zh"))
            {
                string TranslatedText = TranslateText(FlashcardText.Text, options.Locale.Language);
                await TextToSpeech.Default.SpeakAsync(TranslatedText, options);
            }
            else if (options.Locale.Language.StartsWith("fr"))
            {
                string TranslatedText = TranslateText(FlashcardText.Text, options.Locale.Language);
                await TextToSpeech.Default.SpeakAsync(TranslatedText, options);
            }
            else if (options.Locale.Language.StartsWith("ja"))
            {
                string TranslatedText = TranslateText(FlashcardText.Text, options.Locale.Language);
                await TextToSpeech.Default.SpeakAsync(TranslatedText, options);
            }
            else if (options.Locale.Language.StartsWith("en"))
            {
                await TextToSpeech.Default.SpeakAsync(FlashcardText.Text, options);
            }
            else
            {
                await TextToSpeech.Default.SpeakAsync(FlashcardText.Text, options);
            }

        }
    }

    public string TranslateText(string input, string language)
    {
        // Set the language from/to in the url (or pass it into this function)
        //spanish
        if (language.StartsWith("es"))
        {
            URL = String.Format
        ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
         "en", "es", Uri.EscapeUriString(input));
        }
        //chinese
        else if (language.StartsWith("zh"))
        {
            URL = String.Format
        ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
         "en", "zh", Uri.EscapeUriString(input));
        }
        //french
        else if (language.StartsWith("fr"))
        {
            URL = String.Format
      ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
       "en", "fr", Uri.EscapeUriString(input));
        }
        //japenese
        else if (language.StartsWith("ja"))
        {
            URL = String.Format
      ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
       "en", "ja", Uri.EscapeUriString(input));
        }

        HttpClient httpClient = new HttpClient();
        string result = httpClient.GetStringAsync(URL).Result;

        // Get all json data
        var jsonData = JsonSerializer.Deserialize<List<dynamic>>(result);

        // Extract just the first array element (This is the only data we are interested in)
        var translationItems = jsonData[0];

        var test = translationItems[0];

        var test2 = test[0];

        // Translation Data
        string translation = test2.ToString();

        // Return translation
        return translation;
    }

    public User LoggedInUser { get; set; }
    public DeckGroupDeck ChosenDeckGroupDeck { get; set; }
    public List<DeckFlashCard> FlashCards { get; set; }

    public DateTime StartSessionTime { get; set; }

    public DateTime EndSessionTime { get; set; }

    public List<FlashCard> CorrectFlashCards = new List<FlashCard>();

    public List<FlashCard> IncorrectFlashCards = new List<FlashCard>();

    public StudySession StudySession { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public StudySessionFlashCard StudySessionFlashCard { get; set; }

    public FlashCard CurentFlashCard { get; set; }

    //bool checking if the backbutton was pressed
    public bool BackButtonPressed { get; set; }

    //bool for checking if user is navigating to a new page (completed study session)
    public bool CompletedSession { get; set; }

    public List<Locale> Locales { get; set; }

    public bool LanguageSelected { get; set; }

    public string ChosenVoice { get; set; }

    public string URL { get; set; }

}