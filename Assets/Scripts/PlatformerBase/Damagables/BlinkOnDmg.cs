using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class BlinkOnDmg : AOnDmgEffect {

    public float Speed = 1f;
    [Range(0, 1)]public float Amount = 1f;

    public bool isBlinking;
    public float currBlinkTime = 0;
    public float direction = 1;
    private Renderer _renderer;


    public override void Start() {
        base.Start();
        this._renderer = GetComponent<Renderer>();
    }


    public void Update() {
        if (!isBlinking) {
            return;
        }

        this.currBlinkTime += Time.deltaTime * this.Speed * this.direction;
        if (this.currBlinkTime < 0) {
            this.isBlinking = false;
            this.currBlinkTime = 0f;
            this.direction = 1;
        }

        if (this.currBlinkTime >= Amount) {
            this.direction = -1;
            this.currBlinkTime = Amount;
        }

        _renderer.material.SetFloat("_FlashAmount", this.currBlinkTime);
    }//Update


    public override void OnDamageTaken(GameObject instigator, int dmgAmount) {
        isBlinking = true;
        this.currBlinkTime = 0f;
    }//OnDamageTaken


}//class

//[System.Serializable]
//public class BlinkEffectProps {


//}//class