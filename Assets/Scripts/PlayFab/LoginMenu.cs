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
    public TMP_InputField ID;
    public TMP_InputField Password;
    public TMP_InputField UserName;

<<<<<<< Updated upstream
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
=======
    public void LoginUser()
    {
        string email = ID.text;
        string password = Password.text;
        if (IsValidEmail(email))
        {
            Login(email, password);
        }
        else
        {
            LoginUsername(password);
        }
    }

    //For Login
     void Login(string email, string password)
>>>>>>> Stashed changes
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
<<<<<<< Updated upstream
            Password = password
=======
            Password = password,
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    // Reset Password
=======
    public void LoginUsername(string password)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = UserName.text,
            Password = password,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }
    
    //Reset Password
>>>>>>> Stashed changes
    public void ResetPassword()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
<<<<<<< Updated upstream
            Email = Email.text,
            TitleId = "YOUR_PLAYFAB_TITLE_ID"
=======
            Email = ID.text,
            TitleId = "91BDE",
>>>>>>> Stashed changes
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

    //tocheckthe email
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

    public void OnGuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnGuestLoginSuccess, OnError);
    }

    void OnGuestLoginSuccess(LoginResult result)
    {
        Message.text = "Login Successful";
        SceneManager.LoadScene(1);
    }
}
