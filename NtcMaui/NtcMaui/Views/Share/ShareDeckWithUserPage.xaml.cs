using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Share;

public partial class ShareDeckWithUserPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckWithUserPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;
        ShareType = query["Share Type"] as string;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (ShareType == "Clone")
        {
            TopLabel.Text = $"Clone {SelectedDeck.DeckName} to another User.";
        }
        else
        {
            TopLabel.Text = $"Copy {SelectedDeck.DeckName} to another User.";
        }
        Users = await GetAllUsers();
        //this code will get rid of the logged in user so that they don't appear in the list when sharing.
        var userToRemove = Users.Where(user => user.Username == LoggedInUser.Username).FirstOrDefault();
        if (userToRemove != null)
        {
            Users.Remove(userToRemove);
        }

        UserPicker.ItemsSource = Users;
    }

    private void UserPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this to eventually find a user in the Finish btn.
            ErrorLabel.IsVisible = false;
            UserName = picker.Items[selectedIndex];
        }
    }

    private async void FinishBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        User selectedUser = await GetUserByUsername(UserName);
        if (selectedUser == null || selectedUser.UserId == 0)
        {
            ErrorLabel.Text = "Please Select a User from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            List<UserDeck> userDecks = await GetAllUserDecks(selectedUser.UserId);
            var UserDeckMatchingSelected = userDecks.Where(ud => ud.Deck.DeckName == SelectedDeck.DeckName).FirstOrDefault();
            if (UserDeckMatchingSelected != null)
            {
                ErrorLabel.Text = "User already has this deck. Please go back and select a different deck or a different user please";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                if (ShareType == "Clone")
                {
                    SharedUser = selectedUser;
                    //add an if else check after test to see if the SharedUser already has a deckname similiar
                    //probably by user deck and checking to see if there is a userdeck that has a deckname to the Selected.

                    Deck clonedDeck = new Deck();
                    clonedDeck.IsPublic = false;
                    //might have caused error re-test
                    //clonedDeck.DeckFlashCards = SelectedDeck.DeckFlashCards;
                    clonedDeck.DeckName = SelectedDeck.DeckName;
                    clonedDeck.DeckDescription = SelectedDeck.DeckDescription;
                    clonedDeck.ReadOnly = true;
                    //then we need to post this new Deck
                    //await SaveDeckAsync(clonedDeck);
                    //then we need to find the new deck...probably through indexes of finding decks by name? since there will now be multiple.
                    //then the last index is the newest one/the one that was cloned.
                    Decks = await GetAllDecks();

                    //multiple decks with same name test
                    List<Deck> multipleSameNameDecks = Decks.Where(d => d.DeckName.Contains(clonedDeck.DeckName)).ToList();
                    //with this code we should have the newest created deck
                    clonedDeck = multipleSameNameDecks.Last();
                    clonedDeck.DeckFlashCards = SelectedDeck.DeckFlashCards;

                    //then we can use the deck and upload deckflashcards.
                    foreach (DeckFlashCard deckFlashCard in clonedDeck.DeckFlashCards)
                    {
                        DeckFlashCard newDeckFlashCard = new DeckFlashCard();
                        newDeckFlashCard.DeckId = clonedDeck.DeckId;
                        newDeckFlashCard.FlashCardId = deckFlashCard.FlashCardId;
                        //await SaveDeckFlashCardAsync(newDeckFlashCard);
                    }
                    //then we can use the Shared User and the clonedDeckId to the userDeck.

                    UserDeck userDeck = new UserDeck();
                    userDeck.DeckId = clonedDeck.DeckId;
                    userDeck.UserId = SharedUser.UserId;
                    //await SaveUserDeckAsync(userDeck);

                    //then go back to DeckPage
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                    await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
                }
                else if (ShareType == "Copy")
                {
                    SharedUser = selectedUser;
                    //with copying you are giving them a direct access so then it's just a basic userdeck they need.
                    UserDeck userDeck = new UserDeck();
                    userDeck.DeckId = SelectedDeck.DeckId;
                    userDeck.UserId = SharedUser.UserId;
                    await SaveUserDeckAsync(userDeck);

                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                    await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
                }
            }
        }     
    }

    public async Task<List<UserDeck>> GetAllUserDecks(int userId)
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
    public async Task<User> GetUserByUsername(string name)
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

    public async Task SaveDeckAsync(Deck deck)
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

    //is going to get all of the Deck
    public async Task<List<Deck>> GetAllDecks()
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

    public async Task<List<User>> GetAllUsers()
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

    //Posts a Deckflashcard
    public async Task SaveDeckFlashCardAsync(DeckFlashCard deckFlashCard)
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

    //remember the 1 to 1 relationship later on.
    public async Task SaveUserDeckAsync(UserDeck userDeck)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        //wait to see what they want from it and explain what you are thinking/what he envisions. you could have been right in the beginning 
        //with the idea of creating new ones from there and saving them or just choosing 1 like your new idea. --past Brody
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

    public List<Deck> Decks { get; set; }

    public User SharedUser { get; set; }

    public string UserName { get; set; }

    public User LoggedInUser { get; set; }
    public Deck SelectedDeck { get; set; }

    public string ShareType { get; set; }

    public List<User> Users { get; set; }


}