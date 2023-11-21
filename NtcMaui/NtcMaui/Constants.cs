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
    }
}
