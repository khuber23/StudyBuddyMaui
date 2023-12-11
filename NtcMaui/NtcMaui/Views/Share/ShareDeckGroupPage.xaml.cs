using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Share;

public partial class ShareDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckGroupPage()
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
        DeckGroupPicker.ItemsSource = await Constants.GetAllUserDeckGroupByUserId(LoggedInUser.UserId);
    }

    //picks the deckgroups based on the user DeckGroups
    private void DeckGroupPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this deckname to eventually find a deck in the next btn.
            ErrorLabel.IsVisible = false;
            DeckGroupName = picker.Items[selectedIndex];
        }
    }

    //will take them to the next page dealing with Copying a deck to a user
    private async void CopyBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        DeckGroup selectedDeckGroup = await Constants.GetDeckGroupByDeckGroupName(DeckGroupName);
        if (selectedDeckGroup == null || selectedDeckGroup.DeckGroupId == 0)
        {
            ErrorLabel.Text = "Please Select a Deck Group from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeckGroup = selectedDeckGroup;
            string chosenShareType = "Copy";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current DeckGroup" , SelectedDeckGroup},
                {"Share Type", chosenShareType }
                };
            
            await Shell.Current.GoToAsync(nameof(ShareDeckGroupWithUserPage), navigationParameter);
        }
    }

        //will take them to the next page dealing with cloning a deck to a user
        private async void CloneBtn_Clicked(object sender, EventArgs e)
        {
        ErrorLabel.IsVisible = false;
        DeckGroup selectedDeckGroup = await Constants.GetDeckGroupByDeckGroupName(DeckGroupName);
        if (selectedDeckGroup == null || selectedDeckGroup.DeckGroupId == 0)
        {
            ErrorLabel.Text = "Please Select a Deck Group from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeckGroup = selectedDeckGroup;
            string chosenShareType = "Clone";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current DeckGroup" , SelectedDeckGroup},
                {"Share Type", chosenShareType }
                };

            await Shell.Current.GoToAsync(nameof(ShareDeckGroupWithUserPage), navigationParameter);
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


	public DeckGroup SelectedDeckGroup { get; set; }

    public string DeckGroupName { get; set; }

    public User LoggedInUser { get; set; }

}