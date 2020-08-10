using UnityEngine;
using TMPro;

public class Balance : MonoBehaviour
{
    public string spriteTag;
    public int textLengthLimit;
    // Start is called before the first frame update
    void Start()
    {
        var balance = PlayerPrefs.GetInt("Balance", 0).ToString();
        balance = balance.Length > textLengthLimit
            ? balance.Remove(textLengthLimit) + "..."
            : balance;
        GetComponent<TextMeshProUGUI>().text = spriteTag + balance;
    }
}
