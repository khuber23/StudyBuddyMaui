using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminUsersPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminUsersPage()
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
        Users = await Constants.GetAllUsers();
        Users = Users.Where(u => u.Username != LoggedInUser.Username).ToList();
        UsersListView.ItemsSource = Users;
    }

    private void UsersListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedUser = e.SelectedItem as User;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Selected User", SelectedUser}
                };
            Shell.Current.GoToAsync(nameof(AdminEditUserPage), navigationParameter);
        }
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<User> users = await Constants.GetAllUsers();
        users = users.Where(u => u.Username != LoggedInUser.Username).ToList();
        if (users != null)
        {
            if (e != null)
            {
                UsersListView.ItemsSource = null;
                UsersListView.ItemsSource = users.Where(u => u.Username.Contains(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
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

    public List<User> Users { get; set; }


}