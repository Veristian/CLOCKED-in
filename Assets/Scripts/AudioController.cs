using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterVolume;

    public const string MIXER_MASTER = "Master Volume";

    void Awake()
    {
        masterVolume.onValueChanged.AddListener(SetMasterVolume);
    }

    void Start()
    {
        masterVolume.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, 1f);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, masterVolume.value);
    }

    void SetMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }
}
