using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Share;

public partial class ShareDeckGroupWithUserPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckGroupWithUserPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        ShareType = query["Share Type"] as string;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (ShareType == "Clone")
        {
            TopLabel.Text = $"Clone {SelectedDeckGroup.DeckGroupName} to another User.";
        }
        else
        {
            TopLabel.Text = $"Copy {SelectedDeckGroup.DeckGroupName} to another User.";
        }
        RecipientsListView.ItemsSource = Recipients;
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

    private async void AddUserBtn_Clicked(object sender, EventArgs e)
    {
        bool found = false;
        ErrorLabel.IsVisible = false;
        User selectedUser = await GetUserByUsername(UserName);
        if (selectedUser != null || selectedUser.UserId != 0)
        {
            if (Recipients.Count == 0)
            {
                Recipients.Add(selectedUser);
            }
            else
            {
                foreach (User user in Recipients)
                {
                    if (user.Username == selectedUser.Username)
                    {
                        ErrorUser = selectedUser;
                        ErrorLabel.Text = $"{ErrorUser.Username} already added to Recipients";
                        ErrorLabel.IsVisible = true;
                        found = true;
                        break;
                    }
                    else
                    {
                        found = false;
                    }
                }

                if (found == false)
                {
                    Recipients.Add(selectedUser);
                }
            }
        }
    }

    private void DeleteUserBtn_Clicked(object sender, EventArgs e)
    {
        foreach (User user in Recipients)
        {
            if (user.Username == SelectedUser.Username)
            {
                Recipients.Remove(user);
                SelectedUser = null;
                DeleteUserBtn.IsVisible = false;
                ErrorLabel.IsVisible = false;
                break;
            }
        }
    }

    private async void CancelBtn_Clicked(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(ShareDeckGroupPage), navigationParameter);
    }

    private async void FinishBtn_Clicked(object sender, EventArgs e)
    {
        bool hasDeckGroup = false;
        ErrorLabel.IsVisible = false;
        if (Recipients.Count == 0)
        {
            ErrorLabel.Text = "Please add a user to recipients";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            foreach (User selectedUser in Recipients)
            {
                ErrorUser = selectedUser;
                List<UserDeckGroup> userDeckGroups = await GetAllUserDeckGroups(selectedUser.UserId);
                var UserDeckGroupMatchingSelected = userDeckGroups.Where(ud => ud.DeckGroup.DeckGroupName == SelectedDeckGroup.DeckGroupName).FirstOrDefault();
                if (UserDeckGroupMatchingSelected != null)
                {

                    hasDeckGroup = true;
                    break;
                }
            }
            if (hasDeckGroup == true)
            {
                ErrorLabel.Text = $"{ErrorUser.Username} already has this deckGroup. Please go back and select a different deckGroup or a different user please";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                if (ShareType == "Clone")
                {
                    foreach (User selectedUser in Recipients)
                    {
                        SharedUser = selectedUser;

                        DeckGroup clonedDeckGroup = new DeckGroup();
                        clonedDeckGroup.IsPublic = false;

                        clonedDeckGroup.DeckGroupName = SelectedDeckGroup.DeckGroupName;
                        clonedDeckGroup.DeckGroupDescription = SelectedDeckGroup.DeckGroupDescription;
                        clonedDeckGroup.ReadOnly = true;
                        //then we need to post this new Deck
                        await SaveDeckGroupAsync(clonedDeckGroup);
                        //then we need to find the new deckGroup...probably through indexes of finding deckgroups by name? since there will now be multiple.
                        //then the last index is the newest one/the one that was cloned.
                        DeckGroups = await GetAllDeckGroups();

                        //multiple decks with same name test
                        List<DeckGroup> multipleSameNameDeckGroups = DeckGroups.Where(d => d.DeckGroupName.Contains(clonedDeckGroup.DeckGroupName)).ToList();
                        //with this code we should have the newest created deckGroup
                        clonedDeckGroup = multipleSameNameDeckGroups.Last();
                        clonedDeckGroup.DeckGroupDecks = SelectedDeckGroup.DeckGroupDecks;

                        //then we can use the DeckGrop decks to clone the decks within for a deckgroup as well as flashcards for deck
                        foreach (DeckGroupDeck deckGroupDeck in clonedDeckGroup.DeckGroupDecks)
                        {
                            //make a copy of the deck
                            Deck clonedDeck = new Deck();
                            clonedDeck.DeckName = deckGroupDeck.Deck.DeckName;
                            clonedDeck.DeckDescription = deckGroupDeck.Deck.DeckDescription;
                            clonedDeck.IsPublic = false;
                            clonedDeck.ReadOnly = true;
                            await SaveDeckAsync(clonedDeck);

                            //retrieve it
                            Decks = await GetAllDecks();

                            //multiple decks with same name test
                            List<Deck> multipleSameNameDecks = Decks.Where(d => d.DeckName.Contains(clonedDeck.DeckName)).ToList();
                            //with this code we should have the newest created deck
                            clonedDeck = multipleSameNameDecks.Last();

                            //after getting clonedDeck we need to make it a userDeck, originally forgot
                            UserDeck userDeck = new UserDeck();
                            userDeck.DeckId = clonedDeck.DeckId;
                            userDeck.UserId = SharedUser.UserId;
                            await SaveUserDeckAsync(userDeck);

                            clonedDeck.DeckFlashCards = deckGroupDeck.Deck.DeckFlashCards;
                            //then copy the flashcards within the copied deck?
                            foreach (DeckFlashCard deckFlashCard in clonedDeck.DeckFlashCards)
                            {
                                DeckFlashCard newDeckFlashCard = new DeckFlashCard();
                                newDeckFlashCard.DeckId = clonedDeck.DeckId;
                                newDeckFlashCard.FlashCardId = deckFlashCard.FlashCardId;
                                await SaveDeckFlashCardAsync(newDeckFlashCard);
                            }

                            //then save the DeckGroupDeck
                            DeckGroupDeck clonedDeckGroupDeck = new DeckGroupDeck();
                            clonedDeckGroupDeck.DeckGroupId = clonedDeckGroup.DeckGroupId;
                            clonedDeckGroupDeck.DeckId = clonedDeck.DeckId;
                            //double check to make sure that anything that isn't postable is null.
                            await SaveDeckGroupDeckAsync(clonedDeckGroupDeck);

                        }
                        //then we can use the Shared User and the clonedDeckGroupId to the userDeckGroup.

                        UserDeckGroup userDeckGroup = new UserDeckGroup();
                        userDeckGroup.DeckGroupId = clonedDeckGroup.DeckGroupId;
                        userDeckGroup.UserId = SharedUser.UserId;
                        await SaveUserDeckGroupAsync(userDeckGroup);
                    }
                    //then go back to DeckGroupPage
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "Current User", LoggedInUser }
                    };
                    await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
                }
                else if (ShareType == "Copy")
                {
                    foreach (User selectedUser in Recipients)
                    {
                        SharedUser = selectedUser;
                        //with copying you are giving them a direct access so then it's just a basic userdeck they need.
                        UserDeckGroup userDeckGroup = new UserDeckGroup();
                        userDeckGroup.DeckGroupId = SelectedDeckGroup.DeckGroupId;
                        userDeckGroup.UserId = SharedUser.UserId;
                        await SaveUserDeckGroupAsync(userDeckGroup);
                        foreach (DeckGroupDeck deckGroupDeck in SelectedDeckGroup.DeckGroupDecks)
                        {
                            UserDeck userDeck = new UserDeck();
                            userDeck.UserId = SharedUser.UserId;
                            userDeck.DeckId = deckGroupDeck.DeckId;
                            await SaveUserDeckAsync(userDeck);
                        }


                    }
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                    await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
                }
            }
        }
                
    }

    private void RecipientsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedUser = e.SelectedItem as User;
            DeleteUserBtn.IsVisible = true;
        }
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

    //Posts a Deckflashcard
    public async Task SaveDeckGroupDeckAsync(DeckGroupDeck deckGroupDeck)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroupDeck>(deckGroupDeck, Constants._serializerOptions);
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


    public async Task<List<UserDeckGroup>> GetAllUserDeckGroups(int id)
    {
        List<UserDeckGroup> deckGroups = new List<UserDeckGroup>();

        //originally
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/maui/deckgroup/{id}", string.Empty));
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

    public async Task SaveUserDeckGroupAsync(UserDeckGroup userDeckGroup)
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

    public async Task SaveUserDeckAsync(UserDeck userDeck)
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

    public async Task SaveDeckGroupAsync(DeckGroup deckGroup)
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



    public async Task<List<DeckGroup>> GetAllDeckGroups()
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
    public List<Deck> Decks { get; set; }

    public List<DeckGroup> DeckGroups { get; set; }

    public User SharedUser { get; set; }
    public string UserName { get; set; }

    public List<User> Users { get; set; }

    public User LoggedInUser { get; set; }

    //this will be the User you select from the recipients List
    public User SelectedUser { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }

    public string ShareType { get; set; }

    //this user is just used for error continuity and lets yoy know if there was an issue when trying to share something with this user.
    public User ErrorUser { get; set; }

    public ObservableCollection<User> Recipients { get; set; } = new ObservableCollection<User>();


}