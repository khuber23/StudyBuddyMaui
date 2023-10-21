using ApiStudyBuddy.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace NtcMaui.Views.SignAndCreate;

public partial class ForgotPassword : ContentPage
{
    private SignIn signInPage;

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
            List<User> users = await this.signInPage.GetAllUsers();
            User userFound = users.FirstOrDefault(user => user.Username == inputUserName);
            if (userFound != null)
            {
                if (newPassword == ReEnterNewPassword && !string.IsNullOrWhiteSpace(newPassword))
                {
                    userFound.Password = newPassword;
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

    private void UpdatePassword(User user)
    {
        if (user != null)
        {
            // Just verifying that user's password has successfully changed. 
            string newPassowrd = user.Password;

        }
    }

    //public async Task<User> UpdateUser(User user)
    //{
    //    HttpClient client = new HttpClient();
    //    string path = string.Format($"{Constants.TestUrl}/api/User/{user.UserId}");
    //    Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User/{user.UserId}", string.Empty));

    //    HttpRequestMessage response = await client.pu

    //public async Task RunAsync()
    //{

    //}
}