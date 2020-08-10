using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text debugText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
