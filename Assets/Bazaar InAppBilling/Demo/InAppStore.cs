using BazaarInAppBilling;
using MyketPlugin;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RTLTMPro;

public class InAppStore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextCoin;
    private int selectedProductIndex;

    void Start()
    {
        StoreHandler.instance.InitializeBillingService(OnServiceInitializationFailed, OnServiceInitializedSuccessfully);
    }

    public void BuyProduct(int index)
    {
        selectedProductIndex = index;
        StoreHandler.instance.Purchase(index, OnPurchaseFailed, OnPurchasedSuccessfully);
    }

    public void CheckInventory(int index)
    {
        selectedProductIndex = index;
        StoreHandler.instance.CheckInventory(index, OnInventoryCheckFailed, OnInventoryHadProduct);
    }

    public void SetValidatePurchasesState(bool state)
    {
        StoreHandler.instance.validatePurchases = state;
    }

    private void OnServiceInitializedSuccessfully()
    {
        StoreHandler.instance.LoadProductPrices(OnLoadingPricesFailed, OnPricesLoadedSuccessfully);
    }

    private void OnServiceInitializationFailed(int errorCode, string message)
    {
    }
    
    private void OnPricesLoadedSuccessfully()
    {

    }

    private void OnLoadingPricesFailed(int errorCode, string message)
    {

    }

    private void OnPurchasedSuccessfully(Purchase purchase, int productIndex)
    {
        Toast._ShowAndroidToastMessage("خرید با موفقیت انجام شد");
        if (purchase.productId.StartsWith("c_"))
        {
            StoreHandler.instance.ConsumePurchase(purchase, productIndex,a,b);
            SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(purchase.productId.Substring(2)));
            Update_Coin();
        }
    }
    private void a(int a , string b )
    {
        
    }
    private void b(Purchase p , int i)
    {
        
    }
    private void OnPurchaseFailed(int errorCode, string message)
    {
        Toast._ShowAndroidToastMessage("خرید لغو شد ");
        switch (errorCode)
        {
            case StoreHandler.SERVICE_IS_NOW_READY_RETRY_OPERATION:

                BuyProduct(selectedProductIndex);

                return;
            case StoreHandler.ERROR_WRONG_SETTINGS:

                break;
            case StoreHandler.ERROR_BAZAAR_NOT_INSTALLED:

                break;
            case StoreHandler.ERROR_SERVICE_NOT_INITIALIZED:

                break;
            case StoreHandler.ERROR_INTERNAL:

                break;
            case StoreHandler.ERROR_OPERATION_CANCELLED:

                break;
            case StoreHandler.ERROR_CONSUME_PURCHASE:

                break;
            case StoreHandler.ERROR_NOT_LOGGED_IN:

                break;
            case StoreHandler.ERROR_HAS_NOT_PRODUCT_IN_INVENTORY:

                break;
            case StoreHandler.ERROR_CONNECTING_VALIDATE_API:

                break;
            case StoreHandler.ERROR_PURCHASE_IS_REFUNDED:

                break;
            case StoreHandler.ERROR_NOT_SUPPORTED_IN_EDITOR:

                break;
            case StoreHandler.ERROR_WRONG_PRODUCT_INDEX:

                break;
            case StoreHandler.ERROR_WRONG_PRODUCT_ID:

                break;
        }
    }

    private void OnInventoryHadProduct(Purchase purchase, int productIndex)
    {

    }

    private void OnInventoryCheckFailed(int errorCode, string message)
    {
        switch (errorCode)
        {
            case StoreHandler.SERVICE_IS_NOW_READY_RETRY_OPERATION:

                CheckInventory(selectedProductIndex);

                return;
            case StoreHandler.ERROR_WRONG_SETTINGS:

                break;
            case StoreHandler.ERROR_BAZAAR_NOT_INSTALLED:

                break;
            case StoreHandler.ERROR_SERVICE_NOT_INITIALIZED:

                break;
            case StoreHandler.ERROR_INTERNAL:

                break;
            case StoreHandler.ERROR_OPERATION_CANCELLED:

                break;
            case StoreHandler.ERROR_CONSUME_PURCHASE:

                break;
            case StoreHandler.ERROR_NOT_LOGGED_IN:

                break;
            case StoreHandler.ERROR_HAS_NOT_PRODUCT_IN_INVENTORY:

                break;
            case StoreHandler.ERROR_CONNECTING_VALIDATE_API:

                break;
            case StoreHandler.ERROR_PURCHASE_IS_REFUNDED:

                break;
            case StoreHandler.ERROR_NOT_SUPPORTED_IN_EDITOR:

                break;
            case StoreHandler.ERROR_WRONG_PRODUCT_INDEX:

                break;
            case StoreHandler.ERROR_WRONG_PRODUCT_ID:

                break;
        }
        
    }
    private void Update_Coin()
    {
        TextCoin.text = SecurePlayerPrefs.GetInt("coin").ToString();
    }
}