using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class to handle a pasword manager component in order to implement an easy and basic login system with password protection. DISCLAIMER: no protection and encryption implemented!
/// How is works:
/// <list type="number">
/// <item> No password is being typed. The button listens to events (push). When it's pressed it starts the sequence and calls onStartedTypingInvoke which starts the onStartedTyping event </item>
/// <item> The event calls the TypingStarted method which starts a coroutine, ListenToInputStream, which controls the input typed from now </item>
/// <item> The coroutine checks for the input length: when the resulting string is equal to the password in legth, it checks if the typed password is correct or not, and invoke the proper event </item>
/// <item> TypeChar is called by a registered button and add a char into the password sequence </item>
/// </list>
/// </summary>
public class PasswordManager : MonoBehaviour
{
    public string password;

    private int passwordLength;
    private string inputStreamPassword = "";
    private bool isTyping = false;
    private Coroutine stream = null;

    /// <summary>
    /// Internal event when any registered key is pressed the first time and the manager waits for the password sequence
    /// </summary>
    private UnityEvent onStartedTyping = new UnityEvent();
    [Tooltip("Event for the right password")] public UnityEvent onPasswordIsRight = new UnityEvent();
    [Tooltip("Event for the wrong password")] public UnityEvent onPasswordIsWrong = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        passwordLength = password.Length;
        onStartedTyping.AddListener(TypingStarted);
    }

    private void OnEnable()
    {
        isTyping = false;
        stream = null;
        inputStreamPassword = "";
        onStartedTyping.AddListener(TypingStarted);
    }

    /// <summary>
    /// Method called when any registered button is pressed the first time or after a full path recognition to begin listening to new incoming char sequence again.
    /// </summary>
    private void TypingStarted()
    {
        if (!isTyping)
        {
            onStartedTyping.RemoveListener(TypingStarted);
            if (stream == null)
            {
                stream = StartCoroutine(ListenToInputStream());
            }
            isTyping = true;
        }
    }

    /// <summary>
    /// This coroutine is active until a password with same length as the original password is provided. Thenk check if the password is correct, and fires the proper events
    /// </summary>
    /// <returns></returns>
    IEnumerator ListenToInputStream()
    {
        while (inputStreamPassword.Length < passwordLength)
        {
            yield return null;
        }
        isTyping = false;

        if (inputStreamPassword.Equals(password))
        {
            onPasswordIsRight.Invoke();
            Debug.LogError("RIGHT!");
        } else
        {
            onPasswordIsWrong.Invoke();
            Debug.LogError("WRONG!");
        }
        inputStreamPassword = "";
        onStartedTyping.AddListener(TypingStarted);
        stream = null;
    }
    
    /// <summary>
    /// Called by a registered button when it's pressed and adds the char to the typed password
    /// </summary>
    /// <param name="val"></param>
    public void TypeChar(string val)
    {
        if (isTyping)
        {
            inputStreamPassword = string.Concat(inputStreamPassword, val);
            Debug.LogError("Typed char: " + val + ". Current partial password: " + inputStreamPassword);
        }
    }

    public void onStartedTypingInvoke()
    {
        onStartedTyping.Invoke();
    }
}
