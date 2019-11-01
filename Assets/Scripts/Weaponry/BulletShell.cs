using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletShell : MonoBehaviour {

    public Vector2 PushForce = new Vector2(10, 30);
    public float SpinForce = 2.0f;
    public AudioClip BouncingSound;
    public LayerMask SoundOnLayer;
    
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private AudioSource _audioSource;
    private bool bIsSoundPlayed;


    public void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        bIsSoundPlayed = false;

        FlyAway();
    }//Start


    public void LateUpdate() {
        if(bIsSoundPlayed)
            CleanUp();
    }//LateUpdate


    public void CleanUp() {
        if(_audioSource == null)
            return;
        if(_audioSource.isPlaying)
            return;
        Destroy(_audioSource);
        //_audioSource.enabled = false;
        
    }//CleanUp


    public void FlyAway() {
        float swing = Random.Range(PushForce.x, PushForce.y);
        float spingSwing = Random.Range(-10, 10);

        var direction = Mathf.Sign(Player.Instance.transform.localScale.x);
        _rigidbody.AddForce(new Vector2(-1 * direction, 1) * (swing));
        _rigidbody.AddTorque(SpinForce + spingSwing);
    }//FlyAway


    public void OnCollisionEnter2D(Collision2D collision) {
        if ((SoundOnLayer & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;
        if(bIsSoundPlayed)
            return;
        if(BouncingSound == null || _audioSource == null)
            return;

        bIsSoundPlayed = true;
        _audioSource.PlayOneShot(BouncingSound);
    }//OnCollisionEnter2D

}//class BulletShell
