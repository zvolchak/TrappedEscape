using UnityEngine;

/// <summary>
///  
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorStartOffCycle : MonoBehaviour {

    public Vector2 StartDelay = new Vector2(0.1f, 0.5f);

    private Animator _animator;
    private bool bIsDelaying;

    public void Start() {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;

        bIsDelaying = true;
        float enableDelay = Random.Range(StartDelay.x, StartDelay.y);
        Invoke("EnableAnimator", enableDelay);
    }//Start


    public void Update() {
        if(!bIsDelaying)
            return;

        if(_animator.enabled == true)
            _animator.enabled = false;
    }


    public void EnableAnimator() {
        _animator.enabled = true;
        bIsDelaying = false;
    }//EnableAnimator

}// class AnimatorStartOffCycle
