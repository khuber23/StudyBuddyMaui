using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminDeckGroupPage()
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
        UserDeckGroups = await Constants.GetAllUserDeckGroups();
        DeckGroupListView.ItemsSource = UserDeckGroups;
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<UserDeckGroup> userDeckGroups = await Constants.GetAllUserDeckGroups();

        if (userDeckGroups != null)
        {
            if (e != null)
            {
                DeckGroupListView.ItemsSource = null;
                DeckGroupListView.ItemsSource = userDeckGroups.Where(udg => udg.DeckGroup.DeckGroupName.Contains(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
        }

    }

    //When user clicks on an item in the listView it will take the item and send it through to the neck Page
    private void DeckGroupListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        if (e.SelectedItem != null)
        {
            ErrorLabel.IsVisible = false;
            UserDeckGroup = e.SelectedItem as UserDeckGroup;
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current DeckGroup", UserDeckGroup.DeckGroup}
                };
                    Shell.Current.GoToAsync(nameof(AdminEditDeckGroupPage), navigationParameter);
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

    public User LoggedInUser { get; set; }

    public UserDeckGroup UserDeckGroup { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }
}