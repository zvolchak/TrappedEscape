using System.Collections.Generic;
using UnityEngine;
using GHTriggers;
// DEPR??? Using Lever + DamageOnCollide instead

///<summery>
///</summery>
[RequireComponent(typeof(Collider2D))]
public class Spikes : LeverReactor {

    public LayerMask DamagableLayer;
    public int Damage = 100;

    private Collider2D _collider;
    private Animator _animator;
    private Dictionary<GameObject, Damageable> knownDamagables = new Dictionary<GameObject, Damageable>();
    private List<GameObject> currentDamaged = new List<GameObject>();


    public void Start() {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        if (DefaultState) {
            Activate();
        } else {
            Deactivate();
        }
    }//Start


    /// <summary>
    ///  Check if layer of collision is of the target, get its Damagabale object,
    /// save it to the "history" list to save performance later and apoply damage
    /// to the object.
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision) {
        if (!this.SwitchState)
            return;

        GameObject targetGo = collision.gameObject;
        bool isLayer = GameUtils.Utils.CompareLayers(DamagableLayer, targetGo.layer);
        if (!isLayer)
            return;

        if (this.currentDamaged.Contains(targetGo))
            return;

        Damageable dmg = null;
        if (!this.knownDamagables.ContainsKey(targetGo)) {
            dmg = collision.GetComponent<Damageable>();
            this.knownDamagables.Add(targetGo, dmg);
        } else {
            dmg = this.knownDamagables[targetGo];
        }

        if (dmg == null)
            return;

        bool isCrit = Random.Range(0, 2) == 0 ? true : false;
        dmg.TakeDamage(this.gameObject, Damage, isCrit);

        this.currentDamaged.Add(collision.gameObject);
    }//OnTriggerEnter2D


    public void OnTriggerExit2D(Collider2D collision) {
        GameObject targetGo = collision.gameObject;
        bool isLayer = GameUtils.Utils.CompareLayers(DamagableLayer, targetGo.layer);
        if (!isLayer)
            return;
        if (this.currentDamaged.Contains(targetGo))
            this.currentDamaged.Remove(targetGo);
    }


    public override void Activate() {
        _animator.SetTrigger("In");

        _collider.enabled = true;
        this.SwitchState = true;
    }//Activate


    public override void Deactivate() {
        _animator.SetTrigger("Out");
        _collider.enabled = false;

        this.SwitchState = false;
        this.currentDamaged.Clear();
    }//Deactivate

}//Spikes
