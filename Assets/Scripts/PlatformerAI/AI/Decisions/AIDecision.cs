﻿using UnityEngine;


///<summery>
///</summery>
[CreateAssetMenu(menuName="AI/Decisions/Default")]
public abstract class AIDecision : ScriptableObject {

    public string ToggleStateName;

    public abstract bool Decide(AIController controller);

}//AIDecision
