
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;
using RTLTMPro;
using System.Collections;


public class sorta_online : MonoBehaviour, IPunOwnershipCallbacks
{

    private PhotonView photonview;
    [SerializeField] private GameObject end;
    [SerializeField] private GameObject bordin;
    [SerializeField] private GameObject lose;
    [SerializeField] TextMeshProUGUI text_dast;
    [SerializeField] TextMeshProUGUI text_dast_col;
    [SerializeField] TextMeshProUGUI text_dast_harif;
    private int dast = 0;
    private int dast_col = 0;
    private int dast_col_harif = 0;
    public bool a = false;
    public List<GameObject> cards;
    public List<GameObject> sortedcards;
    [SerializeField] a_online moves;
    public bool is_bot = false;
    public List<GameObject> cart_can_move;
    public List<GameObject> cart_can;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private float margin = 60f;// 10 
    Image img;
    public float arcAngle = 30f;
    public float radius = 200f;
    bool change = true;  
    void Start()
    {
        photonview  =GetComponent<PhotonView>();
        q();
    }
    private void Update()
    {
        if(a)
        {
            a = false;
            m();
        }
    }
    public void m(List<GameObject> ls =null)
    {
        if(transform.parent.parent.parent.GetComponent<PhotonView>().IsMine)
        {
            try
            {
                photonview.RPC("startTimer", RpcTarget.AllBuffered, transform.parent.parent.parent.GetComponent<PhotonView>().Owner.ActorNumber.ToString());
            }
            catch
            {
                saound x = GameObject.Find("canvas").GetComponent<saound>();
                foreach (var s in ls)
                {
                    // sorted list
                    if (x.leaves.Contains(int.Parse(s.name)))
                    {
                        photonview.RPC("startTimer", RpcTarget.AllBuffered, x.leaves[x.leaves.IndexOf(int.Parse(s.name))].ToString());
                        break;
                    }
                }
                return;
            }
            startTimer(transform.parent.parent.parent.GetComponent<PhotonView>().Owner.ActorNumber.ToString());
            cart_can_move.Clear();
            int a = GameObject.Find("main cards ").GetComponent<manager_online>().h;// hokm  
            int b = GameObject.Find("a").GetComponent<a_online>().c;// zamine  
            int z = GameObject.Find("a").GetComponent<a_online>().z;// zamine rol
            if (b == 0)
            {
                r();
                add_all();
                if (is_bot)
                {
                    randommove();
                }
                return;
            }
            // get len of hokms
            if (a == 6)
            {
                add_all();
                r();
            }
            int hokm = 0;
            int dari = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (z == transform.GetChild(i).GetComponent<card_online>().Rol)
                {
                    dari++;
                }
                if (a == transform.GetChild(i).GetComponent<card_online>().Rol)
                {
                    hokm++;
                }
            }
            if (dari != 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<card_online>().Rol == z)
                    {
                        transform.GetChild(i).GetComponent<Image>().color = Color.white;
                        cart_can_move.Add(transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                if (a == z && hokm > 0)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (z == transform.GetChild(i).GetComponent<card_online>().Rol)
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
            if (is_bot)
            {
                randommove();
                asd();// All Card Set Gray 
            }
            
        }
    }
    [PunRPC]
    public void startTimer(string NamePlayer)
    {
            print("startTimerCalled");
            print("Whith Name Player: " + NamePlayer);
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("a"))
            {
                RTLTextMeshPro te = g.transform.GetChild(1).GetComponent<RTLTextMeshPro>();
                RTLTextMeshPro ActorNumber = g.transform.GetChild(2).GetComponent<RTLTextMeshPro>();
                if (ActorNumber.text == NamePlayer)
                {
                    img = te.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>();
                    img.GetComponent<timer>().a = NamePlayer;
                    img.GetComponent<timer>().enabled = true;
                    img.enabled = true;
                }
                else
                {
                    te.transform.parent.GetChild(0).GetChild(0).GetComponent<timer>().enabled = false;
                    te.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
                }
            }
    }
    [PunRPC]
    public void stopTimer()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("a"))
        {
            RTLTextMeshPro te = g.transform.GetChild(1).GetComponent<RTLTextMeshPro>();
            te.transform.parent.GetChild(0).GetChild(0).GetComponent<timer>().enabled = false;
            te.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
        }
    }
    [PunRPC]
    public void SortCard()
    {
            cards.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                cards.Add(transform.GetChild(i).gameObject);
            }
            sortedcards = cards
                .OrderBy(g => GetCustomRank(g.GetComponent<card_online>().Rol))
                .ThenBy(g => g.GetComponent<card_online>().P)
                .ToList();
            for (int i = 0; i < sortedcards.Count; i++)
            {
                sortedcards[i].transform.SetSiblingIndex(i);
            }
    }
    void AdjustSpacing()
    {
        // گرفتن عرض پنل (container)
        RectTransform container = GetComponent<RectTransform>();
        float containerWidth = container.rect.width;
        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = -316.03f;
    }
    public IEnumerator MoveAndScale(GameObject g,string targetTo = null)
    {
        Transform targetTransform;
        if (targetTo!=null)
        {
            targetTransform = GameObject.Find("a" + targetTo)?.transform;
        }
        else
        {
            targetTransform = GameObject.Find("a" + gameObject.name)?.transform;
        }
        g.transform.parent = targetTransform;
        float speed = 5.0f;
        Vector3 targetScale = new Vector3(0.55f, 0.55f, g.transform.localScale.z);
        while (Vector3.Distance(g.transform.position, targetTransform.position) > 0.01f ||
               Vector3.Distance(g.transform.localScale, targetScale) > 0.01f)
        {
            g.transform.position = Vector3.Lerp(g.transform.position, targetTransform.position, speed * Time.deltaTime);
            g.transform.rotation = Quaternion.Lerp(g.transform.rotation, targetTransform.rotation, speed * Time.deltaTime);
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, targetScale, speed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnTransformChildrenChanged()
    {
        if (transform.childCount == 0)
        {
            change = true;
        }
        if (transform.childCount == 13)
        {
            SortCard();
            LayoutCardsInArc();
            // ACR missing players
            saound x = GameObject.Find("canvas").GetComponent<saound>();
            for (int i = 0; i < x.leaves.Count; i++)
            {
                GameObject.Find(x.leaves[i].ToString()).GetComponent<sorta_online>().SortCard();
                GameObject.Find(x.leaves[i].ToString()).GetComponent<sorta_online>().LayoutCardsInArc();
            }
        }
        if (transform.childCount == 5 && change)
        {
            SortCard();
            LayoutCardsInArc();
            // ACR missing players
            saound x = GameObject.Find("canvas").GetComponent<saound>();
            for (int i = 0; i < x.leaves.Count; i++)
            {
                GameObject.Find(x.leaves[i].ToString()).GetComponent<sorta_online>().SortCard();
                GameObject.Find(x.leaves[i].ToString()).GetComponent<sorta_online>().LayoutCardsInArc();
            }
            change = false;
        }
        if (transform.childCount == 13)
        {
            StraightenCardsImmediately();
            cards.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                cards.Add(transform.GetChild(i).gameObject);
            }
            if (transform.parent.parent.parent.GetComponent<PhotonView>().IsMine)
            {
                LayoutCardsInArc();
            }
        }
    }
    void LayoutCardsInArc()
    {
        GetComponent<HorizontalLayoutGroup>().spacing = -383.43f;
        StartCoroutine(AnimateSpacingAndRotation(GetComponent<HorizontalLayoutGroup>(), -321.27f, 1.2f)); // شروع انیمیشن
    }

    IEnumerator AnimateSpacingAndRotation(HorizontalLayoutGroup layoutGroup, float targetSpacing, float duration)
    {
        yield return new WaitForSeconds(1.5f);
        float initialSpacing = layoutGroup.spacing; // مقدار فعلی spacing
        float elapsedTime = 0f;

        int cardCount = cards.Count;
        if (cardCount == 0) yield break; // اگر هیچ کارتی وجود ندارد انیمیشن متوقف شود

        int middleIndex = cardCount / 2;
        float angleStep = arcAngle / cardCount;

        // محاسبه حداکثر چرخش بر اساس تعداد کارت‌ها
        float maxRotation = Mathf.Lerp(0, 22.5f, Mathf.InverseLerp(1, 13, cardCount));

        while (elapsedTime < duration)
        {
            // مقدار تدریجی برای انیمیشن
            float t = elapsedTime / duration;

            // تغییر تدریجی spacing
            layoutGroup.spacing = Mathf.Lerp(initialSpacing, targetSpacing, t);

            // انیمیشن چرخش کارت‌ها به صورت تدریجی
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

        // بلافاصله صاف کردن کارت‌ها
        for (int i = 0; i < cardCount; i++)
        {
            cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    [PunRPC]
    public void RPCmove(string g,string targetTo = null)
    {
        GameObject d = GameObject.Find(g);
        d.transform.parent = gameObject.transform.root;

        d.GetComponent<card_online>().playsound();
        if (!string.IsNullOrEmpty(targetTo))
        {
            StartCoroutine(MoveAndScale(d,targetTo));
        }
        else
        {
            StartCoroutine(MoveAndScale(d));
        }
        if (!d.GetComponent<card_online>().isfront)
        {
            d.GetComponent<card_online>().FlipCard();
        }
        d.GetComponent<card_online>().end = true;
        d.GetComponent<Button>().enabled = false;
        d.GetComponent<Image>().color = Color.white;
    }
    [PunRPC]
    private void  RPCmoveAdmin(string Target)
    {
        GameObject target = GameObject.Find(Target);
        for (int i = 0; i < target.transform.childCount; i++)
        {
            target.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
    }
    public void move(GameObject g,string TargetTo=null)
    {
        asd();
        if(TargetTo!=null)
        {
            object[] paramentrs = new object[] { g.name,TargetTo };
            photonview.RPC("RPCmove", RpcTarget.AllBuffered, paramentrs);
        }
        else
        {
            object[] paramentrs = new object[] { g.name, "" };
            photonview.RPC("RPCmove", RpcTarget.AllBuffered, paramentrs);
        }
    }
    [PunRPC]
    public void q()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.gray;
            //transform.GetChild(i).GetComponent<Button>().enabled = false;
        }
    }
    public void r()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.white;
            //transform.GetChild(i).GetComponent<Button>().enabled = true;
        }
    }
    public void add_all()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            cart_can_move.Add(transform.GetChild(i).gameObject);
        }
    }
    [PunRPC]
    public void randommove()
    {
        if(GameObject.Find("a" + gameObject.name).transform.childCount==0)
        {
            int rand = random.Range(0, cart_can_move.Count);
            // set parent ... 
            cart_can_move[rand].GetComponent<card_online>().myparent = gameObject;
            cart_can_move[rand].GetComponent<card_online>().s = gameObject.name;
            move(cart_can_move[rand]);
            q();
        }
    }
    [PunRPC]
    private void randommoveAdmin(string t)
    {
        cart_can.Clear();
        GameObject target =  GameObject.Find(t);
        if (GameObject.Find("a" + target.name).transform.childCount == 0)
        {
            int a = GameObject.Find("main cards ").GetComponent<manager_online>().h;// hokm  
            int b = GameObject.Find("a").GetComponent<a_online>().c;// zamine  
            int z = GameObject.Find("a").GetComponent<a_online>().z;// zamine rol
            if (b == 0)
            {
                r();
                //add_all();
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    cart_can.Add(target.transform.GetChild(i).gameObject);
                }
            }
            // get len of hokms
            if (a == 6)
            {
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    cart_can.Add(target.transform.GetChild(i).gameObject);
                }
            }
            int hokm = 0;
            int dari = 0;
            for (int i = 0; i < target.transform.childCount; i++)
            {
                if (z == target.transform.GetChild(i).GetComponent<card_online>().Rol)
                {
                    dari++;
                }
                if (a == target.transform.GetChild(i).GetComponent<card_online>().Rol)
                {
                    hokm++;
                }
            }
            if (dari != 0)
            {
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    if (target.transform.GetChild(i).GetComponent<card_online>().Rol == z)
                    {
                        cart_can.Add(target.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                if (a == z && hokm > 0)
                {
                    for (int i = 0; i < target.transform.childCount; i++)
                    {
                        if (z == target.transform.GetChild(i).GetComponent<card_online>().Rol)
                        {
                            cart_can.Add(target.transform.GetChild(i).gameObject);
                        }
                    }
                }
                else
                {
                    //add_all();
                    for (int i = 0; i < target.transform.childCount; i++)
                    {
                            cart_can.Add(target.transform.GetChild(i).gameObject);
                    }
                    //r(); // All True 
                }
            }
            if (is_bot)
            {
                randommove();
                asd();// All Card Set Gray 
            }
            int rand = random.Range(0, cart_can.Count);
            // set parent ... 
            cart_can[rand].GetComponent<card_online>().myparent = target;
            cart_can[rand].GetComponent<card_online>().s = target.name;
            move(cart_can[rand],target.name);
            target.GetComponent<sorta_online>().q();
            photonview.RPC("RPCmoveAdmin", RpcTarget.AllBuffered,target.name);
        }
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

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if(targetView==photonview)
        {
            photonview = targetView;
        }
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
    public int Dast()
    {
        return dast;
    }
    public int DastCol()
    {
        return dast_col;
    }
    public int DastHarif()
    {
        return dast_col_harif;
    }
    public void SetDast(int newValue)
    {
        dast = newValue;
        text_dast.text = dast.ToString();
    }
    public void SetDastCol(int newValue)
    {
        dast_col = newValue;
        text_dast_col.text = dast_col.ToString();
        if(GetComponent<PhotonView>().IsMine && GetComponent<PhotonView>().Owner != null)
        {
            bordin.GetComponent<Animator>().enabled = false;
            bordin.GetComponent<Animator>().enabled = true;
        }
    }
    public void SetDastHarif(int newValue)
    {
        dast_col_harif = newValue;
        text_dast_harif.text = dast_col_harif.ToString();
        if(GetComponent<PhotonView>().IsMine && GetComponent<PhotonView>().Owner != null)
        {
            lose.GetComponent<Animator>().enabled = false;
            lose.GetComponent<Animator>().enabled = true;
        }
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