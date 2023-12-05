using System.Collections.ObjectModel;
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
        RecipientsListView.ItemsSource = Recipients;
        
        Users = await Constants.GetAllUsers();
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
        else
        {
            ErrorLabel.IsVisible = true;
        }
    }

    private async void AddUserBtn_Clicked(object sender, EventArgs e)
    {
        bool found = false;
        ErrorLabel.IsVisible = false;
        User selectedUser = await Constants.GetUserByUsername(UserName);
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
        await Shell.Current.GoToAsync(nameof(ShareDeckPage), navigationParameter);
    }

    private async void FinishBtn_Clicked(object sender, EventArgs e)
    {
        bool hasDeck = false;
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
                List<UserDeck> userDecks = await Constants.GetAllUserDecksById(selectedUser.UserId);
            var UserDeckMatchingSelected = userDecks.Where(ud => ud.Deck.DeckName == SelectedDeck.DeckName).FirstOrDefault();
            if (UserDeckMatchingSelected != null)
            {
                
                hasDeck = true;
                break;
            }
        }
           if (hasDeck == true)
        {
            ErrorLabel.Text = $"{ErrorUser.Username} already has this deck. Please go back and select a different deck or a different user please";
            ErrorLabel.IsVisible = true;
        }
        else
        {
                if (ShareType == "Clone")
                {
                foreach ( User selectedUser in Recipients)
                {
                    //add an if else check after test to see if the SharedUser already has a deckname similiar
                    //probably by user deck and checking to see if there is a userdeck that has a deckname to the Selected.
                    SharedUser = selectedUser;
                    Deck clonedDeck = new Deck();
                    clonedDeck.IsPublic = false;
                    clonedDeck.DeckName = SelectedDeck.DeckName;
                    clonedDeck.DeckDescription = SelectedDeck.DeckDescription;
                    clonedDeck.ReadOnly = true;
                    //then we need to post this new Deck
                    await Constants.SaveDeckAsync(clonedDeck);
                    //then we need to find the new deck...probably through indexes of finding decks by name? since there will now be multiple.
                    //then the last index is the newest one/the one that was cloned.
                    Decks = await Constants.GetAllDecks();

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
                        await Constants.SaveDeckFlashCardAsync(newDeckFlashCard);
                    }
                    //then we can use the Shared User and the clonedDeckId to the userDeck.

                    UserDeck userDeck = new UserDeck();
                    userDeck.DeckId = clonedDeck.DeckId;
                    userDeck.UserId = SharedUser.UserId;
                    await Constants.SaveUserDeckAsync(userDeck);
                }
                //then go back to DeckPage
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
            }
                
                else if (ShareType == "Copy")
                {
                foreach (User selectedUser in Recipients)
                {
                    //with copying you are giving them a direct access so then it's just a basic userdeck they need.
                    SharedUser = selectedUser;
                    UserDeck userDeck = new UserDeck();
                    userDeck.DeckId = SelectedDeck.DeckId;
                    userDeck.UserId = SharedUser.UserId;
                    await Constants.SaveUserDeckAsync(userDeck);
                }
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
                await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
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

    //this will be the User you select from the recipients List
    public User SelectedUser { get; set; }

    public List<Deck> Decks { get; set; }

    public User SharedUser { get; set; }

    public string UserName { get; set; }

    public User LoggedInUser { get; set; }
    public Deck SelectedDeck { get; set; }

    public string ShareType { get; set; }


    //this user is just used for error continuity and lets yoy know if there was an issue when trying to share something with this user.
    public User ErrorUser { get; set; }

    public List<User> Users { get; set; }

    public ObservableCollection<User> Recipients { get; set; } = new ObservableCollection<User>();


}