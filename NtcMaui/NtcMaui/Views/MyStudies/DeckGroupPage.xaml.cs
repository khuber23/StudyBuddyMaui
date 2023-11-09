using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;
using NtcMaui.Views.Share;

namespace NtcMaui.Views.MyStudies;

public partial class DeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public DeckGroupPage()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        UserDeckGroups = await GetAllUserDeckGroups();
        DeckGroupListView.ItemsSource = UserDeckGroups;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private void GoToDashboardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    //button click event
    private void GoToCreateDeckGroupPage(object sender, EventArgs e)
    {

        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(CreateDeckGroupPage), navigationParameter);
    }

    //button click event
    private void GoToShareDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(ShareDeckGroupPage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    //When user clicks on an item in the listView it will take the item and send it through to the neck Page
    private void DeckGroupListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        
        if (e.SelectedItem != null)
        {
            ErrorLabel.IsVisible = false;
            UserDeckGroup = e.SelectedItem as UserDeckGroup;
            //add an if statement showing if a user has the checkbox checked for editing. 
            //if editischecked
            if (EditingCheckBox.IsChecked == true)
            {
                if (UserDeckGroup.DeckGroup.ReadOnly == true)
                {
                    ErrorLabel.Text = $"{UserDeckGroup.DeckGroup.DeckGroupName} isn't editable";
                    ErrorLabel.IsVisible = true;
                    //it's weird but this is the work around for dealing with re-selecting options for the item.
                    UserDeckGroup = null;
                    DeckGroupListView.SelectedItem = null;
                }
                else
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current DeckGroup", UserDeckGroup.DeckGroup}
                };
                    Shell.Current.GoToAsync(nameof(EditDeckGroupPage), navigationParameter);
                }
                
            }
            else
            {
                //else
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current DeckGroup", UserDeckGroup.DeckGroup}
                };
                Shell.Current.GoToAsync(nameof(BuildDeckGroupPage), navigationParameter);
            }
        }
    }

    public async Task<ObservableCollection<UserDeckGroup>> GetAllUserDeckGroups()
    {
        ObservableCollection<UserDeckGroup> deckGroups = new ObservableCollection<UserDeckGroup>();

        //originally
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/maui/deckgroup/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                deckGroups = JsonSerializer.Deserialize<ObservableCollection<UserDeckGroup>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return deckGroups;
    }


    public User LoggedInUser { get; set; }

    public UserDeckGroup UserDeckGroup { get; set; }

    public ObservableCollection<UserDeckGroup> UserDeckGroups { get; set; } 
}