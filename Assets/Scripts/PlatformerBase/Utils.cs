using UnityEngine;

namespace GameUtils {
    public static class Utils {

        public enum ControllerType { dualshock, xbox, keyboard, touchscreen }

        /// <summary>
        ///  Conver box collider bounds to world space.
        /// Taken from: http://answers.unity3d.com/questions/605550/how-to-convert-a-boxcollider2d-bounds-to-world-spa.html by vargonian.
        /// </summary>
        /// <param name="collider"> box collider component. </param>
        /// <returns></returns>
        public static Rect BoundsToWorld(ref BoxCollider2D collider) {
            float worldRight = collider.transform.TransformPoint(collider.offset + new Vector2(collider.size.x * 0.5f, 0)).x;
            float worldLeft = collider.transform.TransformPoint(collider.offset - new Vector2(collider.size.x * 0.5f, 0)).x;

            float worldTop = collider.transform.TransformPoint(collider.offset + new Vector2(0, collider.size.y * 0.5f)).y;
            float worldBottom = collider.transform.TransformPoint(collider.offset - new Vector2(0, collider.size.y * 0.5f)).y;
            return new Rect(worldTop,
                             worldRight,
                             worldBottom,
                             worldLeft
                            );
        }//BountsToWorld


        /// <summary>
        ///  Returns border coordinates of the box collider bounds. Use xMin, xMax, yMin, yMax.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns float>float[]{ top, right, bottom, left }</returns>
        public static Rect GetCorners(Bounds bounds) {
            Rect result = new Rect();
            result.xMin = bounds.center.x - bounds.extents.x;
            result.xMax = bounds.center.x + bounds.extents.x;

            result.yMax = bounds.center.y + bounds.extents.y;
            result.yMin = bounds.center.y - bounds.extents.y;
            return result;
        }//GetCorners


        /// <summary>
        ///  Transform Camera bounds (width, height) to world location.
        ///  Taken from: http://answers.unity3d.com/questions/501893/calculating-2d-camera-bounds.html by GeekyMonkey.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Bounds OrthographicBounds(this Camera camera) {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            return new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        }//OrthographicBounds


        /// <summary>
        ///  Check if target vector is withing bounds.
        /// Return [is_in_horizontal, is_in_vertical] boolian states.
        /// </summary>
        /// <param name="checkWithin">Bounds to check target overlap.</param>
        /// <param name="target">Target vector to check if in bounds.</param>
        public static bool[] IsWithinBounds(Bounds checkWithin, Vector3 target) {
            var inHorizontal = target.x > checkWithin.min.x && target.x < checkWithin.max.x;
            var inVertical = target.y > checkWithin.min.y && target.y < checkWithin.max.y;
            return new bool[] { inHorizontal, inVertical };
        }//IsWithinBounds


        /// <summary>
        ///  Unity Editor warning message only about not finding the game object
        /// on the scene.
        /// </summary>
        /// <param name="goName">Name of the game obbject that was not found.</param>
        public static void WarningGONotFound(string goName) {
        #if UNITY_EDITOR
            Debug.LogWarning("Couldn't find Spawner with the name '" + goName + "'!");
        #endif
        }//WarningGONotFound


        public static void ErrorMessage(string msg) {
        #if UNITY_EDITOR
            Debug.LogError(msg);
        #endif
        }//ErrorMessage


        public static void WarningMessage(string msg) {
            #if UNITY_EDITOR
                Debug.LogWarning(msg);
            #endif
        }//WarningMessage


        /// <summary>
        ///  Helps debugging joystick buttons names...
        /// </summary>
        public static void PrintAnyKeyDown() {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(kcode))
                    Debug.Log("KeyCode down: " + kcode + " = " + (int)kcode);
            }
        }//PrintAnyKeyDown


        /// <summary>
        ///  Return rotation Quaternion value from Origin position towards Input Position
        /// (typically mouse). Set your object's transform.rotation to this returned value
        /// to rotate (or look at) towards the Input Position.
        /// </summary>
        /// <param name="targetCamera">Main camera</param>
        /// <param name="origPos">Position of the object Which is looking towards input</param>
        /// <param name="inputPos">Position towards which origPos will be looking at.</param>
        public static Quaternion LookAtMouse(Camera targetCamera, Vector3 origPos, Vector3 inputPos) {
            var pos = targetCamera.WorldToScreenPoint(origPos);
            var dir = inputPos - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }//LookAtMouse


        /// <summary>
        ///  Rotate a Vector by desiered angle around axis. For example,
        /// passing (45, 0, 0) rotation angle value, will roatate origin
        /// direction around X axis by 45 degrees.
        /// </summary>
        /// <param name="origDirection">Origin Vector to rotate.</param>
        /// <param name="rotationAngle">Angle to rate origin vector by.</param>
        /// <returns>Rotate vector.</returns>
        public static Vector3 RotateVector(Vector3 origDirection, Vector3 rotationAngle) {
            return Quaternion.Euler(rotationAngle) * origDirection;
        }//RotateVector


        /// <summary>
        ///  Compare two layer masks and return True if the same. False - not the same.
        /// </summary>
        public static bool CompareLayers(LayerMask targetLayer, LayerMask whoToCheckAgainst) {
            return (targetLayer & 1 << whoToCheckAgainst) == 1 << whoToCheckAgainst;
        }//CompareLayers


        public static ControllerType GetConnectedController() {
            string[] names = Input.GetJoystickNames();
            for (int x = 0; x < names.Length; x++) {
                if (names[x].Length == 19) {
                    return ControllerType.dualshock;
                }
                if (names[x].Length == 33) {
                    return ControllerType.xbox;
                }
            }

            return ControllerType.keyboard;
        }//GetConnectedController


        /// <summary>
        ///  To get the size of the window (web) the game is running in.
        /// Source: https://answers.unity.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMainGameViewSize() {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }


        public static Quaternion RotateInDirection(Vector3 direction, Vector3 orig) {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion newRot = Quaternion.AngleAxis(angle, orig);

            return newRot;
        }//RotateInDirection

    }//Utils

}//namespace
