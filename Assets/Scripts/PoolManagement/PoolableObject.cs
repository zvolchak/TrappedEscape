using UnityEngine;


public class PoolableObject : MonoBehaviour{

    private SpriteRenderer _spriteRenderer;
    /// <summary>
    /// Return a GameObject that instantiated this object.
    /// </summary>
    public GameObject Instantiator { get; private set; } //who created this object

    public virtual void OnEnable() {
        if(this.SpriteCmp.enabled == false)
            this.SpriteCmp.enabled = true;
    }//OnEnable


    public virtual void OnDestroy() {
        SetActive(false);
    }


    public void Hide(bool state) {
        this.SpriteCmp.enabled = state;
    }


    public ObjectPool OwnedByPool { get; private set; }


    public void SetActive(bool state) {
        this.gameObject.SetActive(state);
    }

    public bool IsActive { get { return this.gameObject.activeSelf; } }

    public virtual Rigidbody2D RigidBodyCmp => null;


    /// <summary>
    ///  A pool that created this object will set itself as ThePool here.
    /// </summary>
    /// <param name="thePool"></param>
    public void SetOwnerPool(ObjectPool thePool) { this.OwnedByPool = thePool; }


    public void SetInstantiator(GameObject whoCreatedIt) {
        this.Instantiator = whoCreatedIt;
    }//SetInstantiator


    public SpriteRenderer SpriteCmp {
        get {
            if (_spriteRenderer == null) {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                if(_spriteRenderer == null)
                    _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }


    public override bool Equals(object other) {
        PoolableObject o = other as PoolableObject;
        if(o == null)
            return false;
        return o.name == this.name && o.OwnedByPool == this.OwnedByPool;
    }//Equals


    public override int GetHashCode() {
        return base.GetHashCode();
    }//GetHashCode


    public override string ToString() {
        return string.Format("Name: {0}; Pool: {1}", this.name, this.OwnedByPool.name);
    }//ToString

}//class
