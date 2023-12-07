using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using System.Collections.ObjectModel;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public CreateDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private async void GoToDeckGroupPage(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        DeckGroups = await Constants.GetAllDeckGroups();
        var foundDeckGroup = DeckGroups.FirstOrDefault(x => x.DeckGroupName == DeckGroupNameEntry.Text);
        if (foundDeckGroup != null) 
        {
            if (foundDeckGroup.DeckGroupName == DeckGroupNameEntry.Text)
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Deck Group already exists. Please choose a different name.";
            }
        }
        else
        {

      

        //in this code then might need to create a deckgroup or make a method to create a deck group and post it before moving on?
        //I'll get the method ready
        DeckGroup = new DeckGroup();
        DeckGroup.DeckGroupName = DeckGroupNameEntry.Text;
        DeckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
        DeckGroup.ReadOnly = false;
        DeckGroup.IsPublic = IsPublic;
        //alright so basically we need to save it to a deckgroup then basically assign it to the User DeckGroup after.
        await Constants.SaveDeckGroupAsync(DeckGroup);

        //get a list of the deckgroups and find the one matching the Description of current Deckgroup.
        //this allows us to reassign the DeckGroup
        List<DeckGroup> deckGroups = await Constants.GetAllDeckGroups();
        foreach (DeckGroup deckGroup in deckGroups)
        {
            if (deckGroup.DeckGroupName == DeckGroup.DeckGroupName && deckGroup.DeckGroupDescription == DeckGroup.DeckGroupDescription)
            {
                DeckGroup = deckGroup;
                break;
            }
        }
        //create userDeckGroup
        UserDeckGroup userDeckGroup = new UserDeckGroup();
        userDeckGroup.UserId = LoggedInUser.UserId;
        userDeckGroup.DeckGroupId = DeckGroup.DeckGroupId;

        //so now UserDeckGroup is at least made and if there was an issue a user can at least later go into it to view it/edit
        await Constants.SaveUserDeckGroupAsync(userDeckGroup);

        //So this method is basically going towards the Full Creation of a UserDeckGroup. So after they create one, immedietly takes them to BuildDeckGroup.
        //might eventually need a different page/folder for editing.

        //pass on the DeckGroup that they made for their UserDeckGroup to the next page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //this is the current DeckGroup that belongs to the user, easier to deal with this than the userDeckGroup itself.
                    {"Current DeckGroup", DeckGroup }
                };
       await Shell.Current.GoToAsync(nameof(BuildDeckGroupPage), navigationParameter);
        }
    }

    private void IsPublicCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            IsPublic = true;
        }
        else
        {
            IsPublic = false;
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

	private void GoToDeckGroupPageTab(object sender, EventArgs e)
	{
		//eventually make this the dashboard page and also send the user through to this page.
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
	}


	public bool IsPublic { get; set; }

    public User LoggedInUser { get; set; }

    public DeckGroup DeckGroup { get; set; }

    public List<DeckGroup> DeckGroups { get; set; }


}