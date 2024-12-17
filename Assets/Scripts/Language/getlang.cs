using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getlang : MonoBehaviour
{
    private void OnEnable()
    {
        if(Language.is_Persian())
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
