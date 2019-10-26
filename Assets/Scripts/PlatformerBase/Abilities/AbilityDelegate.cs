namespace GHAbilities {
    public class AbilityDelegate {
        /// <summary>
        ///  Something that is called when OnWall action happened, like a grab or
        /// wall jump.
        /// 
        /// Signature: (WallGrab cmp);
        /// </summary>
        /// <param name="cmp"></param>
        public delegate void OnWallAction(WallGrab cmp);

        /// <summary>
        ///  An Event called when Use() method of AAbility is called.
        ///  
        /// Signature: (AAbility usedAbility).
        /// </summary>
        /// <param name="abilityUsed"></param>
        public delegate void OnUseDelegate(AAbility usedAbility);
    }//class
}//namespace