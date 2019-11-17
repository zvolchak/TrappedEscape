using System.Collections.Generic;
using UnityEngine;


public class Imposter : MonoBehaviour {

    public Player Target;
    public Sfx ExposedSfx;

    /// <summary>
    /// The Actor who has been Impostered by Imposter cmp.
    /// </summary>
    public List<Imposter> ImposteredBy { get; private set; }
    /// <summary>
    /// The one who has been Impostered, e.g. the Target.
    /// </summary>
    public Imposter Impostered { get; private set; }
    public SpriteShifter Shifter { get; private set; }
    public bool IsCanShift => this.Shifter != null;

    private AActor2D _actor;
    private bool bHasInit = false;
    private Sprite origSprite;


    public void Start() {
        _actor = GetComponent<AActor2D>();
    }


    protected void init() {
        if (bHasInit)
            return;
        if (Target == null) {
            bHasInit = true;
            return;
        }

        if (Target.SpriteRendererCmp == null)
            return;

        if(this.origSprite == null)
            this.origSprite = SpriteCmp.sprite;

        SpriteCmp.sprite = Target.SpriteRendererCmp.sprite;
        this.Impostered = Target.GetComponent<Imposter>();
        if (this.Impostered == null) {
            this.Impostered = Target.gameObject.AddComponent<Imposter>();
        }

        this.Impostered.SetImposter(this);

        _actor.DamagableCmp.EOnDeath -= KillTargetOnDeath;
        _actor.DamagableCmp.EOnDeath += KillTargetOnDeath;

        bHasInit = true;
    }//Start


    public void Update() {
        this.init();
    }


    public void ShowTrueSprite() {
        if(Target == null)
            return;
        if (this.origSprite == null) {
            #if UNITY_EDITOR
                Debug.LogWarning("No origSprite set to ShowTrueSprite by " + _actor.name);
            #endif
            return;
        }

        if(this.ExposedSfx != null)
            this.ExposedSfx.PlayParticles(this.transform.position);
        this.SpriteCmp.sprite = this.origSprite;
    }//ToggleSprite


    public void SetImposter(Imposter imp) {
        if (ImposteredBy == null)
            ImposteredBy = new List<Imposter>();

        if(!ImposteredBy.Contains(imp))
            ImposteredBy.Add(imp);
    }//SetImposter


    public void UnsetImposter(Imposter imp) {
        if (ImposteredBy == null)
            ImposteredBy = new List<Imposter>();

        if (ImposteredBy.Contains(imp))
            ImposteredBy.Remove(imp);
    }//UnsetImposter


    public void KillTargetOnDeath(GameObject go) {
        //This actor is Not an imposter. Prbbly a Player.
        if (this.Target == null)
            return;

        //Imposter reveild. Cant damage Player anymore.
        if (this.origSprite == SpriteCmp.sprite)
            return;
        
        AActor2D targetActor = Impostered.ThisActor;
        targetActor.DamagableCmp.TakeDamage(this.gameObject, int.MaxValue, false);
    }//KillTargetOnDeath


    public void SetShifter(SpriteShifter shifter) {
        this.Shifter = shifter;
    }


    public void OnDestroy() {
        _actor.DamagableCmp.EOnDeath -= KillTargetOnDeath;
    }


    public SpriteRenderer SpriteCmp => _actor.SpriteRendererCmp;
    public AActor2D ThisActor => _actor;

}//class
