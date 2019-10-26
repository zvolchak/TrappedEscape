using UnityEngine;

/// <summary>
///  Raycast meta data describing an individual ray that has been casted.
/// </summary>
[System.Serializable]
public class RaycastMeta{
    public Vector3 Origin;
    public Vector3 Direction;
    public RaycastHit2D Ray;
    public float Length;
    public float Angle;
    public int Index;


    public RaycastMeta() { }

    public RaycastMeta(Vector3 orig, Vector3 dir, RaycastHit2D ray, float length, 
                        int index, float angle=float.NegativeInfinity) {
        this.Origin = orig;
        this.Direction = dir;
        this.Ray = ray;
        this.Length = length;
        this.Angle = angle;
        this.Index = index;
    }//RaycastMeta


    public void Set(RaycastMeta meta) {
        this.Origin = meta.Origin;
        this.Direction = meta.Direction;
        this.Ray = meta.Ray;
        this.Length = meta.Length;
        this.Angle = meta.Angle;
        this.Index = meta.Index;
    }

    
    /// <summary>
    /// A "Direction * Length" of the ray casted. If there is a Ray hit, then use
    /// Ray.distance instead of this.Length.
    /// </summary>
    public Vector3 FullDir {
        get {
            float length = this.Length;
            if(this.Ray)
                length = this.Ray.distance;
            return this.Direction * length;
        }
    }
    public string HitTag {
        get {
            if (!this.Ray) return "";
            else return this.Ray.collider.tag;
        }
    }//HitTag
}//RaycastMeta


[System.Serializable]
public class RaycastDebugColor {
    public bool On = true;

    public Color NoHit = Color.white;
    public Color Hit = Color.red;
    public Color First = Color.cyan;
    public Color Last = Color.gray;
    public Color Special = Color.blue;
}//RaycastDebugColor class