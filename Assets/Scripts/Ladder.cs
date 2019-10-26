using GHPhysics;
using GHPlatformerControls;
using System.Collections.Generic;
using UnityEngine;


namespace GHTriggers {
    public class Ladder : TriggerListener {

        private Dictionary<Collider2D, LadderClimber> climbersCash;


        public override void Start() {
            base.Start();

            if (this.climbersCash == null)
                this.climbersCash = new Dictionary<Collider2D, LadderClimber>();
        }//Start


        public override void OnTriggerEnter2D(Collider2D collision) {
            base.OnTriggerEnter2D(collision);
            if(!HasEntered)
                return;

            LadderClimber climber = null;
            if (this.climbersCash.ContainsKey(collision))
                climber = this.climbersCash[collision];
            else {
                climber = collision.GetComponent<LadderClimber>();
                this.climbersCash.Add(collision, climber);
            }

            if(climber != null)
                climber.SetLadder(this);
        }//OnTriggerEnter2D


        public override void OnTriggerExit2D(Collider2D collision) {
            base.OnTriggerExit2D(collision);
            if(!HasExited)
                return;

            if(!this.climbersCash.ContainsKey(collision))
                return;

            if(this.climbersCash[collision] == null)
                return;

            this.climbersCash[collision].UnsetLadder(this);
        }//OnTriggerExit2D

    }//class
}//namespace