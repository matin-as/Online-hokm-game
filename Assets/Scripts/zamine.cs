using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zamine : MonoBehaviour
{
    int z=5;
    int c = 0;
    [SerializeField] a ad;
    private void OnTransformChildrenChanged()
    {
        // get Rol 
        try
        {
            ad.add(transform.GetChild(0).GetComponent<card>().Rol, transform.GetChild(0).GetComponent<card>().s);
        }
        catch
        {

        }
    }
}
