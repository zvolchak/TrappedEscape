using System.Collections.Generic;
using UnityEngine;


namespace GHWeaponry {
    ///<summery>
    ///</summery>
    public class Holster : MonoBehaviour {

        public AActor2D Owner;
        [Tooltip("Where weapons will be placed.")]
        public Transform ArmsSocket;
        public string FireInput = "Fire1";
        public string ReloadInput = "Reload";
        public string NextWeaponInput = "NextWeapon";
        public string PistolInput = "Pistol";
        public string AssoultRifleInpue = "AssoultRifle";

        private bool bIsFirePressed = false;
        private List<Weapon> availableWeapons = new List<Weapon>();
        private int weaponIndex = 0;

        /* ************************************************************************** */
        

        /// <summary>
        /// Signature: ShootDelegate(Weapon shootingWeapong);
        /// </summary>
        public Delegates.ShootDelegate EOnShootEvent;

        /// <summary>
        /// Signature: ReloadDelegate(bool status);
        /// </summary>
        public Delegates.ReloadDelegate EOnReloadEvent;

        /// <summary>
        /// Signature: WeaponSwitchDelegate(Weapon prevWeapon, Weapon nextWeapon);
        /// </summary>
        public Delegates.WeaponSwitchDelegate EOnWeaponSwitch;

        /* ************************************************************************** */

        public void Start() {
            //_crosshair = GetComponentInChildren<Crosshair>();
            foreach (Weapon w in GetComponentsInChildren<Weapon>()) {
                this.AddWeapon(w);
            }//foreach

            //EOnWeaponSwitch += OnWeaponSwitched;
        }//Start


        public void Update() {
            OnWeaponScrollInput();
            if (FireInput != "") {
                if (Input.GetButtonDown(FireInput))
                    bIsFirePressed = true;
                if (Input.GetButtonUp(FireInput))
                    bIsFirePressed = false;
            }

            if (ReloadInput != "")
                OnReaload();

            if (bIsFirePressed)
                this.OnFirePressed();
        }//Update


        public void OnReaload() {
            if (!Input.GetButtonDown(ReloadInput))
                return;
            if (ActiveWeapon == null)
                return;
            if (ActiveWeapon.Props.ClipStatus == ActiveWeapon.Props.ClipSize)
                return;
            if (ActiveWeapon.Props.IsReloading)
                return;
            StartCoroutine(ActiveWeapon.Props.ReloadCoRoutine());
        }//OnReaload


        public void OnWeaponScrollInput() {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            Weapon prevWeapon = ActiveWeapon;
            if (scroll > 0) {
                UpdateSelectionIndex(1);
            } else if (scroll < 0) {
                UpdateSelectionIndex(-1);
            }

            OnWeaponSwitched(prevWeapon, ActiveWeapon);
        }//OnWeaponScrollInput


        public void OnFirePressed() {
            if (ActiveWeapon == null)
                return;

            //Vector3 targetPos = _crosshair.transform.position;
            Vector3 targetPos = this.transform.position;
            if (CrossCmp != null)
                targetPos = CrossCmp.transform.position;
            Vector2 targetRot = (ActiveWeapon.transform.position - targetPos).normalized;
            var rot = Quaternion.LookRotation(targetRot);
            rot.x = 0;
            rot.y = 0;

            ActiveWeapon.Shoot(rot);

            EOnShootEvent?.Invoke(ActiveWeapon);
        }//OnFirePressed


        public void OnWeaponSwitched(Weapon prev, Weapon next) {
            if (prev != null && next != null && prev != next) {
                prev.Hide(true);
            }

            if (next != null) {
                next.Hide(false);
            }

            int nextWeaponIndex = this.availableWeapons.IndexOf(next);
            if (nextWeaponIndex >= 0)
                this.weaponIndex = nextWeaponIndex;

            EOnWeaponSwitch?.Invoke(prev, next);
        }//OnWeaponSwitched


        public bool AddWeapon(Weapon weaponToAdd) {
            if (this.availableWeapons.Contains(weaponToAdd))
                return false;

            this.availableWeapons.Add(weaponToAdd);

            if (this.Owner != null)
                weaponToAdd.Owner = this.Owner;

            //Make sure weapon's X scale is the same as the actor's BEFORE
            //weapon's Parent object is set.
            Vector3 wScale = weaponToAdd.transform.localScale;
            wScale.x = this.transform.localScale.x;

            weaponToAdd.transform.SetParent(this.transform);
            Vector3 armsPosition = Vector3.zero;
            if (ArmsSocket != null)
                armsPosition = ArmsSocket.transform.localPosition;

            if (ActiveWeapon != null && ActiveWeapon != weaponToAdd && !ActiveWeapon.IsHidden) {
                OnWeaponSwitched(this.ActiveWeapon, weaponToAdd);
                //weaponToAdd.Hide(true);
            }

            weaponToAdd.RigidBodyCmp.velocity = Vector2.zero;
            weaponToAdd.RigidBodyCmp.angularVelocity = 0f;
            weaponToAdd.RigidBodyCmp.isKinematic = true;
            if (weaponToAdd.ColliderCmp != null) {
                weaponToAdd.ColliderCmp.enabled = false;
            }

            weaponToAdd.transform.localPosition = armsPosition;
            weaponToAdd.transform.SetParent(this.transform);
            weaponToAdd.transform.localScale = wScale;

            weaponToAdd.Props.EOnReloadEvent -= OnReloadEvent;
            weaponToAdd.Props.EOnReloadEvent += OnReloadEvent;

            return true;
        }//AddWeapon


        /// <summary>
        ///  Add or Substruct a value to the weapon selection index.
        /// O
        /// </summary>
        /// <param name="addOrSubstruct"></param>
        public void UpdateSelectionIndex(int addOrSubstruct) {
            this.weaponIndex += addOrSubstruct;
            if (this.weaponIndex < 0)
                this.weaponIndex = this.availableWeapons.Count - 1;
            if (this.weaponIndex > this.availableWeapons.Count - 1)
                this.weaponIndex = 0;
        }//UpdateSelectionIndex


        /// <summary>
        ///  This function needed to abstract multiple weapons event away from the
        /// outside world callers. Many Weapons - but only One can be active at a time.
        /// Thus, outside world need to register to one event in here, instead of to
        /// all possible weapons.
        /// </summary>
        /// <param name="status"></param>
        public void OnReloadEvent(bool status) {
            EOnReloadEvent?.Invoke(status);
        }//OnReloadEvent


        public Weapon ActiveWeapon {
            get {
                if (this.availableWeapons.Count == 0)
                    return null;
                if (this.weaponIndex < 0)
                    return null;
                if (this.weaponIndex > this.availableWeapons.Count - 1)
                    return null;
                return this.availableWeapons[this.weaponIndex];
            }//get
        }//ActiveWeapon


        public Crosshair CrossCmp {
            get {
                //if(ActiveWeapon == null)
                //    return _crosshair;
                //if(ActiveWeapon.CrosshairCmp == null)
                //    return _crosshair;
                return ActiveWeapon.CrosshairCmp;
            }
        }

    }//Holster
}//namespace