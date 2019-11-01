namespace GHWeaponry {
    public class Delegates {
        public delegate void ShootDelegate(Weapon shootingWeapong);

        ///  Called on Start and End of the reload cycle.
        /// </summary>
        /// <param name="status">True - reload started. False - reload finished.</param>
        public delegate void ReloadDelegate(bool status);
        public delegate void WeaponSwitchDelegate(Weapon prevWeapon, Weapon nextWeapon);
    }//class
}//GHWeaponry
