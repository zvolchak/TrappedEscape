using UnityEngine;
using GHTriggers;
using System.Collections.Generic;

public class SpriteShifter : TriggerListener {

    public List<SpriteShifter> ShifterConnections;

    public bool IsMainTerminal { get; private set; }

    private Dictionary<int, Imposter> imposters = new Dictionary<int, Imposter>();
    private bool bHasUsed = false;


    public override void Start() {
        base.Start();
        this.IsMainTerminal = true;

        for (int i = 0; i < ShifterConnections.Count; i++) {
            if(ShifterConnections[i] == null)
                continue;
            if (ShifterConnections[i].ShifterConnections.Contains(this))
                continue;

            ShifterConnections[i].ShifterConnections.Add(this);
        }//for
    }//Start


    public override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if(!HasEntered)
            return;

        Imposter imp = this.tryCashImposter(collision.gameObject);
        if(imp == null)
            return;

        imp.SetShifter(this);
    }//OnTriggerEnter2D


    public override void OnTriggerExit2D(Collider2D collision) {
        base.OnTriggerExit2D(collision);
        if(!HasExited)
            return;

        Imposter imp = this.tryCashImposter(collision.gameObject);
        if(imp == null)
            return;

        imp.SetShifter(null);
        bHasUsed = false;
        this.IsMainTerminal = true;
        for (int i = 0; i < ShifterConnections.Count; i++) {
            ShifterConnections[i].IsMainTerminal = true;
        }//for
    }//OnTriggerExit2D


    public override void OnTriggerStay2D(Collider2D collision) {
        base.OnTriggerStay2D(collision);
        if(!IsStaying)
            return;

        if(!IsMainTerminal)
            return;

        Imposter imp = this.tryCashImposter(collision.gameObject);
        if (Input.GetAxisRaw("Use") != 0 && !bHasUsed) {
            Debug.Log("Shifting -> " + this.name);
            this.IsMainTerminal = true;
            for (int i = 0; i < ShifterConnections.Count; i++) {
                ShifterConnections[i].IsMainTerminal = false;
            }//for

            bHasUsed = true;
            Shift(imp);
        }
    }//OnTriggerStay2D


    private Imposter tryCashImposter(GameObject go) {
        Imposter imp = null;
        int goHash = go.GetHashCode();
        bool hasKey = this.imposters.ContainsKey(goHash);

        if (hasKey) {
            imp = this.imposters[goHash];
            hasKey = true;
        } else {
            imp = go.GetComponent<Imposter>();
        }//if else

        return imp;
    }//tryCashImposter


    public void Shift(Imposter cameInImp) {
        if(cameInImp == null)
            return;

        List<Imposter> toShift = new List<Imposter>();

        if (cameInImp.Target == null) {
            //The one who came in is the one being Impostered.
            toShift.AddRange(cameInImp.ImposteredBy);
        } else {
            if (cameInImp.Impostered == null) {
                #if UNITY_EDITOR
                    Debug.LogWarning("Impostered not set while trying to Shift() by " + cameInImp.gameObject.name);
                #endif
                return;
            }
            //The one who came in is The Imposter.
            toShift.Add(cameInImp);
        }

        for (int i = 0; i < toShift.Count; i++) {
            if(toShift[i].IsCanShift)
                toShift[i].ShowTrueSprite();
        }//for
    }//Shift


    public void OnDrawGizmos() {
        for (int i = 0; i < ShifterConnections.Count; i++) {
            if(ShifterConnections[i] == null)
                continue;
            Gizmos.DrawLine(this.transform.position, ShifterConnections[i].transform.position);
        }//for
    }//OnDrawGizmos

}//class
