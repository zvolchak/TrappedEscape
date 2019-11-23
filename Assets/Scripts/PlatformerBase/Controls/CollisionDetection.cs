using System.Collections.Generic;
using UnityEngine;

// Based of:
//https://github.com/SebLague/2DPlatformer-Tutorial/blob/master/

[RequireComponent(typeof(BoxCollider2D))]
public class CollisionDetection : MonoBehaviour {

    /// <summary>
    ///  Skinwidth may cause hit ray confision: it will flick between Has 
    /// collision and no collision every other frame. Increasing from 0.015 to 
    /// 0.025 seemed to fix that issue.
    /// </summary>
    public const float SKINWIDTH = 0.055f;

    public CollisionDetectionProps Props;
#if UNITY_EDITOR
    public RaycastDebugColor DebugProps;
#endif

    public bool IsOnSlope { get; private set; }
    public bool IsFallingThrough { get; private set; }
    //Meta data for of the raycast performed this frame in the Vertical direction.
    public RaycastMeta[] VerticalRayMeta { get; private set; }
    //Meta data for of the raycast performed this frame in the Horizontal direciton.
    public RaycastMeta[] HorizontalRayMeta { get; private set; }

    public bool Above { get; private set; }
    public bool Below;
    public bool Left  { get; private set; }
    public bool Right { get; private set; }


    protected Vector2 raySpacing {
        get {
            Bounds bounds = ColliderCmp.bounds;
            bounds.Expand(SKINWIDTH * -2);
            Vector2 spaces = Vector2.zero;
            spaces.x = bounds.size.y / (this.Props.HorizontalRays - 1);
            spaces.y = (bounds.size.x) / (this.Props.VerticalRays - 1);
            return spaces;
        }//get
    }//raySpacing


    private BoxCollider2D   _boxCollider;
    private SRaycastOrigins rayOrigins;
    private float slopeAngleLastFrame, currentSlopeAngle;
    private RaycastHit2D currentVertHit;

    /* ****************************************************************************************** */

    struct SRaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }//struct

    /* ****************************************************************************************** */


    public void Start() {
        _boxCollider = GetComponent<BoxCollider2D>();
    }//Start


    public void Move(ref Vector3 deltaMovement) {
        Reset();
        UpdateRayOrigin();
        if(deltaMovement.x != 0)
            HorizontalCollisions(ref deltaMovement);
        VerticalCollisions(ref deltaMovement);
    }//Update


    /// <summary>
    ///  Cast Horizontal ray with respect to "rayLength" sign. SKINWIDTH is added 
    /// to the rayLength here.
    /// </summary>
    /// <param name="rayLength">Length of the ray and the direction (based of
    ///                         its sign) to cast. </param>
    /// <param name="positionIndex">Position of the ray relative to Number of
    ///                             rays count set in Props.</param>
    /// <returns>Casted ray meta data.</returns>
    public RaycastMeta CastHorizontalRay(float rayLength, int positionIndex) {
        float directionX = Mathf.Sign(rayLength);
        rayLength = Mathf.Abs(rayLength) + SKINWIDTH;

        Vector2 rayOrigin = (directionX == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
        Vector2 direction = Vector2.right * directionX;

        rayOrigin += Vector2.up * (raySpacing.x * positionIndex);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, 
                                            rayLength, this.Props.CollisionMask);

        RaycastMeta meta = new RaycastMeta(rayOrigin, direction, hit, rayLength, positionIndex);

#if UNITY_EDITOR
        if (this.DebugProps.On) {
            if (positionIndex == 0)
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.First);
            else
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.NoHit);
            if (hit)
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.Hit);
        }
