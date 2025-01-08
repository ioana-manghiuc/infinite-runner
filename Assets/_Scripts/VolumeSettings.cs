using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Slider _soundtrackSlider;
    [SerializeField]
    private Slider _sfxSlider;

    public const string MIXER_SOUNDTRACK = "SoundtrackVolume";
    public const string MIXER_SFX = "SFXVolume";
    public const string SOUNDTRACK_KEY = "Soundtrack";
    public const string SFX_KEY = "SFX";

    private void Start()
    {
        SetSoundtrackVolume(_soundtrackSlider.value);
        SetSfxVolume(_sfxSlider.value);
    }
    private void Awake()
    {
        _soundtrackSlider.value = PlayerPrefs.GetFloat(SOUNDTRACK_KEY, 1f);
        _sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        _soundtrackSlider.onValueChanged.AddListener(SetSoundtrackVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(SOUNDTRACK_KEY, _soundtrackSlider.value);
        PlayerPrefs.SetFloat(SFX_KEY, _sfxSlider.value);
    }

    void SetSoundtrackVolume(float value)
    {
        if(value != 0)
            mixer.SetFloat(MIXER_SOUNDTRACK, Mathf.Log10(value) * 20);
        else
            mixer.SetFloat(MIXER_SOUNDTRACK, -80);
    }

    void SetSfxVolume(float value)
    {
        if(value != 0)
            mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        else
            mixer.SetFloat(MIXER_SFX, -80);
    }
}
