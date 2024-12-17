using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Photon.Pun;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using System.Collections.Generic;
using random = UnityEngine.Random;




public class card_online : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private List<AudioClip> clips;
    private PhotonView photonView;
    public GameObject myparent;
    public string s;
    [SerializeField] Sprite frontImage;
    [SerializeField] Sprite backImage;
    private Image cardImage;
    private bool isFront = false;
    [SerializeField] private int p;
    [SerializeField] private int rol;
    public bool a;
    public bool end;
    Vector3 lasttransform;
    private float cardAspectRatio = 2.5f / 3.5f;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        cardImage = GetComponent<Image>();
        cardImage.sprite = backImage;
        GetComponent<Button>().AddEventListener(0, ItemClicked);
        GetComponent<Button>().enabled = false;
        Button button = GetComponent<Button>();
        ColorBlock colorBlock = button.colors;
        colorBlock.pressedColor = Color.white;
        button.colors = colorBlock;
        GetComponent<Image>().color = Color.gray;
        gameObject.tag = "mycard";
        ScaleCardToFit();
    }
    private void Update()
    {
        if(a==true)
        {
            a = false;
            FlipCard(); 
        }
    }

    public void FlipCard()
    {
        if (isFront)
        {
            StartCoroutine(FlipToBack());
        }
        else
        {
            StartCoroutine(FlipToFront());
        }
    }
    private void ItemClicked(int t)
    {
        if (GetComponent<Image>().color == Color.white)
        {
            transform.parent.GetComponent<sorta_online>().q();
            transform.parent.GetComponent<sorta_online>().move(gameObject);
        }
    }
    [PunRPC]
    public void RPC_ItemClicked()
    {
        myparent = transform.parent.gameObject;
        s = transform.parent.name;
    }
    private IEnumerator FlipToBack()
    {
        // چرخش کارت
        for (float i = 0; i <= 1; i += Time.deltaTime * 2)
        {
            transform.localScale = new Vector3(Mathf.Lerp(1, 0, i), 1, 1);
            yield return null;
        }

        // تغییر تصویر به پشت
        cardImage.sprite = backImage;

        for (float i = 0; i <= 1; i += Time.deltaTime * 2)
        {
            transform.localScale = new Vector3(Mathf.Lerp(0, 1, i), 1, 1);
            yield return null;
        }

        isFront = false;
    }

    private IEnumerator FlipToFront()
    {
        // چرخش کارت
        for (float i = 0; i <= 1; i += Time.deltaTime * 2)
        {
            transform.localScale = new Vector3(Mathf.Lerp(1, 0, i), 1, 1);
            yield return null;
        }

        // تغییر تصویر به جلو
        cardImage.sprite = frontImage;

        for (float i = 0; i <= 1; i += Time.deltaTime * 2)
        {
            transform.localScale = new Vector3(Mathf.Lerp(0, 1, i), 1, 1);
            yield return null;
        }

        isFront = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<Image>().color == Color.white && GetComponent<Button>().enabled == false)
        {
            lasttransform = transform.position;
        }
        //throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetComponent<Image>().color == Color.white && GetComponent<Button>().enabled == false&& end==false)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(end==false&&GetComponent<Image>().color == Color.white)
        {
            if (lasttransform.y - transform.position.y < lasttransform.y - 300)
            {
                transform.parent.GetComponent<sorta_online>().q();
                transform.parent.GetComponent<sorta_online>().move(gameObject);
                return;
            }
            transform.position = lasttransform;
        }
    }
    public int P
    {
        get { return p; }
        private set { p = value; }
    }

    public int Rol
    {
        get { return rol; }
        private set { rol = value; }
    }
    public bool isfront
    {
        get { return isFront; }
        private set { isFront = value; }
    }
    public void ScaleCardToFit()
    {
        // دریافت اندازه Canvas
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // محاسبه اندازه Canvas
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // محاسبه عرض و ارتفاع کارت بر اساس نسبت ابعاد
        float targetCardWidth;
        float targetCardHeight;

        // محاسبه بر اساس بزرگترین ابعاد ممکن که کارت در Canvas جا بگیرد و نسبت ابعاد حفظ شود
        if (canvasWidth / canvasHeight < cardAspectRatio)
        {
            // Canvas به طور افقی کشیده‌تر است
            targetCardWidth = canvasWidth * 0.3f;  // مثلاً 30 درصد عرض Canvas
            targetCardHeight = targetCardWidth / cardAspectRatio;
        }
        else
        {
            // Canvas به طور عمودی کشیده‌تر است
            targetCardHeight = canvasHeight * 0.4f; // مثلاً 40 درصد ارتفاع Canvas
            targetCardWidth = targetCardHeight * cardAspectRatio;
        }

        // تنظیم اندازه کارت
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(targetCardWidth, targetCardHeight);

        // تنظیم موقعیت کارت در وسط صفحه
        rect.anchoredPosition = Vector2.zero; // مرکز Canvas
    }

    public void playsound()
    {
        GetComponent<AudioSource>().clip = clips[random.Range(0, clips.Count)];
        GetComponent<AudioSource>().Play();
    }
}