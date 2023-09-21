using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiStudyBuddy.Models;
using Microsoft.Maui.ApplicationModel;
using System.Windows;

namespace NtcMaui.Views.SignAndCreate;

public partial class SignIn : ContentPage
{
    public SignIn()
    {
        InitializeComponent();
    }
    public void CompleteSignIn(object sender, EventArgs e)
    {
        //add method before this with other tasks that check stuff like if user exists in database and the password they typed is re-typed again correctly.
        this.ValidateUser();
    }

    public async void GoToAccountCreation(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateAccount));
    }

    //local Url test to retrieve users from localhost and check something.
    public async Task<List<User>> GetAllUsers()
    {
        List<User> users = new List<User>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<User>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return users;
    }

    public async void ValidateUser()
    {
        List<User> users = await GetAllUsers();
        foreach (User user in users)
        {
            if (user.Username == UserNameEntry.Text && user.Password == PasswordEntry.Text)
            {
                Error.Text = string.Empty;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", user }
                };
                await Shell.Current.GoToAsync(nameof(Success), navigationParameter);
            }
            else
            {
                Error.TextColor = Colors.Red;
                Error.Text = "Invalid User name or password";
            }

        }
    }
}