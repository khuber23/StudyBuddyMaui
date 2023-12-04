using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminEditDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminEditDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        List<DeckGroup> deckGroups = await Constants.GetAllDeckGroups();
        UserDeckGroups = await Constants.GetAllUserDeckGroups();
        UserDeckGroups = UserDeckGroups.Where(ud => ud.DeckGroupId == SelectedDeckGroup.DeckGroupId || ud.DeckGroup.DeckGroupName == SelectedDeckGroup.DeckGroupName).ToList();
        DeckGroupsToEdit = deckGroups.Where(d => d.DeckGroupName == SelectedDeckGroup.DeckGroupName).ToList();
        DeckGroupNameEntry.Text = SelectedDeckGroup.DeckGroupName;
        DeckGroupDescriptionEntry.Text = SelectedDeckGroup.DeckGroupDescription;
    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        foreach (DeckGroup deckGroup in DeckGroupsToEdit)
        {
            SelectedDeckGroup = deckGroup;
            SelectedDeckGroup.DeckGroupName = DeckGroupNameEntry.Text;
            SelectedDeckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
            //not worrying about the public stuff for these.

            await PutDeckGroupAsync(SelectedDeckGroup);
        }

        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(AdminDeckGroupPage), navigationParameter);
    }

    private void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = false;
        FinishDeleteBtn.IsVisible = true;
        CancelBtn.IsVisible = true;
        WarningLabel.Text = $"Warning: You are about to delete {SelectedDeckGroup.DeckGroupName}";
        WarningLabel.IsVisible = true;
    }

    private async void FinishDeleteBtn_Clicked(object sender, EventArgs e)
    {
        //get the userDeckGroups and get the ones where by the name.
        foreach (UserDeckGroup userDeckGroup in UserDeckGroups)
        {
            await DeleteDeckGroupAsync(userDeckGroup.UserId, userDeckGroup.DeckGroupId);
        }
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(AdminDeckGroupPage), navigationParameter);

    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = true;
        FinishDeleteBtn.IsVisible = false;
        CancelBtn.IsVisible = false;
        WarningLabel.IsVisible = false;
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

    //update the api to deal with endpoint for userId and DeckGroupId for deleting userDeckGroup
    public async Task DeleteDeckGroupAsync(int userId, int userDeckGroupId)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/{userId}/{userDeckGroupId}", string.Empty));

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

    /// <summary>
    /// Does a Put command on the deckgroup
    /// </summary>
    /// <param name="deckGroup">the deckgroup you are updating</param>
    /// <returns>updated deckgroup</returns>
    public async Task PutDeckGroupAsync(DeckGroup deckGroup)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup/%7Bid%7D?deckgroupid={SelectedDeckGroup.DeckGroupId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroup>(deckGroup, Constants._serializerOptions);
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

    public bool IsPublic { get; set; }


    public User LoggedInUser { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }

    //gets all the deckgroups needed to be edited (dealing with shared stuff)
    public List<DeckGroup> DeckGroupsToEdit { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }

}