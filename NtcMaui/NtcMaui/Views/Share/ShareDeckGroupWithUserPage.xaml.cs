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
                List<UserDeckGroup> userDeckGroups = await  Constants.GetAllUserDeckGroupByUserId(selectedUser.UserId);
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
                        await Constants.SaveDeckGroupAsync(clonedDeckGroup);
                        //then we need to find the new deckGroup...probably through indexes of finding deckgroups by name? since there will now be multiple.
                        //then the last index is the newest one/the one that was cloned.
                        DeckGroups = await Constants.GetAllDeckGroups();

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
                            await Constants.SaveDeckAsync(clonedDeck);

                            //retrieve it
                            Decks = await Constants.GetAllDecks();

                            //multiple decks with same name test
                            List<Deck> multipleSameNameDecks = Decks.Where(d => d.DeckName.Contains(clonedDeck.DeckName)).ToList();
                            //with this code we should have the newest created deck
                            clonedDeck = multipleSameNameDecks.Last();

                            //after getting clonedDeck we need to make it a userDeck, originally forgot
                            UserDeck userDeck = new UserDeck();
                            userDeck.DeckId = clonedDeck.DeckId;
                            userDeck.UserId = SharedUser.UserId;
                            await Constants.SaveUserDeckAsync(userDeck);

                            clonedDeck.DeckFlashCards = deckGroupDeck.Deck.DeckFlashCards;
                            //then copy the flashcards within the copied deck?
                            foreach (DeckFlashCard deckFlashCard in clonedDeck.DeckFlashCards)
                            {
                                DeckFlashCard newDeckFlashCard = new DeckFlashCard();
                                newDeckFlashCard.DeckId = clonedDeck.DeckId;
                                newDeckFlashCard.FlashCardId = deckFlashCard.FlashCardId;
                                await Constants.SaveDeckFlashCardAsync(newDeckFlashCard);
                            }

                            //then save the DeckGroupDeck
                            DeckGroupDeck clonedDeckGroupDeck = new DeckGroupDeck();
                            clonedDeckGroupDeck.DeckGroupId = clonedDeckGroup.DeckGroupId;
                            clonedDeckGroupDeck.DeckId = clonedDeck.DeckId;
                            //double check to make sure that anything that isn't postable is null.
                            await Constants.SaveDeckGroupDeckAsync(clonedDeckGroupDeck);

                        }
                        //then we can use the Shared User and the clonedDeckGroupId to the userDeckGroup.

                        UserDeckGroup userDeckGroup = new UserDeckGroup();
                        userDeckGroup.DeckGroupId = clonedDeckGroup.DeckGroupId;
                        userDeckGroup.UserId = SharedUser.UserId;
                        await Constants.SaveUserDeckGroupAsync(userDeckGroup);
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
                        await Constants.SaveUserDeckGroupAsync(userDeckGroup);
                        foreach (DeckGroupDeck deckGroupDeck in SelectedDeckGroup.DeckGroupDecks)
                        {
                            UserDeck userDeck = new UserDeck();
                            userDeck.UserId = SharedUser.UserId;
                            userDeck.DeckId = deckGroupDeck.DeckId;
                            await Constants.SaveUserDeckAsync(userDeck);
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