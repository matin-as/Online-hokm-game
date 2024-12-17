using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class vid : MonoBehaviour
{
    GameObject g;
    private void Start()
    {
        //SceneManager.sceneLoaded += OnSceneChange;
        //DontDestroyOnLoad(gameObject);
    }
    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex==1)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            g = GameObject.Find("Canvas");
            GameObject main = GameObject.Find("main");
            g.SetActive(false);
            VideoPlayer videoPlayer = main.GetComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
            for (int i = 0; i < main.transform.childCount; i++)
            {
                main.transform.GetChild(i).GetComponent<VideoPlayer>().Prepare();
            }
        }
    }
    private void OnVideoPrepared(VideoPlayer videoPlayer)
    {
        videoPlayer.Play();
        g.SetActive(true);
        //Destroy(gameObject);
    }
}
