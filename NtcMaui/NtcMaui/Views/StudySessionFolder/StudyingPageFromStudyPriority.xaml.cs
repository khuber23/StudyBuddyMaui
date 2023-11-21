using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

public partial class StudyingPageFromStudyPriority : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    int index = 0;

    public StudyingPageFromStudyPriority()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        ChosenStudySession = query["Study Session"] as StudySession;
        CardsToStudy = query["Cards to Study"] as List<StudySessionFlashCard>;
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

        //need to get the flashcard at first instance. 
        //then after thsi when users swipe left or right we go to the next flashcard
        string firstquestionText = CardsToStudy[0].FlashCard.FlashCardQuestion;
        FlashcardText.Text = firstquestionText;
        if (CardsToStudy[0].FlashCard.FlashCardQuestionImage != null)
        {
            FlashCardImage.IsVisible = true;
            FlashCardImage.Source = CardsToStudy[0].FlashCard.FlashCardQuestionImage;
        }
        else
        {
            FlashCardImage.IsVisible = false;
            FlashCardImage.Source = null;
        }
    }

    //code that just is there to check if a user clicked the back button.
    protected override bool OnBackButtonPressed()
    {
        // Do something here 
        BackButtonPressed = true;
        return base.OnBackButtonPressed();
    }

    protected override void OnDisappearing()
    {
        //need a way to check if user is going to previous page or the next page.
        if (BackButtonPressed == false && CompletedSession == false)
        {
            //due to weird error, testing something, I am going to add all the flashcards left into the InCorrectFlashCards List.
            foreach (StudySessionFlashCard flashcard in CardsToStudy)
            {
                if (!IncorrectFlashCards.Contains(flashcard.FlashCard) && !CorrectFlashCards.Contains(flashcard.FlashCard))
                {
                    IncorrectFlashCards.Add(flashcard.FlashCard);
                }

            }
            SaveStudySession();
        }
    }

    //code is different than other areas. in here we basically update everything
    private async void SaveStudySession()
    {
        EndSessionTime = DateTime.Now;

        EndSessionTime = DateTime.Now;

        StudySession session = new StudySession();
        //basically we need to remake the old session with the new values for putting
        session.StudySessionId = ChosenStudySession.StudySessionId;
        session.StartTime = StartSessionTime;
        session.EndTime = EndSessionTime;
        session.UserId = LoggedInUser.UserId;
        session.DeckGroupId = ChosenStudySession.DeckGroupId;
        session.DeckId = ChosenStudySession.DeckId;
        //might check this later but if they exit early the study Session would not be complete.
        session.IsCompleted = false;
        PutStudySessionAsync(session);

        foreach (FlashCard flashCard in CorrectFlashCards)
        {
            if (CorrectFlashCards.Count > 0)
            {
                StudySessionFlashCard = new StudySessionFlashCard();
                StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                StudySessionFlashCard.StudySessionId = session.StudySessionId;
                StudySessionFlashCard.IsCorrect = true;
                //DO NOT ADD AWAIT IN FRONT IT WILL NOT WORK OTHERWISE
                PutStudySessionFlashCardAsync(StudySessionFlashCard);
            }
        }



        foreach (FlashCard flashCard in IncorrectFlashCards)
        {
            if (IncorrectFlashCards.Count > 0)
            {
                StudySessionFlashCard = new StudySessionFlashCard();
                StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                StudySessionFlashCard.StudySessionId = session.StudySessionId;
                StudySessionFlashCard.IsCorrect = false;
                //DO NOT ADD AWAIT IN FRONT IT WILL NOT WORK OTHERWISE
                PutStudySessionFlashCardAsync(StudySessionFlashCard);
            }
        }
    }

    //might just end up getting rid of this for swiping since I can't really test it out with a phone emulator since it's ungodly slow...and we have buttons
    async void OnTapRecognized(object sender, TappedEventArgs args)
    {
        //old code for testing.
        if (FlashcardText.Text == CardsToStudy[index].FlashCard.FlashCardQuestion)
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = CardsToStudy[index].FlashCard.FlashCardAnswer;
            if (CardsToStudy[index].FlashCard.FlashCardAnswerImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = CardsToStudy[index].FlashCard.FlashCardAnswerImage;
            }
            else
            {
                FlashCardImage.IsVisible = false;
                FlashCardImage.Source = null;
            }
        }
        else if (FlashcardText.Text == CardsToStudy[index].FlashCard.FlashCardAnswer)
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = CardsToStudy[index].FlashCard.FlashCardQuestion;
            if (CardsToStudy[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = CardsToStudy[index].FlashCard.FlashCardQuestionImage;
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
        IncorrectFlashCards.Add(CardsToStudy[index].FlashCard);
        //when user is on the last card.
        if (index == (CardsToStudy.Count - 1))
        {
            //commenting these out as there is a current issue with task 86
            ////now we need to make the StudySession here and eventually call a post to post it

            //original code
            EndSessionTime = DateTime.Now;

            StudySession session = new StudySession();
            //basically we need to remake the old session with the new values for putting
            session.StudySessionId = ChosenStudySession.StudySessionId;
            session.StartTime = StartSessionTime;
            session.EndTime = EndSessionTime;
            session.UserId = LoggedInUser.UserId;
            session.DeckGroupId = ChosenStudySession.DeckGroupId;
            session.DeckId = ChosenStudySession.DeckId;
            //do a check for if it is complete.
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == CardsToStudy.Count)
            {
                session.IsCompleted = true;
            }
            else
            {
                session.IsCompleted = false;
            }
            //check this for later
            ChosenStudySession = session;

            //put the StudySession mostly to change the times as well as the isCompleted.
            await PutStudySessionAsync(ChosenStudySession);

           

            //update/put StudySession flashcard with the current sessionId and cardId to change the IsCorrect.

            foreach (FlashCard flashCard in CorrectFlashCards)
            {
                if (CorrectFlashCards.Count > 0)
                {
                    //instead of making new flashcard we are essentially putting/updating it.
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = ChosenStudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = true;
                    //test to see if it works
                    await PutStudySessionFlashCardAsync(StudySessionFlashCard);
                }
            }

            foreach (FlashCard flashCard in IncorrectFlashCards)
            {
                if (IncorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = ChosenStudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = false;
                    //test to see if it works
                    await PutStudySessionFlashCardAsync(StudySessionFlashCard);
                }
            }
            //store both card sets for right and wrong and continue to session stats Page.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards },
                {"Study Session", ChosenStudySession }
                };
            await Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
        }
        else
        {
            index++;
            FlashcardText.Text = CardsToStudy[index].FlashCard.FlashCardQuestion;
            if (CardsToStudy[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.IsVisible = true;
                FlashCardImage.Source = CardsToStudy[index].FlashCard.FlashCardQuestionImage;
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
        CorrectFlashCards.Add(CardsToStudy[index].FlashCard);
        //when user is on the last card.
        if (index == (CardsToStudy.Count - 1))
        {
            //commenting these out as there is a current issue with task 86
            ////now we need to make the StudySession here and eventually call a post to post it

            //original code
            EndSessionTime = DateTime.Now;

            StudySession session = new StudySession();
            //basically we need to remake the old session with the new values for putting
            session.StudySessionId = ChosenStudySession.StudySessionId;
            session.StartTime = StartSessionTime;
            session.EndTime = EndSessionTime;
            session.UserId = LoggedInUser.UserId;
            session.DeckGroupId = ChosenStudySession.DeckGroupId;
            session.DeckId = ChosenStudySession.DeckId;
            //do a check for if it is complete.
            //Basically check if The CorrectFlashcardsCount matches with how many deckflashcards count. If match then user said they understood them all
            //else they didn't understand all the flashcards and the StudySession isn't complete
            if (CorrectFlashCards.Count == CardsToStudy.Count)
            {
                session.IsCompleted = true;
            }
            else
            {
                session.IsCompleted = false;
            }
            //check this for later
            ChosenStudySession = session;
            await PutStudySessionAsync(ChosenStudySession);



            ////update/put StudySession flashcard with the current sessionId and cardId to change the IsCorrect.

            foreach (FlashCard flashCard in CorrectFlashCards)
            {
                if (CorrectFlashCards.Count > 0)
                {
                    //instead of making new flashcard we are essentially putting/updating it.
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = ChosenStudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = true;
                    //test to see if it works
                    await PutStudySessionFlashCardAsync(StudySessionFlashCard);
                }
            }

            foreach (FlashCard flashCard in IncorrectFlashCards)
            {
                if (IncorrectFlashCards.Count > 0)
                {
                    StudySessionFlashCard = new StudySessionFlashCard();
                    StudySessionFlashCard.FlashCardId = flashCard.FlashCardId;
                    StudySessionFlashCard.StudySessionId = ChosenStudySession.StudySessionId;
                    StudySessionFlashCard.IsCorrect = false;
                    //test to see if it works
                    await PutStudySessionFlashCardAsync(StudySessionFlashCard);
                }
            }
            //store both card sets for right and wrong and continue to session stats Page.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Correct Cards", CorrectFlashCards },
                {"Incorrect Cards", IncorrectFlashCards },
                {"Study Session", ChosenStudySession }
                };
            await Shell.Current.GoToAsync(nameof(SessionStatsPage), navigationParameter);
        }
        else
        {
            index++;
            FlashcardText.Text = CardsToStudy[index].FlashCard.FlashCardQuestion;
            if (CardsToStudy[index].FlashCard.FlashCardQuestionImage != null)
            {
                FlashCardImage.Source = CardsToStudy[index].FlashCard.FlashCardQuestionImage;
            }
            else
            {
                FlashCardImage.Source = null;
            }
        }

    }

    /// <summary>
    /// Does a Put command on the StudySession
    /// </summary>
    /// <param name="studySession">the studySession you are updating</param>
    /// <returns>A task/updated studysession</returns>
    public async Task PutStudySessionAsync(StudySession studySession)
    {
        //note to self. You need to have the %7Bid%7D? since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySession/%7Bid%7D?studysessionid={studySession.StudySessionId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<StudySession>(studySession, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\study session successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    /// <summary>
    /// Does a Put command on the StudySessionFlashCard
    /// </summary>
    /// <param name="studySessionFlashCard">the studySessionflashcard you are updating</param>
    /// <returns>A task/updated studysessionflashcard</returns>
    public async Task PutStudySessionFlashCardAsync(StudySessionFlashCard studySessionFlashCard)
    {
        //note to self. You need to have the %7Bid%7D? since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/%7Bid%7D?studysessionid={studySessionFlashCard.StudySessionId}&flashcardid={studySessionFlashCard.FlashCardId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<StudySessionFlashCard>(studySessionFlashCard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\studysessionflashcard successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
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

    public StudySession ChosenStudySession { get; set; }

    public List<StudySessionFlashCard> CardsToStudy { get; set; }
    public User LoggedInUser { get; set; }

    public DateTime StartSessionTime { get; set; }

    public DateTime EndSessionTime { get; set; }

    public StudySessionFlashCard StudySessionFlashCard {get; set;}

    public List<FlashCard> CorrectFlashCards = new List<FlashCard>();

    public List<FlashCard> IncorrectFlashCards = new List<FlashCard>();

    public bool BackButtonPressed { get; set; }

    //bool for checking if user is navigating to a new page (completed study session)
    public bool CompletedSession { get; set; }

    public List<Locale> Locales { get; set; }

    public bool LanguageSelected { get; set; }

    public string ChosenVoice { get; set; }

    public string URL { get; set; }
}
