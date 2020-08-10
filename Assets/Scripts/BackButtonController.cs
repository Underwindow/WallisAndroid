using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    public Button backButton;
    int tapCount = 0;

    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (backButton != null)
                {
                    var tmpHash = backButton.GetHashCode();
                    backButton.onClick.Invoke();
                    if (tmpHash == backButton.GetHashCode())
                        backButton = null;
                }
                else
                {
                    tapCount++;
                    if (tapCount == 2)
                        Application.Quit();
                    else
                    {
                        StartCoroutine(QuitCoroutine(2));
                        _ShowAndroidToastMessage("Tap again to exit the application");
                    }
                }
            }
        }
    }

    IEnumerator QuitCoroutine(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        tapCount = 0;
    }

    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    public void SetBackButton(Button newBackButton)
    {
        backButton = newBackButton;
    }
}
