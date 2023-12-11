using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Edit;

public partial class EditUserPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditUserPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        SelectedUser = LoggedInUser;
        UserNameEntry.Text = SelectedUser.Username;
        UserImageEntry.Text = SelectedUser.ProfilePicture;
        FirstNameEntry.Text = SelectedUser.FirstName;
        LastNameEntry.Text = SelectedUser.LastName;
        EmailEntry.Text = SelectedUser.Email;
        if (SelectedUser.ProfilePicture == null)
        {
            ProfileImage.Source = "stockprofileimage.png";
        }
        else
        {
            ProfileImage.Source = SelectedUser.ProfilePicture;
        }
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
        LoggedInUser = null;
        //var navigationParameter = new Dictionary<string, object>
        //        {
        //            { "Current User", LoggedInUser }
        //        };
        //don't know where to take them to the sign in page upon deletion. Need to also add it so it will disable the back button. as they shouldn't be able to go back.
        await Shell.Current.GoToAsync(nameof(SignIn));

    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        bool validImage = false;
        WarningLabel.IsVisible = false;
        //change to deal with user
        SelectedUser.Username = UserNameEntry.Text;
        SelectedUser.ProfilePicture = UserImageEntry.Text;
        SelectedUser.FirstName = FirstNameEntry.Text;
        SelectedUser.LastName = LastNameEntry.Text;
        SelectedUser.Email = EmailEntry.Text;
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
            await Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
        }

    }

    private void GoToHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
    }

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}

	private void GoToDashboardPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    private async void UploadImageBtn_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images
        });

        ImagePath = result.FullPath;
        ProfileImage.Source = ImagePath;
        UserImageEntry.Text = ImagePath;
    }

    public string ImagePath { get; set; }

    public User LoggedInUser { get; set; }

    public User SelectedUser { get; set; }
}