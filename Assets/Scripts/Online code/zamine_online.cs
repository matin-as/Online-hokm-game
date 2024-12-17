using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zamine_online : MonoBehaviour
{
    int z=5;
    int c = 0;
    [SerializeField] private a_online ad;
    private void Start()
    {
        ad = GameObject.Find("a").GetComponent<a_online>();
    }
    private void OnTransformChildrenChanged()
    {
        // get rol 
        try
        {
            ad.add(transform.GetChild(0).GetComponent<card_online>().Rol, transform.GetChild(0).GetComponent<card_online>().s);
        }
        catch
        {

        }
    }
}
