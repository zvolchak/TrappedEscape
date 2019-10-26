using UnityEngine;


namespace GHActor {
    ///<summery>
    ///</summery>
    public interface IStatusEffector {

        string GetName();

        bool ApplyEffect(AActor2D actor);
        bool RemoveEffect(AActor2D actor);

        /// <summary>
        ///  For how long should the effector be applied to the actor.
        /// </summary>
        /// <returns></returns>
        float GetMaxDuration();
        float GetCurrentDuration();

    }//StatusEffector
}//namespace