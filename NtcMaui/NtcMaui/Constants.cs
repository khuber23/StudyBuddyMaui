using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using ApiStudyBuddy.Models;
using System.Diagnostics;

namespace NtcMaui
{
    public static class Constants
    {
        public static HttpClient _client = new HttpClient();
        public static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        { 
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        //public static string Scheme = "https"; // or http
        //public static string Port = "5001";
        //Url of our test api
        public static string TestUrl = "https://instruct.ntc.edu/teststudybuddyapi";
        
        //Prod endpoint
        //public static string TestUrl = "https://instruct.ntc.edu/studybuddyapi";

        public static string LocalApiUrl = "https://localhost:7025";

        //is going to get all of the Deck
        public static async Task<List<Deck>> GetAllDecks()
        {
            List<Deck> decks = new List<Deck>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    decks = JsonSerializer.Deserialize<List<Deck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return decks;
        }

        public static async Task<List<UserDeck>> GetAllDeckByUserId(int userId)
        {
            List<UserDeck> decks = new List<UserDeck>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck/maui/user/{userId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    decks = JsonSerializer.Deserialize<List<UserDeck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return decks;
        }

        /// <summary>
        /// gets the deck based on deck name
        /// </summary>
        /// <returns>a deck</returns>
        public static async Task<Deck> GetDeckByDeckName(string DeckName)
        {
            Deck deck = new Deck();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck/deckname/{DeckName}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deck = JsonSerializer.Deserialize<Deck>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deck;
        }

        /// <summary>
        /// gets the deck group based on deckgroup name
        /// </summary>
        /// <returns>a deck</returns>
        public static async Task<DeckGroup> GetDeckGroupByDeckGroupName(string DeckGroupName)
        {
            DeckGroup deckGroup = new DeckGroup();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup/deckgroup/{DeckGroupName}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroup = JsonSerializer.Deserialize<DeckGroup>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroup;
        }

        //is going to get all of the Deck
        public static async Task<List<DeckGroupDeck>> GetAllDeckGroupDecks()
        {
            List<DeckGroupDeck> deckGroupDecks = new List<DeckGroupDeck>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroupDecks = JsonSerializer.Deserialize<List<DeckGroupDeck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroupDecks;
        }

        //is going to get all of the Deck
        public static async Task<List<DeckGroupDeck>> GetDeckGroupDecksByDeckGroupId(int deckGroupId)
        {
            List<DeckGroupDeck> deckGroupDecks = new List<DeckGroupDeck>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck/maui/{deckGroupId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroupDecks = JsonSerializer.Deserialize<List<DeckGroupDeck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroupDecks;
        }

        //is going to get all of the Deck Groups
        public static async Task<List<DeckGroup>> GetAllDeckGroups()
        {
            List<DeckGroup> deckGroups = new List<DeckGroup>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroups = JsonSerializer.Deserialize<List<DeckGroup>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroups;
        }

        //is going to get all of the Deck Groups
        public static async Task<List<DeckFlashCard>> GetAllDeckFlashCards()
        {
            List<DeckFlashCard> deckFlashCards = new List<DeckFlashCard>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckFlashCard", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckFlashCards = JsonSerializer.Deserialize<List<DeckFlashCard>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckFlashCards;
        }

        //is going to get all of the Deck
        public static async Task<DeckGroupDeck> GetSpecificDeckGroupDeck(int deckGroupId, int deckId)
        {
            DeckGroupDeck deckGroupDeck = new DeckGroupDeck();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck/maui/specificdeckgroupdeck/{deckGroupId}/{deckId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroupDeck = JsonSerializer.Deserialize<DeckGroupDeck>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroupDeck;
        }

        public static async Task<List<FlashCard>> GetAllFlashCards()
        {
            List<FlashCard> flashCards = new List<FlashCard>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Flashcard", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    flashCards = JsonSerializer.Deserialize<List<FlashCard>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return flashCards;
        }

        //will get the list of all StudySessionFlashcards
        public static async Task<List<StudySessionFlashCard>> GetAllStudySessionFlashCards(int userId)
        {
            List<StudySessionFlashCard> flashcards = new List<StudySessionFlashCard>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/maui/full/{userId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    flashcards = JsonSerializer.Deserialize<List<StudySessionFlashCard>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return flashcards;
        }

        //is going to get all of the Deck
        public static async Task<List<UserDeck>> GetAllUserDecks()
        {
            List<UserDeck> userDecks = new List<UserDeck>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    userDecks = JsonSerializer.Deserialize<List<UserDeck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return userDecks;
        }

        public static async Task<List<UserDeck>> GetAllUserDecksById(int userId)
        {
            List<UserDeck> decks = new List<UserDeck>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck/maui/user/{userId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    decks = JsonSerializer.Deserialize<List<UserDeck>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return decks;
        }

        //pass in the UserName from the DeckPicker here to get specific user by username.
        public static async Task<User> GetUserByUsername(string name)
        {
            User user = new User();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User/MVC/User?username={name}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<User>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return user;
        }

        public static async Task<List<UserDeckGroup>> GetAllUserDeckGroups()
        {
            List<UserDeckGroup> deckGroups = new List<UserDeckGroup>();


            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroups = JsonSerializer.Deserialize<List<UserDeckGroup>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroups;
        }

        public static async Task<List<UserDeckGroup>> GetAllUserDeckGroupByUserId(int userId)
        {
            List<UserDeckGroup> deckGroups = new List<UserDeckGroup>();

            //originally
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/maui/deckgroup/{userId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroups = JsonSerializer.Deserialize<List<UserDeckGroup>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return deckGroups;
        }

        public static async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    users = JsonSerializer.Deserialize<List<User>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return users;
        }

        public static async Task DeleteUser(int userId)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User/%7Bid%7D?userid={userId}", string.Empty));

            try
            {
                HttpResponseMessage response = await Constants._client.DeleteAsync(uri);
               if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\Item successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public static async Task PutUserAsync(User user)
        {
            //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User/%7Bid%7D?userid={user.UserId}", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<User>(user, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully updated.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        //posts the DeckGroupDeck
        public static async Task SaveDeckGroupDeckAsync(DeckGroupDeck deckGroupDeck)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<DeckGroupDeck>(deckGroupDeck, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tdeckGroupDeck successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }


        public static async Task SaveUserDeckGroupAsync(UserDeckGroup userDeckGroup)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<UserDeckGroup>(userDeckGroup, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }


        public static async Task SaveUserDeckAsync(UserDeck userDeck)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<UserDeck>(userDeck, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public static async Task SaveDeckAsync(Deck deck)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public static async Task SaveDeckGroupAsync(DeckGroup deckGroup)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<DeckGroup>(deckGroup, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        //Posts a Deckflashcard
        public static async Task SaveDeckFlashCardAsync(DeckFlashCard deckFlashCard)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckFlashCard", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<DeckFlashCard>(deckFlashCard, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\deckFlashCard successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }


        public static async Task SaveUserAsync(User user)
        {
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User", string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<User>(user, Constants._serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await Constants._client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tUser successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        //Post for Study Session
        public static async Task SaveStudySessionAsync(StudySession studySession)
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

        //gets the Study Sessions
        public static async Task<List<StudySession>> GetAllStudySessions()
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


        //Post for Study Session Flashcards
        public static async Task SaveStudySessionFlashcardAsync(StudySessionFlashCard studySessionFlashCard)
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

        /// <summary>
        /// Does a Put command on the StudySession
        /// </summary>
        /// <param name="studySession">the studySession you are updating</param>
        /// <returns>A task/updated studysession</returns>
        public static async Task PutStudySessionAsync(StudySession studySession)
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
        public static async Task PutStudySessionFlashCardAsync(StudySessionFlashCard studySessionFlashCard)
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

        public static async Task<List<StudySessionFlashCard>> GetIncorrectStudySessionsById(int userId)
        {
            List<StudySessionFlashCard> studySessionFlashCards = new List<StudySessionFlashCard>();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/maui/incorrect/{userId}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    studySessionFlashCards = JsonSerializer.Deserialize<List<StudySessionFlashCard>>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return studySessionFlashCards;
        }

    }
}
