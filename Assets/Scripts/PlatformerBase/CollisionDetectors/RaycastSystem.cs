using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastSystem : MonoBehaviour {

    protected const float SKINWIDTH = 0.015f;

    public LayerMask CollisionLayer;
    public int NumberOfRays = 4;
    [Tooltip("Set to -1 to auto calculate spacing based of collider size")]
    public float RaySpacing = -1;
    public float RayLength = 0.5f;
    public RaycastDebugColor DebugColors;

    public Vector2 Offset = Vector2.zero;
    public bool IsOnSlope { get { return bIsOnSlope; } }
    public bool bIsOnSlope = false;
    public bool HasCollision = false;

    protected BoxCollider2D _boxCollider;
    protected Rigidbody2D _rigidBody;


    /* ****************************************************************************************** */

    public struct SRaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }//struct

    /* ****************************************************************************************** */


    public virtual void Start () {
        InitComponents(this.gameObject);
    }//Start


    public virtual void FixedUpdate() {
        HasCollision = false;
    }//FixedUpdate


    public virtual void Update() {
        //OnRaycast();
    }//Update


    /// <summary>
    ///  GetComponents from "target" game object. Usually it is this.gameobject.
    /// But sometimes this.transfrom.parent needs to be used instead.
    /// </summary>
    /// <param name="target">Gameobject to get components from.</param>
    public virtual void InitComponents(GameObject target) {
        if (_boxCollider == null)
            _boxCollider = target.GetComponent<BoxCollider2D>();
        if (_rigidBody == null)
            _rigidBody = target.GetComponent<Rigidbody2D>();
    }//InitComponents


    /// <summary>
    ///  Cast multiple rays from center of the sphere to its radius + SkinWidth.
    /// TODO: some useful docs here.
    /// </summary>
    public virtual RaycastMeta[] OnRaycast() {
        Vector2 start = RayOrigin + Offset; //Bottom right corner
        float length = RayLength;
        this.RayHits = new RaycastMeta[NumberOfRays];

        bIsOnSlope = false;
        for (int i = 0; i < this.NumberOfRays; i++) {
            Vector2 rayOrigin = start;
            rayOrigin -= RayDirectionPlacement * (GetRaySpacing() * i);

            var ray = Physics2D.Raycast(rayOrigin, RayDirection, length, CollisionLayer);
            
            //Saving Ray meta for future use outside of this for loop.
            //Not important for actualy circular raycasting.
            RaycastMeta rayMeta = new RaycastMeta {
                Origin = start,
                Direction = RayDirection,
                Length = ray.distance,
                Index = i,
                Ray = ray,
                Angle = Vector2.SignedAngle(ray.normal, Vector2.up)
            };

            this.RayHits[i] = rayMeta;

            Color debugColor = DebugColors.NoHit;
            if (DebugColors.On) {
                if (i == 0)
                    debugColor = DebugColors.First;
                if (i == NumberOfRays - 1)
                    debugColor = DebugColors.Last;
            }
            //No soup for this ray.
            if (!ray || ray.collider.gameObject == this.gameObject) {
                this.RayHits[i].Angle = 0;
                if (DebugColors.On)
                    Debug.DrawRay(rayOrigin, RayDirection * length, debugColor);
                continue;
            }//if not ray

            if (!bIsOnSlope && rayMeta.Angle != 0) {
                if (i < this.NumberOfRays / 2)
                    bIsOnSlope = true;
                //numOfRaysOnSlope++;
            }

            if (DebugColors.On) {
                Debug.DrawRay(rayOrigin, RayDirection * ray.distance, DebugColors.Hit); //use red to indicate collision
            }

            if(ray.collider.gameObject != this.gameObject)
                HasCollision = !bIsOnSlope;
        }//for

        return this.RayHits;
    }//CircularRaycast


    protected virtual SRaycastOrigins calculateRayOrigin() {
        Bounds bounds = ColliderCmp.bounds;
        bounds.Expand(SKINWIDTH * -2);
        SRaycastOrigins rayOrigins;
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        return rayOrigins;
    }//UpdateRaycastOrigin


    public virtual Vector2 RayOrigin => this.calculateRayOrigin().bottomRight;


    public virtual Vector2 RayDirectionPlacement { get {
            return Vector2.right;
        }
    }

    public RaycastMeta[] RayHits { get; private set; }

    /// <summary>
    ///  Direction in which Rays will be shot by default. Override This if want
    /// different direction.
    /// </summary>
    public virtual Vector2 RayDirection { get { return -this.transform.up; } }

    /* ************************** GETTERS ********************************** */

    public BoxCollider2D ColliderCmp {
        get {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider2D>();
            return _boxCollider;
        }//get
    }//ColliderCmp

    public float GetRaySpacing() {
        if(RaySpacing != -1)
            return RaySpacing;

        if (RayDirectionPlacement.y == 0) { //rays goes from right to left, therefore - use Width
            return ColliderCmp.size.x / NumberOfRays;
        } else { //bottom to top - use Height
            return ColliderCmp.size.y / NumberOfRays;
        }
    }//GetRaySpacing

}//class RaycastSystem


//public class RaycastMeta {
//    public Vector3 Origin;
//    public Vector3 Direction;
//    public Vector3 DirectionPoint;
//    public RaycastHit2D Ray;
//    public float Length;
//    public float Angle;
//    public int Index;
//}//RaycastMeta class


//[System.Serializable]
//public class RaycastDebugColor {
//    public bool On;

//    public Color NoHit = Color.white;
//    public Color Hit = Color.red;
//    public Color First = Color.cyan;
//    public Color Last = Color.gray;
//    public Color Special = Color.blue;
//}//RaycastDebugColor class