#endif

        return meta;
    }//Raycast


    /// <summary>
    ///  Cast Vertically (down/up) ray with respect to "rayLength" sign. SKINWIDTH is added 
    /// to the rayLength here.
    /// </summary>
    /// <param name="rayLength">Length of the ray and the direction (based of
    ///                         its sign) to cast. </param>
    /// <param name="positionIndex">Position of the ray relative to Number of
    ///                             rays count set in Props.</param>
    /// <returns>Casted ray meta data.</returns>
    public RaycastMeta CastVerticalRay(float rayLength, int positionIndex) {
        float directionY = Mathf.Sign(rayLength); //current movement direction (up/down)
        rayLength = Mathf.Abs(rayLength) + SKINWIDTH;

        Vector2 rayOrigin = (directionY == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft;
        Vector2 direction = Vector2.up * directionY;

        rayOrigin += Vector2.right * (raySpacing.y * positionIndex);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction,
                                            rayLength, this.Props.CollisionMask);

        //To ignore raycast hits on itself, unselect the "Queries Start In Colliders"
        //at the "Project Settings -> Physics 2D

        RaycastMeta meta = new RaycastMeta(rayOrigin, direction, hit, rayLength, positionIndex);

#if UNITY_EDITOR
        if (this.DebugProps.On) {
            if (positionIndex == 0)
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.First);
            else
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.NoHit);
            if (hit)
                Debug.DrawRay(meta.Origin, meta.FullDir, DebugProps.Hit);
        }
