
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using random = UnityEngine.Random;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;


public class sorta : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_dast;
    [SerializeField] TextMeshProUGUI text_dast_col;
    [SerializeField] TextMeshProUGUI text_dast_col_harif;
    private int dast = 0;
    private int dast_col = 0;
    private int dast_col_harif = 0;
    public bool a = false;
    public bool b = false;
    public List<GameObject> cards;
    [SerializeField] a moves;
    public bool is_bot = false;
    public List<GameObject> cart_can_move;
    public List<GameObject> sortedcards;
    public float arcAngle = 30f;
    public float radius = 200f;
    bool change = true;
    void Start()
    {
        q();
    }
    private void Update()
    {
        if(text_dast)
        {
            text_dast.text = dast.ToString();
            text_dast_col.text = dast_col.ToString();
        }
        if(a)
        {
            a = false;
            m();
        }
        if (b)
        {
            b = false;
        }
    }
    private void OnTransformChildrenChanged()
    {
        if(transform.childCount==0)
        {
            change = true;
        }
        if (transform.childCount == 13)
        {
            SortCard();
            LayoutCardsInArc();
        }
        if(transform.childCount == 5&& change)
        {
            SortCard();
            LayoutCardsInArc();
            change = false;
        }
        if (transform.childCount==13)
        {
            StraightenCardsImmediately();
            cards.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                cards.Add(transform.GetChild(i).gameObject);
            }
            LayoutCardsInArc();
        }
    }
    void LayoutCardsInArc()
    {
        float temp = GetComponent<HorizontalLayoutGroup>().spacing;
        float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

        GetComponent<HorizontalLayoutGroup>().spacing = -324.1f;
        StartCoroutine(AnimateSpacingAndRotation(GetComponent<HorizontalLayoutGroup>(), -268.01f, 1.2f)); // شروع انیمیشن
    }

    IEnumerator AnimateSpacingAndRotation(HorizontalLayoutGroup layoutGroup, float targetSpacing, float duration)
    {
        yield return new WaitForSeconds(1.5f);
        float initialSpacing = layoutGroup.spacing;
        float elapsedTime = 0f;

        int cardCount = cards.Count;
        if (cardCount == 0) yield break;

        int middleIndex = cardCount / 2;
        float angleStep = arcAngle / cardCount;

        float maxRotation = Mathf.Lerp(0, 22.5f, Mathf.InverseLerp(1, 13, cardCount));

        while (elapsedTime < duration)
        {

            float t = elapsedTime / duration;


            layoutGroup.spacing = Mathf.Lerp(initialSpacing, targetSpacing, t);


            for (int i = 0; i < cardCount; i++)
            {
                float angle = 0f;
                float rotationFactor = 0f;

                if (i < middleIndex)
                {
                    // کارت‌های سمت چپ
                    angle = angleStep * (middleIndex - i);
                    rotationFactor = (float)(middleIndex - i) / middleIndex;
                    cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, maxRotation, t * rotationFactor));
                }
                else if (i > middleIndex)
                {
                    // کارت‌های سمت راست
                    angle = -angleStep * (i - middleIndex);
                    rotationFactor = (float)(i - middleIndex) / (cardCount - middleIndex - 1);
                    cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -maxRotation, t * rotationFactor));
                }

                // محاسبه مکان کارت بر اساس زاویه (در صورت نیاز به تغییر همزمان موقعیت)
                float radians = angle * Mathf.Deg2Rad;
                Vector3 cardPosition = new Vector3(Mathf.Sin(radians) * radius, -Mathf.Cos(radians) * radius, 0f);
                cards[i].GetComponent<RectTransform>().anchoredPosition = cardPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // انیمیشن به پایان رسیده، مطمئن شوید spacing به مقدار هدف رسیده است
        layoutGroup.spacing = targetSpacing;

        // اطمینان از تنظیم نهایی چرخش کارت‌ها
        fix();
    }
    private void fix()
    {
        int cardCount = cards.Count;
        if (cardCount == 0) return;

        int middleIndex = cardCount / 2;
        float angleStep = arcAngle / cardCount;

        // محاسبه حداکثر چرخش بر اساس تعداد کارت‌ها
        float maxRotation = Mathf.Lerp(0, 22.5f, Mathf.InverseLerp(1, 13, cardCount)); // حداکثر مقدار چرخش بر اساس تعداد کارت‌ها

        for (int i = 0; i < cardCount; i++)
        {
            float angle = 0f;

            // محاسبه زاویه و موقعیت کارت
            if (i < middleIndex)
            {
                // کارت‌های سمت چپ
                angle = angleStep * (middleIndex - i);
                float rotationFactor = (float)(middleIndex - i) / middleIndex; // محاسبه درصد فاصله از مرکز
                cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, maxRotation, rotationFactor));
            }
            else if (i > middleIndex)
            {
                // کارت‌های سمت راست
                angle = -angleStep * (i - middleIndex);
                float rotationFactor = (float)(i - middleIndex) / (cardCount - middleIndex - 1); // محاسبه درصد فاصله از مرکز
                cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -maxRotation, rotationFactor));
            }

            // محاسبه مکان کارت بر اساس زاویه
            float radians = angle * Mathf.Deg2Rad;
            Vector3 cardPosition = new Vector3(Mathf.Sin(radians) * radius, -Mathf.Cos(radians) * radius, 0f);
            cards[i].GetComponent<RectTransform>().anchoredPosition = cardPosition;

        }
    }
    private void StraightenCardsImmediately()
    {
        int cardCount = cards.Count;
        if (cardCount == 0) return;

        // بلافاصله صاف کردن کارت‌ها
        for (int i = 0; i < cardCount; i++)
        {
            cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            cards[i].GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        }
    }
    public void SortCard()
    {
        cards.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            cards.Add(transform.GetChild(i).gameObject);
        }
        sortedcards = cards
            .OrderBy(g => GetCustomRank(g.GetComponent<card>().Rol))
            .ThenBy(g => g.GetComponent<card>().P)
            .ToList();
        for (int i = 0; i < sortedcards.Count; i++)
        {
            sortedcards[i].transform.SetSiblingIndex(i);
        }
    }
    void SwapChildTransforms(Transform parent, int indexA, int indexB)
    {
        // گرفتن فرزندان
        Transform childA = parent.GetChild(indexA);
        Transform childB = parent.GetChild(indexB);

        // گرفتن اندیس‌های اصلی
        int childAIndex = childA.GetSiblingIndex();
        int childBIndex = childB.GetSiblingIndex();

        // جابه‌جایی اندیس‌ها
        childA.SetSiblingIndex(childBIndex);
        childB.SetSiblingIndex(childAIndex);
    }
    public void m()
    {
        cart_can_move.Clear();
        int a = GameObject.Find("main cards ").GetComponent<manager>().h;// hokm  
        int b = GameObject.Find("a").GetComponent<a>().c;// zamine  
        int z = GameObject.Find("a").GetComponent<a>().z;// zamine Rol
        if (b==0)
        {
            r();
            add_all();
            if(is_bot)
            {
                randommove();
            }
            return;
        }
        // get len of hokms
        if (a==6)
        {
            add_all();
            r();
        }
        int hokm = 0;
        int dari = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (z == transform.GetChild(i).GetComponent<card>().Rol)
            {
                dari++;
            }
            if(a == transform.GetChild(i).GetComponent<card>().Rol)
            {
                hokm++;
            }
        }
        if(dari!=0)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).GetComponent<card>().Rol==z)
                {
                    transform.GetChild(i).GetComponent<Image>().color = Color.white;
                    cart_can_move.Add(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            if(a==z&&hokm>0)
            {
                for (int i = 0;i < transform.childCount; i++)
                {
                    if(z==transform.GetChild(i).GetComponent<card>().Rol)
                    {
                        transform.GetChild(i).GetComponent<Image>().color = Color.white;
                        cart_can_move.Add(transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                    }
                }
            }
            else
            {
                add_all();
                r(); // All True 
            }
        }
        if(is_bot)
        {
            randommove();
            asd();// All Card Set Gray 
        }
    }
    public void move(GameObject g)
    {
        g.transform.parent = gameObject.transform.root;
        StartCoroutine(MoveAndScale(g));
        if(!g.GetComponent<card>().isfront)
        {
            g.GetComponent<card>().FlipCard();
        }
        g.transform.GetComponent<card>().end = true;
        g.transform.GetComponent<Button>().enabled = false;
    }
    public IEnumerator MoveAndScale(GameObject g)
    {
        Transform targetTransform = GameObject.Find("a" + gameObject.name)?.transform;
        float speed = 7.0f;
        Vector3 targetScale = new Vector3(0.55f, 0.55f, g.transform.localScale.z);
        while (Vector3.Distance(g.transform.position, targetTransform.position) > 0.01f ||
               Vector3.Distance(g.transform.localScale, targetScale) > 0.01f)
        {
            g.transform.position = Vector3.Lerp(g.transform.position, targetTransform.position, speed * Time.deltaTime);
            g.transform.rotation = Quaternion.Lerp(g.transform.rotation, targetTransform.rotation, speed * Time.deltaTime);
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, targetScale, speed * Time.deltaTime);
            yield return null;
        }
        g.transform.parent = targetTransform;
    }
    public void q()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
    }
    public void r()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }
    public void add_all()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            cart_can_move.Add(transform.GetChild(i).gameObject);
        }
    }
    public void randommove()
    {
        int rand = random.Range(0,cart_can_move.Count);
        cart_can_move[rand].GetComponent<card>().myparent = gameObject;
        cart_can_move[rand].GetComponent<card>().s = gameObject.name;
        cart_can_move[rand].GetComponent<card>().playsound();
        move(cart_can_move[rand]);
        q();
    }
    private void asd()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
    }
    public void clear(GameObject g)
    {
        cart_can_move.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).parent = g.transform;
        }
    }
    public int Dast()
    {
        return dast;
    }
    public int DastCol()
    {
        return dast_col;
    }
    public int Dast_col_harif()
    {
        return dast_col_harif;
    }
    public void SetDastColHarif(int newValue)
    {
        dast_col_harif = newValue;
        text_dast_col_harif.text = dast_col_harif.ToString();
    }
    public void SetDast(int newValue)
    {
        dast = newValue;
    }
    public void SetDastCol(int newValue)
    {
        dast_col = newValue;
    }
    int GetCustomRank(int rol)
    {
        switch (rol)
        {
            case 0:
                return 2;
            case 1:
                return 1;
            case 2:
                return 3;
            case 3:
                return 4;
            default:
                return rol;
        }
    }
}