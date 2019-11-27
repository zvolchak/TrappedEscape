using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GHMisc {
    /// <summary>
    ///  This class controls the Patrolling path for the actor. It can figure out
    /// the "next" point to go to based of the current active point and position
    /// of the object.
    ///  Add this cmp to the GO with Patfinder cmp. This also requires GridSystem
    /// Instance present in the scene.
    /// </summary>
    [RequireComponent(typeof(Pathfinder))]
    public class WaypointControl : MonoBehaviour {

        public Transform PatrolPoints;
        public Pathfinder PathfinderCmp;
        public bool IsDebug = false;
        public float WaitAtPointInSeconds = 1f;

        public List<GridNode> PatrolPath;
        
        private GridNode activePoint = null;
        public bool bHasInit { get; private set; }
        private List<GridNode> origPoints;
        private int nextNodeIncreaseIndex = 1; //going for Next or Previous node.
        private Coroutine waitAtPointRoutine;

        public bool IsWaitingAtWaypoint => this.waitAtPointRoutine != null;

        public delegate void OnPointReached(WaypointControl wpCtrl);
        public OnPointReached EOnLastPointReached;
        public OnPointReached EOnFirstPointReached;
        public OnPointReached EOnPointReached;


        public void Start() {
            bHasInit = false;
        }


        public void Update() {
            if (!bHasInit) {
                if (GridSystem.Instance == null)
                    return;
                PatrolPath = this.findNodesFromTransform(this.PatrolPoints);

                Vector3 from = this.PatrolPath[0].transform.position;
                Vector3 to = this.PatrolPath[1].transform.position;
                var path = PathfinderCmp.FindPath(from, to);
                this.SetPatrolPath(path);
                bHasInit = true;
            }
        }//Update


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<GridNode> findNodesFromTransform(Transform target) {
            if (PatrolPoints == null)
                return null;

            Transform[] points = target.GetComponentsInChildren<Transform>();
            List<GridNode> result = new List<GridNode>();
            for (int i = 0; i < points.Length; i++) {
                Transform point = points[i];
                if(point == target) //Ignore self
                    continue;

                GridNode node = GridSystem.Instance.GetNodeFromWorld(point.position) as GridNode;
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
        /// <param name="whoIsMoving"></param>
        /// <returns></returns>
        public GridNode GetNext(Transform targetPoint=null) {
            GridNode targetNode = null;
            if(targetPoint == null && this.activePoint == null)
                targetNode = PatrolPath[0];
            if(targetPoint == null && this.activePoint != null)
                targetNode = this.activePoint;

            if (targetNode == null) {
                #if UNITY_EDITOR
                    Debug.LogError("MoveTo targetPoint is null for " + this.name);
                #endif
                return null;
            }
            
            int index = PatrolPath.IndexOf(targetNode);
            if (index < 0) {
                #if UNITY_EDITOR
                    Debug.Log("GetNext: No targetPoint in the list for " + this.name);
                #endif
                return null;
            }

            if (index >= PatrolPath.Count - 1) {
                if(this.waitAtPointRoutine != null)
                    this.waitAtPointRoutine = StartCoroutine(WaitAtWaypoint());
                EOnLastPointReached?.Invoke(this);
            } else if (index <= 0) {
                if(this.waitAtPointRoutine != null)
                    this.waitAtPointRoutine = StartCoroutine(WaitAtWaypoint());
                EOnFirstPointReached?.Invoke(this);
            } else {
                EOnPointReached?.Invoke(this);
            }

            index += nextNodeIncreaseIndex;

            if (index >= PatrolPath.Count) index = PatrolPath.Count - 1;
            else if (index < 0) index = 0;

            this.activePoint = PatrolPath[index];
            return PatrolPath[index];
        }//MoveTo


        public bool IsInPatrolArea(Transform target) {
            //FIXME: IMPLEMENT THIS
            // Check if target is on the GridNode that is in the path between
            // first and last point of the _patrolPoints
            GridNode currNode = GridSystem.Instance.GetNodeFromWorld(target.position) as GridNode;
            if (currNode == null) {
                #if UNITY_EDITOR
                Debug.LogWarning("IsInPatrolArea currNode is null for " + target.name);
                #endif
                return false;
            }
            return this.PatrolPath.Contains(currNode);
        }//IsInPatrolArea


        /// <summary>
        ///  Set curren point to move towards. Pass null to unset.
        /// </summary>
        /// <param name="point"></param>
        public void SetActivePoint(Transform point) {
            if (point == null) {
                this.activePoint = null;
                return;
            }
            GridNode target = GridSystem.Instance.GetNodeFromWorld(point.position) as GridNode;
            if (target == null) {
                #if UNITY_EDITOR
                Debug.LogWarning("SetActivePoint node for " + point.transform.position + " is null!");
                #endif
                return;
            }
            this.activePoint = target;
        }//SetActivePoint


        public Transform GetActivePoint() {
            if (this.activePoint == null)
                return null;

            return this.activePoint.transform;
        }//GetActivePoint


        public bool IsAtLastPoint(GridNode target=null) {
            if (target == null)
                target = this.activePoint;
            int size = this.PatrolPath.Count;
            return this.PatrolPath[size - 1] == target;
        }//IsAtLastPoint


        public bool IsAtFirstPoint(GridNode target = null) {
            if (target == null)
                target = this.activePoint;
            int size = this.PatrolPath.Count;
            return this.PatrolPath[0] == target;
        }//IsAtLastPoint


        /// <summary>
        ///  Return a node that is in direction of the Target or is furthest from
        /// it.
        /// </summary>
        /// <param name="target">An object to find a starting node relative to.</param>
        /// <param name="dir">Direction in which target suppose to be looking to.</param>
        public Transform FindStartNodeByDirection(Transform target, int dir) {
            if (PatrolPath == null || PatrolPath.Count < 2)
                return null;
            Transform first = PatrolPath[0].transform;
            Transform last = PatrolPath[PatrolPath.Count - 1].transform;

            Transform result = null;
            Transform furthest = first;
            for (int i = 0; i < 2; i++) {
                Vector3 pos = PatrolPath[i].transform.position;
                float distance = Vector2.Distance(target.position, pos);
                float direction = (target.position - pos).normalized.x;
                if (distance >= (target.position - furthest.position).x)
                    furthest = PatrolPath[i].transform;

                if (-Mathf.Sign(direction) != dir)
                    continue;

                if(result != null)
                    result = furthest;
                else
                    result = PatrolPath[i].transform;
            }//for

            if(result == null)
                result = furthest;
            return result;
        }


        public IEnumerator WaitAtWaypoint() {

            yield return new WaitForSeconds(WaitAtPointInSeconds);

            this.waitAtPointRoutine = null;
        }//WaitAtWaypoint


        public void SetPatrolPath(List<INode> newPoints) {
            bool isSaveOrig = false;
            if (this.origPoints == null) {
                isSaveOrig = true;
                this.origPoints = new List<GridNode>();
            }
            this.PatrolPath.Clear();
            for (int i = 0; i < newPoints.Count; i++) {
                GridNode node = newPoints[i] as GridNode;
                this.PatrolPath.Add(node);
                if(isSaveOrig)
                    this.origPoints.Add(node);
            }
            //this.patrolPoints = newPoints;
            SetActivePoint(null);
        }//SetPatrolPoints


        /// <summary>
        /// Restores PatrolPath to its original points.
        /// </summary>
        public void ResetPatrolPoints() {
            if(this.origPoints == null)
                return;
            GridNode[] path = new GridNode[this.origPoints.Count];
            this.origPoints.CopyTo(path);
            this.PatrolPath = new List<GridNode>(path);
            this.activePoint = null;
        }//ResetPatrolPoints


        public void FlipIterationIndex() {
            this.nextNodeIncreaseIndex = -this.nextNodeIncreaseIndex;
        }


        public void OnDrawGizmos() {
            if(!IsDebug)
                return;

            if (this.PatrolPath == null)
                return;

            for (int i = 0; i < this.PatrolPath.Count; i++) {
                Color drawColor = Color.blue;
                if (i == 0)
                    drawColor = Color.green;
                else if (i == this.PatrolPath.Count - 1)
                    drawColor = Color.red;

                if(this.PatrolPath[i] == this.activePoint)
                    drawColor = Color.yellow;

                Gizmos.color = drawColor;
                if (i > 0) {
                    Gizmos.DrawLine(this.PatrolPath[i - 1].GetPosition(), this.PatrolPath[i].GetPosition());
                }

                //if (i == this.patrolPoints.Count - 1)
                    //continue;

                GridNode node = this.PatrolPath[i];
                Gizmos.DrawSphere(node.GetPosition(), 0.2f);
            }//for
        }//OnDrawGizmos


    }//class
}//namespace