using UnityEngine;


///<summery>
///</summery>
[RequireComponent(typeof(Rigidbody2D))]
public class PoolableRigidBody : PoolableObject {

    private Rigidbody2D _rb;


    public void Start() {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
    }//Start


    public override void OnEnable() {
    }//OnEnable


    public override Rigidbody2D RigidBodyCmp {
        get {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }


}//PoolableRigidBody
