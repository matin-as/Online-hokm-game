using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class animatio : MonoBehaviour
{
    public string path;
    private Image displayImage;
    public Sprite[] sprites;
    private void Start()
    {
        displayImage = GetComponent<Image>();
        if(string.IsNullOrEmpty(path))
        {
            sprites = Resources.LoadAll<Sprite>("pp");
        }
        else
        {
            sprites = Resources.LoadAll<Sprite>(path);
        }
    }
    private void OnEnable()
    {
        StartCoroutine("PlaySlideshow");
    }
    private void OnDisable()
    {
        StopCoroutine("PlaySlideshow");
    }
    IEnumerator PlaySlideshow()
    {
        if(sprites.Length <100)
        {
            yield return null;
        }
        displayImage.color = Color.white;
        int index = 0;
        float frameDuration = (0.6f / sprites.Length) * 0.8f;
        while (true)
        {
            displayImage.sprite = sprites[index];
            index = (index + 1) % sprites.Length;
            yield return new WaitForSeconds(frameDuration);
        }
    }
}
