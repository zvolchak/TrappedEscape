using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  Play sprite animation when object is enabled.
/// Destroy itself when animation cicle is done.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Sfx : MonoBehaviour {

    public ParticleSystem ParticlePrefab;
    public AudioClip AudioSoundEffect;
    [Tooltip("Set spawned particle as this object's child.")]
    public bool IsParticleAsChild = false;
    [Tooltip("Create particles instance on start and re-use this one instead of Instantiate every time..")]
    public bool IsAllowCashing;

    private AudioSource _audioSource;
    private ParticleSystem particlesObj;


    public void Start() {
        _audioSource = GetComponent<AudioSource>();
    }//Start


    /// <summary>
    ///  Spawn a prefab (if not null) at the set position.
    /// </summary>
    /// <param name="atPosition"></param>
    /// <param name="direction"></param>
    /// <returns>Spawned prefab gameObject.</returns>
    public ParticleSystem PlayParticles(Vector3 atPosition, float direction = 1){
        if (ParticlePrefab == null) {
            return null;
        }//if no particles

        ParticleSystem particle_instance;
        if (!IsAllowCashing) {
            particle_instance = Instantiate(ParticlePrefab);
        } else {
            if(this.particlesObj == null)
                particle_instance = Instantiate(ParticlePrefab);
            else
                particle_instance = this.particlesObj;

        }//if allow cashing

        particle_instance.transform.position = atPosition;
        //particle_instance.transform.localScale = new Vector3(direction, 
        //        particle_instance.transform.localScale.x, 
        //        particle_instance.transform.localScale.z);

        var rot = particle_instance.transform.rotation;
        particle_instance.transform.rotation = new Quaternion(rot.x, rot.y, direction * Mathf.Abs(rot.z), rot.w);
        particle_instance.GetComponent<ParticleSystemRenderer>().flip = new Vector3(direction, 0, 0);

        particle_instance.Play();
        particle_instance.gameObject.SetActive(true);

        if (IsParticleAsChild)
            particle_instance.transform.SetParent(this.transform);

        particle_instance.transform.localScale = Vector3.one;

        if (IsAllowCashing && this.particlesObj == null) {
            this.particlesObj = particle_instance;
        }

        if (SoundManager.Instance == null) {
            return particle_instance;
        }//if soundmanager
        if (AudioSoundEffect == null || AudioSourceCmp == null)
            return particle_instance;

        float vol = SoundManager.Instance.GetSfxVolume(AudioSourceCmp.volume);
        AudioSourceCmp.PlayOneShot(AudioSoundEffect, vol);
        return particle_instance;
    }//OnEnable


    public void StopParticles() {
        if(this.particlesObj == null)
            return;

        this.particlesObj.Stop();
    }//StopParticles



    public void PlaySound() {
        if(AudioSoundEffect == null)
            return;
        if(SoundManager.Instance == null)
            return;
        float globalVolume = SoundManager.Instance.GetSfxVolume(AudioSourceCmp.volume);
        AudioSourceCmp.volume = globalVolume;

        AudioSourceCmp.PlayOneShot(AudioSoundEffect);
    }//PlaySound


    public AudioSource AudioSourceCmp {
        get {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            return _audioSource;
        }//getAudioSourceCmp
    }//

}//class Sfx
