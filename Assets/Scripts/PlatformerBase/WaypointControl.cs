using System.Collections.Generic;
using UnityEngine;


namespace GHMisc {
    public class WaypointControl : MonoBehaviour {
        public Transform PatrolPoints;
        public Pathfinder PathfinderCmp;

        private List<GridNode> _patrolPoints;
        private GridNode activePoint = null;
        private bool bHasInit = false;
        

        public void Update() {
            if (!bHasInit) {
                if(GridSystem.Instance == null)
                    return;
                _patrolPoints = this.findNodesFromTransform(this.PatrolPoints);
                bHasInit = true;
            }
        }//Update


        private List<GridNode> findNodesFromTransform(Transform target) {
            if (PatrolPoints == null)
                return null;

            Transform[] points = target.GetComponentsInChildren<Transform>();
            List<GridNode> result = new List<GridNode>();
            for (int i = 0; i < points.Length; i++) {
                Transform point = points[i];
                if(point == target) //Ignore self
                    continue;

                GridNode node = GridSystem.Instance.GetNodeFromWorld(point.position);
                if (!node) {
                    Debug.LogError("No Node for " + point.name + "!");
                    continue;
                }
                if(!result.Contains(node))
                    result.Add(node);
            }//for
            return result;
        }//findNodesFromTransform


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <param name="whoIsMoving"></param>
        /// <returns></returns>
        public Transform GetNext(GameObject whoIsMoving, Transform targetPoint=null) {
            GridNode targetNode = null;
            if(targetPoint == null && this.activePoint == null)
                targetNode = _patrolPoints[0];
            if(targetPoint == null && this.activePoint != null)
                targetNode = this.activePoint;

            if (targetNode == null) {
#if UNITY_EDITOR
                Debug.LogError("MoveTo targetPoint is null for " + this.name);
                return null;
#endif
            }
            
            int index = _patrolPoints.IndexOf(targetNode);
            if (index < 0) {
#if UNITY_EDITOR
                Debug.Log("GetNext: No targetPoint in the list for " + whoIsMoving.name);
#endif
                return null;
            }

            index++;

            if(index >= _patrolPoints.Count)
                index = 0;

            return _patrolPoints[index].transform;
        }//MoveTo


        /// <summary>
        ///  Set curren point to move towards. Pass null to unset.
        /// </summary>
        /// <param name="point"></param>
        public void SetActivePoint(Transform point) {
            GridNode target = GridSystem.Instance.GetNodeFromWorld(point.position);
            if (target == null) {
#if UNITY_EDITOR
                Debug.LogWarning("SetActivePoint node for " + point.transform.position + " is null!");
#endif
                return;
            }
            this.activePoint = target;
        }//SetActivePoint


        public Transform GetActivePoint(GameObject whoIsAsking) {
            if (this.activePoint == null)
                return null;

            return this.activePoint.transform;
        }//GetActivePoint

    }//class
}//namespace