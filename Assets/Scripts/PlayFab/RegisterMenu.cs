using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text Message;
    public TMP_InputField Email;
    public TMP_InputField Password;
    public TMP_InputField ConfirmPassword;
<<<<<<< Updated upstream
    public TMP_InputField User_name;

    //For Registration
    public void Register()
    {
        if(Password.text.Length< 6)
        {
            Message.text = "Password is too short..Should be more than 6 Letters";
            return;
        }

        if(Password != ConfirmPassword)
        {
            Message.text = "Password doesn't Match";
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = Email.text,
            Password = Password.text,
            Username = User_name.text,
            RequireBothUsernameAndEmail = false,
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    //Register Success
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registration Success");
        Message.text = "You are registered successfully and logging in";
        SceneManager.LoadScene(1);
=======
    public TMP_InputField UserName;


    public void OnRegisterEmail()
    {
        if (Password.text.Length > 6)
        {
            Message.text = "Password too short";
        }
        if(Password.text!= ConfirmPassword.text)
        {
            Message.text = "Password doesn't match";
        }
        var request = new RegisterPlayFabUserRequest()
        {
            Username = UserName.text,
            Email = Email.text,
            Password = Password.text,
            RequireBothUsernameAndEmail = false,       
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterEmailSuccess, OnError);
    }

    void OnRegisterEmailSuccess(RegisterPlayFabUserResult result)
    {
        Message.text = "Registered Successfully";
>>>>>>> Stashed changes
    }

    void OnError(PlayFabError error)
    {
<<<<<<< Updated upstream
        Debug.LogError("Error: " + error.GenerateErrorReport());
        Message.text = "Error: " + error.ErrorMessage;
=======
        Message.text = "Error"+error.GenerateErrorReport();
        Debug.Log("Error" + error.ErrorMessage);
>>>>>>> Stashed changes
    }
}
