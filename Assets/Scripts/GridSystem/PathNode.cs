using UnityEngine;


///<summery>
///</summery>
public class PathNode : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;
    private Color origColor;
    private bool bHasSaved;


    public void Start() {
        //origColor = SpriteCmp.color;
        //bHasSaved = true;
    }//Start

    public void OnEnable() {
        if (!bHasSaved) {
            this.origColor = SpriteCmp.color;
            bHasSaved = true;
        }
    }//OnEnable


    public void OnDisable() {
        SpriteCmp.color = origColor;
    }//OnDisable


    public SpriteRenderer SpriteCmp {
        get {
            if(_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

}//PathNode