#endif

        return meta;
    }//Raycast


    /// <summary>
    ///  Raycast horizontally (left/right) with respect to deltaMovement.
    /// This should be called every frame.
    /// </summary>
    public void HorizontalCollisions(ref Vector3 deltaMovement) {
        float directionX = Mathf.Sign(deltaMovement.x);

        if (this.HorizontalRayMeta == null || 
                this.HorizontalRayMeta.Length != this.Props.HorizontalRays)
            this.HorizontalRayMeta = new RaycastMeta[this.Props.HorizontalRays];

        for (int i = 0; i < this.Props.HorizontalRays; i++) {
            RaycastMeta meta = this.CastHorizontalRay(deltaMovement.x, i);

            if(this.HorizontalRayMeta[i] == null)
                this.HorizontalRayMeta[i] = new RaycastMeta();
            this.HorizontalRayMeta[i].Set(meta);

            //---- NO HIT ----
            if (!meta.Ray) continue;

            //Ignore rayhit for Platforms or Ignorable tags.
            if(this.Props.IsIgnoreTag(meta.HitTag)) continue;
            if(this.Props.IsPlatformTag(meta.HitTag)) continue;

            float slopeAngle = Vector2.Angle(meta.Ray.normal, Vector2.up);
            this.HorizontalRayMeta[i].Angle = slopeAngle;

            if (i == 0 && slopeAngle <= this.Props.ClimbingSlope) {
                float distanceToSlopeStart = 0;
                if (slopeAngle != slopeAngleLastFrame) {
                    distanceToSlopeStart = meta.Ray.distance - SKINWIDTH;
                    deltaMovement.x -= distanceToSlopeStart * directionX;
                }
                ClimbSlope(ref deltaMovement, slopeAngle);
                deltaMovement.x += distanceToSlopeStart * directionX;
            }//if

            if (!IsOnSlope || slopeAngle > this.Props.ClimbingSlope) {
                deltaMovement.x = (meta.Ray.distance - SKINWIDTH) * directionX;

                if (IsOnSlope)
                    deltaMovement.y = Mathf.Tan(currentSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMovement.x);

                Left = directionX == -1;
                Right = directionX == 1;
            }//if

            this.HorizontalRayMeta[i].Length = meta.Ray.distance;
        }//for
    }//HorizontalCollisions


    /// <summary>
    ///  Raycast vertically with respect to deltaMovement.
    /// This should be called every frame.
    /// </summary>
    public void VerticalCollisions(ref Vector3 deltaMovement) {
        float directionY = Mathf.Sign(deltaMovement.y); //current movement direction (up/down)

        if (this.VerticalRayMeta == null || 
                this.VerticalRayMeta.Length != this.Props.VerticalRays)
            this.VerticalRayMeta = new RaycastMeta[this.Props.VerticalRays];

        int numOfHits = 0;
        for (int i = 0; i < this.Props.VerticalRays; i++) {
            RaycastMeta meta = this.CastVerticalRay(deltaMovement.y, i);

            //Update Global ray Meta.
            if(this.VerticalRayMeta[i] == null)
                this.VerticalRayMeta[i] = new RaycastMeta();
            this.VerticalRayMeta[i].Set(meta);

            //if (meta.Ray && meta.Ray.collider.gameObject.GetHashCode() == this.gameObject.GetHashCode()) {
            //    Debug.Log(meta.Ray.collider.gameObject.name);
            //    continue;
            //}

            //---- NO HIT ----
            if (!meta.Ray) {
                if(i == 0) IsFallingThrough = false;   
                continue;
            }

            if (this.Props.IsIgnoreTag(meta.HitTag)) continue;

            bool isPlatformTag = this.Props.IsPlatformTag(meta.HitTag);
            numOfHits++;

            if (IsFallingThrough && isPlatformTag) continue;
            else IsFallingThrough = false;            

            if (directionY > 0)
                if (!IsFallingThrough && isPlatformTag) continue;

            deltaMovement.y = (meta.Ray.distance - SKINWIDTH) * directionY;

            Below = directionY == -1;
            Above = directionY == 1;
        }//for

        if(numOfHits == 0)
            IsFallingThrough = false;
    }//VerticalCollisions


    public void ClimbSlope(ref Vector3 velocity, float slopeAngle, int rayIndex=-1) {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y > climbVelocityY)
            return;

        velocity.y = climbVelocityY;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        Below = true;
        IsOnSlope = true;
        currentSlopeAngle = slopeAngle;
    }//ClimbSlope


    public void FallThrough() {
        IsFallingThrough = false;
        for (int i = 0; i < VerticalRayMeta.Length; i++) {
            var ray = VerticalRayMeta[i].Ray;

            if (!ray) continue;

            if (this.Props.PlatformTags.Contains(ray.collider.tag)) {
                IsFallingThrough = true;
                break;
            }
        }//for
    }//FallThrough


    public void UpdateRayOrigin() {
        Bounds bounds = ColliderCmp.bounds;
        bounds.Expand(SKINWIDTH * -2);

        rayOrigins.topLeft      = new Vector2(bounds.min.x, bounds.max.y);
        rayOrigins.topRight     = new Vector2(bounds.max.x, bounds.max.y);
        rayOrigins.bottomLeft   = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight  = new Vector2(bounds.max.x, bounds.min.y);
    }//UpdateRaycastOrigin


    public void Reset() {
        Above = Below = Left = Right = false;
        IsOnSlope = false;
        slopeAngleLastFrame = currentSlopeAngle;
        currentSlopeAngle = 0;
    }//Reset


    public BoxCollider2D ColliderCmp {
        get {
            if(_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider2D>();
            return _boxCollider;
        }
    }

}//class


[System.Serializable]
public class CollisionDetectionProps {

    public Vector2 NumberOfRays = new Vector2(5, 7);
    [Range(0, 90)] public float ClimbingSlope = 45;
    public LayerMask CollisionMask;

    public List<string> CollisionIgnoreTags;
    [Tooltip("Tag name for colliders that are Platforms to jump through. Case sensitive.")]
    public List<string> PlatformTags;


    public bool IsPlatformTag(string tag) {
        return this.PlatformTags.Contains(tag);
    }//IsPlatform


    public bool IsIgnoreTag(string tag) {
        return this.CollisionIgnoreTags.Contains(tag);
    }//IsCollisionIgnoreTag


    public int HorizontalRays { get { return Mathf.FloorToInt(this.NumberOfRays.x); } }
    public int VerticalRays { get { return Mathf.FloorToInt(this.NumberOfRays.y); } }

}//class CollisionDetectionProps