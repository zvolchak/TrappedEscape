using System.Collections.Generic;
using UnityEngine;


///<summery>
///</summery>
[RequireComponent(typeof(Collider2D))]
public class PlatformDropthrough : MonoBehaviour {

    public float ToggleTimeout = 0.2f;
    public string PlatformTag = "Platform";
    public string DownInput = "Vertical";
    public string JumpInput = "Jump";
    public bool IsDropping { get; private set; }

    private List<Collider2D> _colliders;
    //private RaycastGround _raycastGround;
    private CollisionDetection _cd;
    private bool isDownPressed, isJumpPressed;
    private bool isReleasedSinceDrop = true;

    /// <summary>
    ///  Not used anywhere in the scope of this class, but can be set by outside
    /// world and checkfor somewhere in AACtor or whatever.
    /// </summary>
    public bool IsCanDropthrough { get; private set; }


    public void Start() {
        //_raycastGround = GetComponent<RaycastGround>();
        _colliders = new List<Collider2D>();
        _cd = GetComponent<CollisionDetection>();
        var colliders = GetComponents<Collider2D>();
        //Object can have several colliders. Need to get only
        //the non trigger ones.
        foreach (Collider2D cld in colliders) {
            if(!cld.isTrigger)
                _colliders.Add(cld);
        }//foreach

        IsCanDropthrough = true;
    }//Start


    public void Check() {
        if(Input.GetButtonDown(DownInput))
            isDownPressed = true;
        if (Input.GetButtonUp(DownInput)) {
            isDownPressed = false;
            isReleasedSinceDrop = true;
        }

        if (Input.GetButtonDown(JumpInput))
            isJumpPressed = true;
        if (Input.GetButtonUp(JumpInput)) {
            isJumpPressed = false;
            isReleasedSinceDrop = true;
        }
        if (IsDropping || !isReleasedSinceDrop) //already dropping down. Don't need to check again.
            return;

        bool isPlatform = false;
        if (isDownPressed && isJumpPressed) {
            isPlatform = IsOnPlatform();
            if (isPlatform) {
                OnDropthrough();
                isReleasedSinceDrop = false;
            }
        }
    }//Update


    public bool IsOnPlatform() {
        //RaycastMeta[] rays = _raycastGround.OnRaycast();
        RaycastMeta rays = _cd.CastVerticalRay(-1, 2);
        int index = (int)(rays.Length / 2);
        if (rays == null || !rays.Ray)
            return false;

        GameObject hittedObj = rays.Ray.transform.gameObject;
        if(hittedObj == null)
            return false;

        return hittedObj.tag.ToLower().Equals(PlatformTag.ToLower());
    }//IsOnPlatform


    public void OnDropthrough() {
        if(IsDropping)
            return;
        StartCoroutine(ToggleColliders(ToggleTimeout));
    }//OnDropthrough


    public System.Collections.IEnumerator ToggleColliders(float timeout) {
        Toggle(false);                
        IsDropping = true;
        yield return new WaitForSeconds(timeout);
        Toggle(true);
        IsDropping = false;
    }//ToggleCollider


    public void Toggle(bool state) {
        for (int i = 0; i < _colliders.Count; i++) {
            _colliders[i].enabled = state;
        }//for
    }//Toggle


    public void SetCanDropthrough(bool state) {
        IsCanDropthrough = state;
    }

}//PlatformDropthrough
