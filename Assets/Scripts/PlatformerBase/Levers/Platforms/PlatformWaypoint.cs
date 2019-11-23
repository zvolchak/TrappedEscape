using UnityEngine;

namespace GHTriggers {

    public class PlatformWaypoint : MonoBehaviour {
        public WaypointProps Props;
    }//class


    [System.Serializable]
    public class WaypointProps {

        [Tooltip("How long will the arrived platform stay at this waypoint.")]
        public float StayTime = -1f;

    }//class
}//namespace
