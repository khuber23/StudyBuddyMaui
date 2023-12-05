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
        //eventually add some checks/add to database before going back to sign in.
        User user = this.MakeUser();
        await Constants.SaveUserAsync(user);
        await Shell.Current.GoToAsync(nameof(SignIn));
    }

    public User MakeUser()
    {
        User user = new User();
        user.Username = UserNameEntry.Text;
        //when we implement password hash
        user.PasswordHash = passwordHasher.HashPassword(null, PasswordEntry.Text);
        user.Email = EmailEntry.Text;
        user.FirstName = FirstNameEntry.Text;
        user.LastName = LastNameEntry.Text;
        //eventually add profile picture stuff here
        user.IsAdmin = false;
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