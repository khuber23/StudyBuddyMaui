using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminEditUserPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminEditUserPage()
	{
		InitializeComponent();
	}


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedUser = query["Selected User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        UserNameEntry.Text = SelectedUser.Username;
        UserImageEntry.Text = SelectedUser.ProfilePicture;
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = true;
        FinishDeleteBtn.IsVisible = false;
        CancelBtn.IsVisible = false;
        WarningLabel.IsVisible = false;
    }

    private void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = false;
        FinishDeleteBtn.IsVisible = true;
        CancelBtn.IsVisible = true;
        WarningLabel.Text = $"Warning: You are about to delete {SelectedUser.Username}. Hitting finish will delete them.";
        WarningLabel.IsVisible = true;
    }

    //i think in this case if a user is deleted we need to remove the user and any user deck groups and user decks related to that user.
    //for shared stuff I am not going to worry as this can be fixed by deleting the user decks and deckgroups for other users if necessary as an admin.
    private async void FinishDeleteBtn_Clicked(object sender, EventArgs e)
    {
            await Constants.DeleteUser(SelectedUser.UserId);
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        //don't know where to take them so take them to the deck page lol
        await Shell.Current.GoToAsync(nameof(AdminUsersPage), navigationParameter);

    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        WarningLabel.IsVisible = false;
        bool validImage = false;
        //change to deal with user
        SelectedUser.Username = UserNameEntry.Text;
        SelectedUser.ProfilePicture = UserImageEntry.Text;
        if (AdminStatusCheckBox.IsChecked == true)
        {
            SelectedUser.IsAdmin = true;
        }
        else
        {
            SelectedUser.IsAdmin = false;
        }
        //check to make sure any lists are null. if they are not set them to null otherwise the update won't work.
        SelectedUser.UserDecks = null;
        SelectedUser.UserDeckGroups = null;
        if (UserImageEntry.Text.Contains(".png") || UserImageEntry.Text.Contains(".jpg") || UserImageEntry.Text.Contains(".jpeg"))
        {
            validImage = true;
        }
        if (validImage == false)
        {
            WarningLabel.IsVisible = true;
            WarningLabel.Text = "Image path isn't valid. Make sure it is a jpg or png image please.";
        }
        else
        {
            await Constants.PutUserAsync(SelectedUser);
            //then navigate back to admin user page
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
            await Shell.Current.GoToAsync(nameof(AdminUsersPage), navigationParameter);
        }

    }

    //tabs
    private void GoToHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
    }

    private void GoToAdminHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminHomePage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminFlashCardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminDeckPage), navigationParameter);

    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminDeckGroupPage), navigationParameter);
    }

    private void GoToUsersPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminUsersPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }

    public User SelectedUser { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }

    public List<UserDeck> UserDecks { get; set; }
}