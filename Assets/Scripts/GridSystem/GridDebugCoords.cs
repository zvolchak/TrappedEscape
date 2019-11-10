using UnityEngine;
using UnityEngine.UI;

public class GridDebugCoords : MonoBehaviour{
    
    public Text Template;

    private GridSystem _grid;
    private RectTransform _rectTransform;


    public void Start() {
        _rectTransform = GetComponent<RectTransform>();
    }//Start


    public void Update() {
        if (_grid == null) {
            _grid = GridSystem.Instance;
            if(_grid != null)
                SpawnText();
        }
    }//Update


    public void SpawnText() {
        for (int i = 0; i < _grid.allNodes.Count; i++) {
            INode node = _grid.allNodes[i];
            if(node == null)
                continue;

            Vector2 viewportPos = WorldToScreen(node.GetPosition());
            Text go = Instantiate(Template) as Text;
            go.rectTransform.SetParent(_rectTransform);
            go.rectTransform.localScale = Vector3.one;
            go.rectTransform.anchoredPosition = viewportPos;

            var coords = node.GetCoords();
            go.text = coords[0] + ":" + coords[1];
        }//for

        Template.gameObject.SetActive(false);
    }//SpawnText


    public Vector2 WorldToScreen(Vector3 targetPos) {
        Vector2 viewPort = Camera.main.WorldToViewportPoint(targetPos);
        float x = (viewPort.x * _rectTransform.sizeDelta.x) - (_rectTransform.sizeDelta.x * 0.5f);
        float y = (viewPort.y * _rectTransform.sizeDelta.y) - (_rectTransform.sizeDelta.y * 0.5f);
        return new Vector2(x, y);
    }


}//class
