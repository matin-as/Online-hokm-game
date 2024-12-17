using UnityEngine;
using UnityEngine.UI;

public class EnsureVisibility : MonoBehaviour
{
    public GameObject targetObject; // شیء که می‌خواهید مطمئن شوید همیشه دیده می‌شود
    public Canvas canvas;
    private void Start()
    {
        targetObject = gameObject;
        canvas = GameObject.Find("canvas").GetComponent<Canvas>(); ;
    }
    void Update()
    {
        RectTransform rectTransform = targetObject.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        bool isVisible = true;
        foreach (Vector3 corner in corners)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(canvas.GetComponent<RectTransform>(), corner, canvas.worldCamera))
            {
                isVisible = false;
                break;
            }
        }

        if (!isVisible)
        {
            BringToFront();
        }
    }

    void BringToFront()
    {
        targetObject.transform.SetAsLastSibling();
    }
}