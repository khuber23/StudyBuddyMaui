using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using ApiStudyBuddy.Models;
using Microsoft.AspNetCore.Identity;

namespace NtcMaui.Views.SignAndCreate;

public partial class CreateAccount : ContentPage
{

    PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
    public CreateAccount()
    {
        InitializeComponent();
    }
    public async void CompleteCreation(object sender, EventArgs e)
    {
        bool validImage = false;
        ErrorLabel.IsVisible = false;
        //if a username already exists in the database throw up an error, else make the user like normal.
        User testUser = await Constants.GetUserByUsername(UserNameEntry.Text);
        if (testUser != null)
        {
            if (testUser.Username == UserNameEntry.Text)
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "User by this username already exists. Please choose a different username";
            }
        }
         if (testUser.UserId == 0)
        {
            {
                User user = this.MakeUser();
                if (UserImageEntry.Text.Contains(".png") || UserImageEntry.Text.Contains(".jpg") || UserImageEntry.Text.Contains(".jpeg"))
                {
                    validImage = true;
                }
                if (validImage == false)
                {
                    ErrorLabel.IsVisible = true;
                    ErrorLabel.Text = "Image path isn't valid. Make sure it is a jpg or png image please.";
                }
                else
                {
                    await Constants.SaveUserAsync(user);
                    await Shell.Current.GoToAsync(nameof(SignIn));
                }

            }
        }


    }

    public User MakeUser()
    {
        ErrorLabel.IsVisible = false;
        User user = new User();
        user.Username = UserNameEntry.Text;
        //when we implement password hash
        user.PasswordHash = passwordHasher.HashPassword(null, PasswordEntry.Text);
        user.Email = EmailEntry.Text;
        user.FirstName = FirstNameEntry.Text;
        user.LastName = LastNameEntry.Text;
        //eventually add profile picture stuff here
        user.IsAdmin = false;
        //test to make sure the image you want follows these designs for image
        user.ProfilePicture = UserImageEntry.Text;
        if (UserImageEntry.Text == string.Empty || UserImageEntry.Text == null)
        {
            if (ImagePath == string.Empty || ImagePath == null)
            {
                ImagePath = "stockprofileimage.png";
                UserImageEntry.Text = ImagePath;
            }
            else
            {
                user.ProfilePicture = UserImageEntry.Text;
            }
        }
        
        
        return user;
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
}