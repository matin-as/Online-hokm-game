using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using RTLTMPro;
using UnityEngine.UI;

#if UNITY_ANDROID
using static MyketPlugin.MyketPurchase;
using MyketPlugin;
#endif

public class MyketIABEventListener : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private GameObject context;
    [SerializeField] private TextMeshProUGUI TextCoin;
    private static readonly string m_key = "key";


    private static readonly string[] products = new List<string> { "c_6000","c_20000" , "c_80000" , "c_180000" }.ToArray();
#if UNITY_ANDROID

    void OnEnable()
	{
        MyketIAB.init(m_key);
        MyketIAB.enableLogging(false);


        IABEventManager.billingSupportedEvent += billingSupportedEvent;
		IABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		IABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		IABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent += querySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent += querySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent += queryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent += queryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		IABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		IABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		IABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}

    

    void OnDisable()
	{
		IABEventManager.billingSupportedEvent -= billingSupportedEvent;
		IABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		IABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		IABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent -= querySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent -= querySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent -= queryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent -= queryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		IABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		IABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		IABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}
    void billingSupportedEvent()
	{
        MyketIAB.queryInventory(products);
    }

	void billingNotSupportedEvent(string error)
	{
    }

	void queryInventorySucceededEvent(List<MyketPurchase> purchases, List<MyketSkuInfo> skus)
	{

    }

	void queryInventoryFailedEvent(string error)
	{
        //debug.Log("\nqueryInventoryFailedEvent: " + error);
        //debug.text += "\nqueryInventoryFailedEvent: " + error;
        //logtext.text += "queryInventoryFailedEvent: " + error;

    }

    private void querySkuDetailsSucceededEvent(List<MyketSkuInfo> skus)
    {
        //debug.Log(string.Format("querySkuDetailsSucceededEvent. total skus: {0}", skus.Count));
        for (int i = 0; i < skus.Count; ++i)
        {
        }
    }

    private void querySkuDetailsFailedEvent(string error)
    {
        //debug.Log("querySkuDetailsFailedEvent: " + error);
        //debug.text += "\nquerySkuDetailsFailedEvent: " + error;
    }

    private void queryPurchasesSucceededEvent(List<MyketPurchase> purchases)
    {
        //debug.Log(string.Format("queryPurchasesSucceededEvent. total purchases: {0}", purchases.Count));
        //debug.text += string.Format("queryPurchasesSucceededEvent. total purchases: {0}", purchases.Count);
        //logtext.text += string.Format("\nqueryPurchasesSucceededEvent. total purchases: {0}", purchases.Count);
        for (int i = 0; i < purchases.Count; ++i)
        {
            //debug.Log(purchases[i].ToString());
            //logtext.text += purchases[i].ToString();
        }
        foreach (MyketPurchase purchas in purchases)
        {

        }
    }

    private void queryPurchasesFailedEvent(string error)
    {
        //debug.Log("\nqueryPurchasesFailedEvent: " + error);
        //debug.text += "\nqueryPurchasesFailedEvent: " + error;
    }

    void purchaseSucceededEvent(MyketPurchase purchase)
	{
            Toast._ShowAndroidToastMessage(getToastMessage.Translate("Purchase completed successfully."));
            if(purchase.ProductId.StartsWith("c_"))
            {
                MyketIAB.consumeProduct(purchase.ProductId);
                SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(purchase.ProductId.Substring(2)));
                Update_Coin();
            }
    }

	void purchaseFailedEvent(string error)
	{
        Toast._ShowAndroidToastMessage(getToastMessage.Translate("Purchase canceled."));
        //debug.Log("\npurchaseFailedEvent: " + error);
        //debug.text += "\npurchaseFailedEvent: " + error;
    }

	void consumePurchaseSucceededEvent(MyketPurchase purchase)
	{
		//debug.Log("\nconsumePurchaseSucceededEvent: " + purchase);
        //debug.text += "\nconsumePurchaseSucceededEvent: " + purchase;
    }

	void consumePurchaseFailedEvent(string error)
	{
		//debug.Log("\nconsumePurchaseFailedEvent: " + error);
        //debug.text += "\nconsumePurchaseFailedEvent: " + error;

    }
    private void purches(string sku)
    {
        MyketIAB.purchaseProduct(sku);
    }
    private void Update_Coin()
    {
        TextCoin.text = SecurePlayerPrefs.GetInt("coin").ToString();
    }
#endif
    public void Buy_Prpduct(int i )
    {
        #if UNITY_ANDROID
            MyketIAB.purchaseProduct(products[i]);
        #endif
    }
}


