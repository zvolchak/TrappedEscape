using GHActor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIStamina))]
public class UIStamina : MonoBehaviour {

    public Color StaminaUnavailable = Color.red;

    private Player _player => null; //FIXME: used to be player instance! Something else here!
    private StaminaAttribute _stamina;
    private bool bHasInit = false;
    private Slider _slider;
    private Image _fill;
    private Color origFillColor;


    public void Start() {
        if (_player == null)
            return;

        _stamina = _player.GetComponent<StaminaAttribute>();
        _slider = GetComponent<Slider>();
        _fill = _slider.GetComponentInChildren<Image>();
        this.origFillColor = _fill.color;

        _slider.maxValue = _stamina.Amount;
        _slider.value = _stamina.Status;

        _stamina.EOnStaminaUpdated += OnStaminaUpdated;
        _stamina.EOnFullyDrained += OnStaminaDrained;
        _stamina.EOnFullyRestored += OnStaminaRestored;

        bHasInit = true;
    }//Start


    public void Update() {
        if (!bHasInit) {
            Start();
        }
    }//Update


    public void OnStaminaUpdated(float currStamina) {
        _slider.value = currStamina;
    }//OnStaminaUpdated


    public void OnStaminaDrained() {
        _fill.color = StaminaUnavailable;
    }//OnStaminaDrained


    public void OnStaminaRestored() {
        _fill.color = this.origFillColor;
    }//OnStaminaRestored

}//class
