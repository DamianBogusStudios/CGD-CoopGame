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
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        Message.text = "Error: " + error.ErrorMessage;
    }
}
