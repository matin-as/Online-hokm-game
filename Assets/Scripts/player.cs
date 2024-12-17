using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class player : MonoBehaviour
{
    [SerializeField] GameObject chose_hokm;
    [SerializeField] sorta s;
    public bool is_but;
    private void Start()
    {
        Time.timeScale = 1.5f;
    }
    public void I_hakem()
    {
        if(is_but)
        {
            choseRandomHokm();
        }
        else
        {
            chose_hokm.SetActive(!chose_hokm.activeInHierarchy);
        }
    }
    private void choseRandomHokm()
    {
        GameObject.Find("main cards ").GetComponent<manager>().chose_hokm(Random.Range(0,3));
    }
    public void v()
    {
        s.m();
        //I_hakem();
    }
    public void change_model()
    {
        Toast._ShowAndroidToastMessage("انداختن کارت ها تغییر کرد");
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("mycard"))
        {
            g.GetComponent<Button>().enabled = !g.GetComponent<Button>().enabled;
        }
    }
    public void open_setting(GameObject g )
    {
        g.GetComponent<Animator>().SetBool("a", !g.GetComponent<Animator>().GetBool("a"));
    }
    public void exite()
    {
        SceneManager.LoadScene(1);
    }
}
