using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sign Up Fields")]
    public TMP_InputField signUpUsernameField;
    public TMP_InputField signUpEmailField;
    public TMP_InputField signUJpPasswordField;

    [Header("Sign In Fields")]
    public TMP_InputField signInUsernameField;
    public TMP_InputField signInPasswordField;

    [Header("Auth References")]
    public AuthService authService;

    [Header("Login Scene")]
    public string gamesceneName = "Game";


    public void SignUpButton()
    {   
        string username = signUpUsernameField.text.Trim();
        string email = signUpEmailField.text.Trim();
        string password = signUJpPasswordField.text.Trim();
        StartCoroutine(authService.SignUp(username, email, password, OnSignUpCompleted));
    }
    private void OnSignUpCompleted(bool success, string responseData)
    {
        if(success)
        {
            Debug.Log("Sign up successful: " + responseData);

        }
        else{
            Debug.Log("Sign up failed: " + responseData);
        }
    }

    public void SignInButton()
    {
        string username = signInUsernameField.text.Trim();
        string password = signInPasswordField.text;

        StartCoroutine(authService.SignIn(username, password,OnSignInCompleted));
    }
    private void OnSignInCompleted(bool success, string responseData)
    {
        if(success)
        {
            AuthService.SignInResponse signResp = JsonUtility.FromJson<AuthService.SignInResponse>(responseData);
            if(!string.IsNullOrEmpty(signResp.token))
            {
                SessionManager.Instance.SetAuthToken(signResp.token);
                Debug.Log("Login successful" + signResp.token);
                
                // Load the Game scene
                SceneManager.LoadScene(gamesceneName);
            }
            else{
                Debug.Log("No token in response:" + responseData);
            }
   
        }
        else
        {
            Debug.LogError("Login Failed:" + responseData);
        }
    }
}
