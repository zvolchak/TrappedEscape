using UnityEngine;


///<summery>
/// Something that needs te be inherited by any other AI related
/// behaviour objects.
///</summery>
public abstract class AIBase : StateMachineBehaviour {

    public AIDecision[] Decisions;
    public AIProps Props;
    public AIController NPC;

    //public AIDecision currDecision;


    public abstract bool Action();
    public abstract bool Interrupt();


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (NPC == null)
            NPC = animator.GetComponent<AIController>();
    }//OnStateEnter


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        CheckDecisions();
        //if(this.currDecision != null && decision != this.currDecision)
        //    return;
        //if(this.currDecision == null)
        //    this.currDecision = decision;

        this.Action();
    }//OnStateUpdate


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateExit(animator, stateInfo, layerIndex);
        this.Interrupt();
        //this.currDecision = null;
    }//OnStateExit


    public virtual void CheckDecisions() {
        int size = Decisions.Length;
        if (NPC == null) {
#if UNITY_EDITOR
            Debug.LogWarning("NPC controller for " + this.name + " is null!");
#endif
            return;
        }
        for (int i = 0; i < size; i++) {
            if (Decisions[i] == null) {
#if UNITY_EDITOR
                Debug.LogWarning("Decision " + i + " is null!");
#endif
                continue;
            }
            Decisions[i].Decide(NPC);
        }//for
    }//CheckDecisions

}//AIBase
