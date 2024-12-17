using RTLTMPro;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class a : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_dast_col_harif;
    [SerializeField] private GameObject minicard;
    [SerializeField] private GameObject pa;
    [SerializeField] private GameObject mi;
    int dast = 0;
    public int c = 0;
    public int z;
    public List<GameObject> ls;
    int b = 0;
    GameObject barande;
    private GameObject maincard;
    bool l = true;
    void Start()
    {
        maincard = GameObject.Find("main cards ");
        StartCoroutine(ff());
    }
    IEnumerator ff()
    {
        yield return new WaitForSeconds(0.5f);
        ls.Clear();
        List<GameObject> defult_list = maincard.GetComponent<manager>().defult_ls;
        foreach (GameObject defult in defult_list)
        {
            ls.Add(defult);
        }
        ls = RotateList(ls, ls.IndexOf(maincard.GetComponent<manager>().hakem.transform.GetChild(0).GetChild(0).GetChild(0).gameObject));
    }
    public void add(int h,string s )
    {
        l = true;
        if(b==ls.Count)
        {
            b = 0;  
        }
        if(c%5==0)
        {
            z = h;
        }
        c++;
        if (c==4 &&c!=0)
        {
            StartCoroutine(xc());
        }
        if (l)
        {
            if(z!=6 && c != 4)
            {
                l = false;
                b++;
                StartCoroutine(cv());
            }
        }
    }
    static void Swap<T>(List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
    private List<GameObject> RotateList(List<GameObject> list, int win)
    {
        int count = list.Count;
        List<GameObject> rotatedList = new List<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            int newIndex = (i + win) % count;
            rotatedList.Add(list[newIndex]);
        }

        return rotatedList;
    }
    IEnumerator cv()
    {
        yield return new WaitForSeconds(0.5f);//0.7f// 2 
        if (b==-1)
        {
            b = 0;
        }
        if (ls[b].GetComponent<sorta>().is_bot)
        {
            ls[b].GetComponent<sorta>().a = true;
        }
        else
        {
            ls[b].GetComponent<sorta>().m();
        }
    }
    IEnumerator xc()
    {
        yield return new WaitForSeconds(0.5f);
        c = 0;
        b = -1;
        dast++;
        bool is_use_hokm = false;
        int hokm = maincard.GetComponent<manager>().h;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetChild(0).GetComponent<card>().Rol == hokm)
            {
                is_use_hokm = true;
                break;
            }
        }
        int up_point = 0;
        if (is_use_hokm)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<card>().Rol == hokm)
                {
                    if (transform.GetChild(i).GetChild(0).GetComponent<card>().P > up_point)
                    {
                        up_point = transform.GetChild(i).GetChild(0).GetComponent<card>().P;
                        barande = transform.GetChild(i).GetChild(0).GetComponent<card>().myparent;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<card>().Rol == z)
                {
                    if (transform.GetChild(i).GetChild(0).GetComponent<card>().P > up_point)
                    {
                        up_point = transform.GetChild(i).GetChild(0).GetComponent<card>().P;
                        barande = transform.GetChild(i).GetChild(0).GetComponent<card>().myparent;
                    }
                }
            }
        }
        int mm = (ls.IndexOf(barande) + 2) % ls.Count;
        GameObject g;
        if (!ls[mm].GetComponent<sorta>().is_bot || !barande.GetComponent<sorta>().is_bot)
        {
            g =  Instantiate(minicard, mi.transform);
            //if()
            //{
            //    g.transform.GetComponentInChildren<RTLTextMeshPro>().text = (int.Parse(mi.transform.GetChild(mi.transform.childCount).GetComponentInChildren<RTLTextMeshPro>().text) + 1).ToString();
            //}
        }
        else
        {
            g =  Instantiate(minicard, pa.transform);
        }
        g.transform.GetComponentInChildren<RTLTextMeshPro>().text = (barande.GetComponent<sorta>().Dast()+1).ToString();
        if (false)//barande.transform.childCount == 6
        {
            // Check am i hackem and what? 
            if (maincard.GetComponent<manager>().hakem != barande.transform.parent.parent.parent)
            {
                barande.GetComponent<sorta>().SetDastCol(barande.GetComponent<sorta>().DastCol() + 3);
                ls[mm].GetComponent<sorta>().SetDastCol(ls[mm].GetComponent<sorta>().DastCol() + 3);
                //add(barande, ls[mm], 3);
            }
            else
            {
                barande.GetComponent<sorta>().SetDastCol(barande.GetComponent<sorta>().DastCol() + 2);
                ls[mm].GetComponent<sorta>().SetDastCol(ls[mm].GetComponent<sorta>().DastCol() + 2);
                //add(barande, ls[mm], 2);
            }
        }
        else
        {
                barande.GetComponent<sorta>().SetDast(barande.GetComponent<sorta>().Dast() + 1);
                ls[mm].GetComponent<sorta>().SetDast(ls[mm].GetComponent<sorta>().Dast() + 1);
            //add(barande, ls[mm], 1);
        }
        // check is End Of Game ? 
        if (barande.GetComponent<sorta>().DastCol() == 7)
        {
            SceneManager.LoadScene(0);
        }
        if (barande.GetComponent<sorta>().Dast() == 7)
        {
            if (ls[mm].GetComponent<sorta>().is_bot && barande.GetComponent<sorta>().is_bot)
            {
                text_dast_col_harif.text = (int.Parse(text_dast_col_harif.text) + 1).ToString();
            }
            z = 6;
            l = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).GetComponent<card>().myparent.GetComponent<sorta>().SetDast(0);
                // clear All Dast 

            }
            barande.GetComponent<sorta>().SetDastCol(barande.GetComponent<sorta>().DastCol() + 1);
            ls[mm].GetComponent<sorta>().SetDastCol(ls[mm].GetComponent<sorta>().DastCol() + 1);
            maincard.GetComponent<manager>().clear_all();
            dast = 0;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            try
            {
                DestroyImmediate(transform.GetChild(i).GetChild(0).gameObject);
            }
            catch
            {

            }
        }
        ls = RotateList(ls, ls.IndexOf(barande));
        if(dast == 0)
        {
            for (int i = 0; i < pa.transform.childCount; i++)
            {
                Destroy(pa.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < mi.transform.childCount; i++)
            {
                Destroy(mi.transform.GetChild(i).gameObject);
            }
            // get current hakem
            GameObject current_hakem = maincard.GetComponent<manager>().hakem;
            if(current_hakem== ls[mm].transform.parent.parent.parent.gameObject || current_hakem == barande.transform.parent.parent.parent.gameObject)
            {
                maincard.GetComponent<manager>().main_pakhsh(current_hakem);
            }
            else
            {
                maincard.GetComponent<manager>().main_pakhsh(ls[(ls.IndexOf(current_hakem.GetComponentInChildren<sorta>().gameObject) + 1) % ls.Count].transform.parent.parent.parent.gameObject);
            }
            // go to defult value 
            yield return new WaitForSeconds(0.5f);
            ls.Clear();
            List<GameObject> defult_list = maincard.GetComponent<manager>().defult_ls;
            foreach (GameObject defult in defult_list)
            {
                ls.Add(defult);
            }
            b = 0;  
            ls = RotateList(ls, ls.IndexOf(maincard.GetComponent<manager>().hakem.transform.GetChild(0).GetChild(0).GetChild(0).gameObject));
        }
        else
        {
            l = false;
            b++;
            StartCoroutine(cv());
        }
    }
}
