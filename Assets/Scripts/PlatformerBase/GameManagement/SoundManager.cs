using UnityEngine;


///<summery>
/// Control audio play through this class.
/// UI volume is set to SFX volume.
///</summery>
public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    public delegate void MusicVolumeChange(float val);
    public event MusicVolumeChange OnMusicVolumeChange;

    private AudioSource _uiSfx;
    private AudioSource _musicSfx;
    private AudioSource _soundEffects;


    public void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this) {
            //Destroy(this.gameObject);
            return;
        }
    }//Awake


    public void Start() {
        InitVolumeControllers();
    }//Start


    protected void InitVolumeControllers() {
        foreach (AudioSource child in this.GetComponentsInChildren<AudioSource>()) {
            switch (child.name.ToLower()) {
                case ("uisfx"):
                    _uiSfx = child;
                    continue;
                case ("musicsfx"):
                    _musicSfx = child;
                    continue;
                case ("soundeffects"):
                    _soundEffects = child;
                    continue;
            }
        }//foreach
    }//InitVolumeControllers


    public void PlaySoundEffect(AudioClip sound) {
        if(sound == null)
            return;
        if(SfxAudioSource == null)
            return;
        SfxAudioSource.PlayOneShot(sound);
    }//PlaySoundEffect


    public float GetSfxVolume(float yourSourceVolume=-1) {
        return GetAudioSourceVolume(SfxAudioSource, yourSourceVolume);
    }//SfxVolume


    public float GetMusicVolume(float yourSourceVolume = -1) {
        return GetAudioSourceVolume(MusicAudioSource, yourSourceVolume);
    }//MusicVolume


    public void SetSfxVolume(float newVolume) {
        SetAudioSourceVolume(ref _soundEffects, newVolume);
        SetAudioSourceVolume(ref _uiSfx, newVolume);
    }//SetSfxVolume


    public void SetMusicVolume(float newVolume) {
        if(OnMusicVolumeChange != null)
            OnMusicVolumeChange(newVolume);
        SetAudioSourceVolume(ref _musicSfx, newVolume);
    }//SetMusicVolume


    /// <summary>
    ///  Return a "global" audio source volume. Passing "yourSourceVolume" means
    /// a volume value relative to global volume with respect to "yourSourceVolume"
    /// will be returned: source.volume * yourSourceVolume
    /// </summary>
    /// <param name="source"></param>
    /// <param name="yourSourceVolume"></param>
    /// <returns></returns>
    public float GetAudioSourceVolume(AudioSource source, float yourSourceVolume = -1) {
        float result = 1f;
        if (source == null) { //source is not set? Just return max volume value.
            return result;
        }

        if (yourSourceVolume == -1)
            result = source.volume;
        else
            result = source.volume * yourSourceVolume;

        return (float)System.Math.Round(result, 2);
    }//GetAudioSourceVolume


    public void SetAudioSourceVolume(ref AudioSource source, float newVolume) {
        if (source == null) {
            return;
        }
        float rounded = Mathf.Floor(newVolume * (1000)) / (1000);

        source.volume = rounded;
    }//SetAudioSourceVolume


    public void OnApplicationQuit() {
        Instance = null;
    }//OnApplicationQuit

    /* **************************** GETTERS / SETTERS **************************** */

    public AudioSource UIAudioSource {
        get {
            if (_uiSfx == null)
                InitVolumeControllers();
            return _uiSfx;
        }//get
    }//AudioSourceCmp

    public AudioSource MusicAudioSource {
        get {
            if(_musicSfx == null)
                InitVolumeControllers();
            return _musicSfx;
        }//get
    }//MusicSfx


    public AudioSource SfxAudioSource {
        get {
            if(_soundEffects == null)
                InitVolumeControllers();
            return _soundEffects;
        }//get
    }//SoundEffects

}//SoundManager
