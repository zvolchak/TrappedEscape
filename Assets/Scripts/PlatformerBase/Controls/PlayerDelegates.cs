using UnityEngine;
using GHAbilities;

public class PlayerDelegates{

    public delegate void OnLandedEvent();
    public delegate void OnGroundStateChanged(bool prevState, bool newState);
    public delegate void OnVelocityUpdate(Vector2 prevVelocity, Vector2 currVelocity);
    public delegate void OnWallStateChanged(AbilityDelegate.OnWallAction wallJumpCmp);

}//class
