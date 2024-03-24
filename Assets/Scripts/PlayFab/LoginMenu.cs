using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text Message;
    public TMP_InputField Email;
    public TMP_InputField Password;

    // Login Authentication
    public void LoginUser()
    {
        string email = Email.text;
        string password = Password.text;
        if (IsValidEmail(email))
        {
            LoginWithEmail(email, password);
        }
        else
        {
            LoginWithUsername(password);
        }
    }

    // For Login with email
    void LoginWithEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    // For Login with username
    void LoginWithUsername(string password)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = Email.text,
            Password = password
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    // On Login Success
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Success Login");
        Message.text = "Successfully Logged in";
        SceneManager.LoadScene(1);
    }

    // Reset Password
    public void ResetPassword()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = Email.text,
            TitleId = "YOUR_PLAYFAB_TITLE_ID"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    // On Password Reset Success
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Mail sent");
        Message.text = "Password sent";
    }

    // Guest Login
    public void OnGuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnGuestLoginSuccess, OnError);
    }

    // On Guest Login Success
    void OnGuestLoginSuccess(LoginResult result)
    {
        Message.text = "Guest Login Successful";
        Debug.Log("Guest Login Success");
        SceneManager.LoadScene(1);
    }

    // Check if email is valid
    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Error handler
    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        Message.text = "Error: " + error.ErrorMessage;
    }
}
