using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using random = UnityEngine.Random;

public class manager : MonoBehaviour
{
    public List<GameObject> defult_ls;
    [SerializeField] Image image;  
    [SerializeField] Sprite[] sprites;
    int index = 52;
    [SerializeField]List<GameObject> ls_player;
    public GameObject hakem;
    public int h = 6;
    public bool j;
    public bool p;
    void Start()
    {
        main_pakhsh();
    }
    void Update()
    {
        if(j)
        {
            j = false;
            clear_all();
        }
        if(p)
        {
            p = false;
            StartCoroutine(pakhsh(7));
        }
    }
    public void chose_hokm(int i)
    {
        image.sprite = sprites[i];
        image.color = Color.white;
        h = i;
        StartCoroutine(pakhsh(8));
        StartCoroutine(vb());
    }
    IEnumerator vb()
    {
        yield return new WaitForSeconds(2);
        hakem.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<sorta>().m();
    }
    IEnumerator pakhsh(int cardnumber)
    {
        GameObject g;
        for (int i = 0; i < cardnumber; i++)
        {
            for (int a = 0; a < ls_player.Count; a++)
            {
                // 
                transform.GetChild(0).gameObject.SetActive(true);
                g  = Instantiate(transform.GetChild(0).gameObject, ls_player[a].transform.GetChild(0).GetChild(0).GetChild(0).transform);
                g.GetComponent<card>().enabled = true;
                g.GetComponent<Image>().enabled = true;
                if (!g.transform.parent.GetComponent<sorta>().is_bot)
                {
                    g.GetComponent<card>().FlipCard();
                }
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        yield return new WaitForSeconds(2);
    }
    public void Move(GameObject g,int r)
    {
        Destroy(g);
    }
    public void main_pakhsh(GameObject Hakem = null)
    {
        clear_all();
        // 780 chield count 
        int rand = random.Range(0, ls_player.Count);
        // init All Cards 
        if (!hakem)
        {
            hakem = ls_player[rand];
        }
        else
        {
            if(Hakem)
            {
                hakem = Hakem;
            }
        }
        // لیستی از 52 فرزند اول می‌سازیم.
        List<Transform> first52Children = new List<Transform>();

        for (int i = 0; i < 52; i++)
        {
            first52Children.Add(transform.GetChild(i));
        }

        // فرزندان را به صورت تصادفی مرتب (Shuffling) می‌کنیم.
        for (int i = 0; i < first52Children.Count; i++)
        {
            int randomIndex = random.Range(i, first52Children.Count);
            Transform temp = first52Children[i];
            first52Children[i] = first52Children[randomIndex];
            first52Children[randomIndex] = temp;
        }

        // ترتیب فرزندان مرتب شده را در هیراکی اعمال می‌کنیم.
        for (int i = 0; i < first52Children.Count; i++)
        {
            first52Children[i].SetSiblingIndex(i);
        }
        hakem.GetComponent<player>().I_hakem();
        StartCoroutine(pakhsh(5));
    }
    void ShuffleChildrenRPC(int[] randomIndices)
    {
        // ایجاد لیست جدید از فرزندان به ترتیب تصادفی (فقط برای 52 کارت اول)
        List<Transform> shuffledChildren = new List<Transform>();
        for (int i = 0; i < randomIndices.Length; i++)
        {
                shuffledChildren.Add(transform.GetChild(randomIndices[i]));
        }

        // بازسازی ترتیب فقط برای 52 کارت اول
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            shuffledChildren[i].SetSiblingIndex(i);
        }
    }
    public void clear_all()
    {
        jam();
        jam();
        jam();
        jam();
        jam();
        for (int i = 0;i < transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void jam()
    {
        GameObject g;
        for (int a = 0; a < ls_player.Count; a ++)
        {
            g = ls_player[a].transform.GetChild(0).GetChild(0).GetChild(0).gameObject; // sorta 
            for (int i = 0; i < g.transform.childCount; i++)
            {
                try
                {
                    //g.transform.GetChild(i).parent = transform;
                    DestroyImmediate(g.transform.GetChild(i).gameObject);
                    //g.transform.GetChild(i).gameObject.SetActive(false);
                    //g.transform.GetChild(i).SetParent(transform);
                }
                catch
                {

                }
            }
        }
        //ls_player[0].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<sorta>().m();
    }
}
