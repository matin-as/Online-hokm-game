using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class saound : MonoBehaviour
{
    public List<int> leaves;
    [SerializeField] private Sprite select;
    [SerializeField] private Sprite de_select;
    [SerializeField] private Image main_image;
    [SerializeField] private Image Effect_image;
    [SerializeField] private Toggle toggle_MainSaound;
    [SerializeField] private Toggle toggle_EffectSaound;
    [SerializeField] private AudioClip main_saund;
    [SerializeField] private AudioSource effect_audioSource;
    private AudioSource audioSource;

    public bool play_Saound = true;
    public bool play_EffectSaound = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        play_Saound = SecurePlayerPrefs.GetInt("saound_main") == 0;
        play_EffectSaound = SecurePlayerPrefs.GetInt("saound_effect") == 0;
        if (de_select == null)
        {
            return;
        }
        UpdateSaoundUI(main_image, toggle_MainSaound, play_Saound, audioSource);
        UpdateSaoundUI(Effect_image, toggle_EffectSaound, play_EffectSaound, effect_audioSource);
        toggle_MainSaound.onValueChanged.AddListener(change_saound);
        toggle_EffectSaound.onValueChanged.AddListener(change_saound_effect);
    }

    private void UpdateSaoundUI(Image image, Toggle toggle, bool isPlaying, AudioSource audioSource)
    {
        audioSource.mute = !isPlaying;
        image.sprite = isPlaying ? select : de_select;
        image.enabled = true;
        toggle.isOn = isPlaying;
    }

    private void change_saound(bool isPlaying)
    {
        play_Saound = isPlaying;
        SecurePlayerPrefs.SetInt("saound_main", isPlaying ? 0 : 1);
        UpdateSaoundUI(main_image, toggle_MainSaound, play_Saound, audioSource);
    }

    private void change_saound_effect(bool isPlaying)
    {
        play_EffectSaound = isPlaying;
        SecurePlayerPrefs.SetInt("saound_effect", isPlaying ? 0 : 1);
        UpdateSaoundUI(Effect_image, toggle_EffectSaound, play_EffectSaound, effect_audioSource);
    }

    public void play_audio(AudioClip audioClip)
    {
        if (play_EffectSaound)
        {
            effect_audioSource.clip = audioClip;
            effect_audioSource.Play();
        }
    }
}
