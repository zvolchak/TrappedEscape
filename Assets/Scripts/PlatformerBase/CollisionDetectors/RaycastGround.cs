using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastGround : RaycastSystem {

    public override Vector2 RayDirection => -this.transform.up;

}//class RaycastBoxGround
