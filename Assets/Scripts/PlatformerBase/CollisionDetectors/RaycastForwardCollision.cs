using UnityEngine;


public class RaycastForwardCollision : RaycastSystem {

    public override Vector2 RayOrigin {
        get {
            if(Mathf.Sign(this.transform.localScale.x) < 0)
                return this.calculateRayOrigin().bottomLeft;
            else
                return this.calculateRayOrigin().bottomRight;
        }//get
    }
    public override Vector2 RayDirectionPlacement => -Vector2.up;
    public override Vector2 RayDirection => this.transform.right * Mathf.Sign(this.transform.localScale.x);

}//class
