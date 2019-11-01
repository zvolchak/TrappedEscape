using System.Collections.Generic;
using UnityEngine;


namespace GHMisc {
    public class WaypointControl : MonoBehaviour {
        public Transform PatrolPoints;

        private List<Transform> _patrolPoints;
        private Transform activePoint = null;


        public void OnEnable() {
            if (PatrolPoints != null && this._patrolPoints == null) {
                Transform[] points = PatrolPoints.GetComponentsInChildren<Transform>();
                _patrolPoints = new List<Transform>(points);
                //PatrolPOints are added to the list along with children, so remove it.
                _patrolPoints.Remove(PatrolPoints);
            }
        }//OnEnable


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <param name="whoIsMoving"></param>
        /// <returns></returns>
        public Transform GetNext(GameObject whoIsMoving, Transform targetPoint=null) {
            if(targetPoint == null && this.activePoint == null)
                targetPoint = _patrolPoints[0];
            if(targetPoint == null && this.activePoint != null)
                targetPoint = this.activePoint;

            if (targetPoint == null) {
#if UNITY_EDITOR
                Debug.LogError("MoveTo targetPoint is null for " + this.name);
                return null;
#endif
            }
            
            int index = _patrolPoints.IndexOf(targetPoint);
            if (index < 0) {
#if UNITY_EDITOR
                Debug.Log("GetNext: No targetPoint in the list for " + whoIsMoving.name);
#endif
                return null;
            }

            index++;

            if(index >= _patrolPoints.Count)
                index = 0;

            return _patrolPoints[index];
        }//MoveTo


        /// <summary>
        ///  Set curren point to move towards. Pass null to unset.
        /// </summary>
        /// <param name="point"></param>
        public void SetActivePoint(Transform point) {
            this.activePoint = point;
        }//SetActivePoint


        public Transform GetActivePoint(GameObject whoIsAsking) {
            return this.activePoint;
        }//GetActivePoint

    }//class
}//namespace