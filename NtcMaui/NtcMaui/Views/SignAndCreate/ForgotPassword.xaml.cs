using ApiStudyBuddy.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace NtcMaui.Views.SignAndCreate;

public partial class ForgotPassword : ContentPage
{
    private SignIn signInPage;
    PasswordHasher<string> passwordHasher = new PasswordHasher<string>();

    public ForgotPassword()
    {
        InitializeComponent();
    }

    private async void VerifyBtn_Clicked(object sender, EventArgs e)
    {
        // Testing to ensure button works by navigating to the Create Account page.
        /// await Shell.Current.GoToAsync(nameof(CreateAccount));

        this.ValidateEmail();
    }

    public async void ValidateEmail()
    {
        try
        {
            signInPage = new SignIn();
            ErrorMessage.Text = string.Empty;
            ErrorMessage.TextColor = Colors.Red;

            string inputUserName = UserName.Text;
            string newPassword = ReEnterPassword.Text;
            string ReEnterNewPassword = TypeNewPassword.Text;

            if (string.IsNullOrWhiteSpace(inputUserName))
            {
                ErrorMessage.Text = "Please enter a username.";
                return;
            }

            // Using Sign In page Get all User API call method.
            List<User> users = await Constants.GetAllUsers();
            User userFound = users.FirstOrDefault(user => user.Username == inputUserName);
            if (userFound != null)
            {
                if (newPassword == ReEnterNewPassword && !string.IsNullOrWhiteSpace(newPassword))
                {
                    //fix this to deal with hashing
                    userFound.PasswordHash = passwordHasher.HashPassword(string.Empty, newPassword);
                    UpdatePassword(userFound);

                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "Current User", userFound }
                    };
                    await Shell.Current.GoToAsync(nameof(SignIn), navigationParameter);
                }
                else
                {
                    // If user's two new passwords do not match.
                    ErrorMessage.Text = "Both of your new passwords need to match.";
                }
            }
            else
            {
                // If user name does not match.
                ErrorMessage.Text = "The username does not exist.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage.Text = "Error! Invalid Username. " + ex.Message;
        }
    }

    private async void UpdatePassword(User user)
    {
        // To verify user is valid before we offically call API to update their password.
        if (user != null)
        {
            await UpdateUser(user);
        }
    }

    public async Task UpdateUser(User user)
    {
        // We are adding the {id}?userid + the user id together as a string for now until we update the API later so the code looks cleaner. 
        string userIdSet = "{id}?userid=" + user.UserId;
      
        Uri uri = new Uri($"{Constants.TestUrl}/api/User/{userIdSet}");

        try
        {
            string json = JsonSerializer.Serialize<User>(user, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(@"\tUser successfully changed.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tError {0}", ex.Message);
        }
    } 
}
