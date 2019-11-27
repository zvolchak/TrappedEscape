using System.Collections.Generic;
using UnityEngine;


namespace GHCamera {
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour {

        public Animator AnimatorCmp;
        public List<string> TriggerNames;


        public void ShakeRandomly() {
            if(AnimatorCmp == null)
                return;

            int triggerIndex = Random.Range(0, this.TriggerNames.Count);

            AnimatorCmp.SetTrigger(this.TriggerNames[triggerIndex]);
        }//ShakeRandomly


    }//CameraShake
}//namespace
