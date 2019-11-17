using System.Collections.Generic;
using UnityEngine;


namespace GHGameManager {
    public class EnemyTracker : MonoBehaviour {

        public GameObject EnemyPool;

        public List<AActor2D> enemies = new List<AActor2D>();
        public int diedCounter = 0;

        private bool hasInit = false;

        public delegate void DEnemyTrackerCallback(EnemyTracker et);
        public DEnemyTrackerCallback EOnAllEnemiesDied;


        public void Start() {
            var enemyList = EnemyPool.GetComponentsInChildren<AActor2D>();
            this.enemies = new List<AActor2D>(enemyList);
            this.subscribeToDeathEvent(true);
        }//Start


        public void Update() {
            if (!hasInit) {
                this.subscribeToDeathEvent(true);
            }
        }


        public void OnEnemyDied(GameObject go) {
            diedCounter++;
            if (diedCounter >= this.enemies.Count) {
                this.EOnAllEnemiesDied?.Invoke(this);
            }
        }//OnEnemyDied


        private void subscribeToDeathEvent(bool state) {
            for (int i = 0; i < this.enemies.Count; i++) {
                AActor2D enemy = this.enemies[i];
                if(enemy.DamagableCmp == null)
                    return;
                enemy.DamagableCmp.EOnDeath -= OnEnemyDied;
                if (state) enemy.DamagableCmp.EOnDeath += OnEnemyDied;
            }//for

            hasInit = true;
        }//subscribeToDeathEvent


        public void OnDisable() {
            this.subscribeToDeathEvent(false);
        }//OnDisable

    }//class
}//GHGameManager
