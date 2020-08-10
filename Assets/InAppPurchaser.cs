using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class InAppPurchaser : MonoBehaviour
{
    public Color purchasedColor;
    public IAPButton iapButton;

    public Image productImage;
    public GameObject checkMark;

    private void Awake()
    {
        ProductSetActive(!PlayerPrefs.HasKey("NoAds"));
    }
    
    public void OnPurchaseComplete(Product product)
    {
        PlayerPrefs.SetInt("NoAds", 1);

        //FindObjectOfType<DebugText>().debugText.text = $"Purchasing of product.id:{product.definition.id} completed";
        ProductSetActive(false);
    }

    public void OnPurchaseFailure(Product product, PurchaseFailureReason reason)
    {
        print($"Purchasing of product.id{product.definition.id} failed due to {reason}");
        //FindObjectOfType<DebugText>().debugText.text = $"Purchasing of product.id:{product.definition.id} failed due to {reason}";
    }

    IEnumerator IAPButtonCoroutine()
    {
        yield return new WaitForEndOfFrame();

        iapButton.enabled = false;
    }

    void ProductSetActive(bool value)
    {
        var productPurchased = !value;
        if (productPurchased)
        {
            productImage.color = purchasedColor;
            checkMark.SetActive(true);
#if UNITY_EDITOR
            StartCoroutine(IAPButtonCoroutine());
#else
            iapButton.enabled = false;
#endif
        }
        else
        {
            iapButton.enabled = true;
            checkMark.SetActive(false);
        }
    }
}
