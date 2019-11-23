namespace GHTriggers {

    ///<summery>
    ///</summery>
    public class LevelGOStateReact : LeverReactor {


        public void Start() {
            if (DefaultState)
                Activate();
            else
                Deactivate();
        }


        public override void Activate() {
            this.gameObject.SetActive(true);
        }

        public override void Deactivate() {
            this.gameObject.SetActive(false);
        }
    }//LevelGOStateReact
}//namespace