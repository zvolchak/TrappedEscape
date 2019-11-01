using UnityEngine;


[CreateAssetMenu(menuName = "Props/AIConfig/Default")]
public class AIProps : ScriptableObject {

    //public Weapon TheWeapon;
    [Tooltip("Recovery time from a damage state")]
    public float RecoverTime = 0.4f;
    public float VisionRange = 5f;

    public float TimeOfDamage { get; private set; } = 0f;

    public void SetTimeOfDamage(float time) {
        TimeOfDamage = time;
    }//SetTimeOfDamage

}//class AIProps