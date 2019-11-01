using UnityEngine;


[RequireComponent(typeof(Damageable))]
public class PoolableEnemy : PoolableObject{

    private Damageable _damagable;
    private EnemyPool _enemyPool;


    public void Start() {
        DamagableCmp.EOnDeath += OnEnemyDeath;
    }//Start


    public void OnEnemyDeath(GameObject instigator) {
        if (_enemyPool == null) {
            _enemyPool = this.OwnedByPool as EnemyPool;
        }
        if(_enemyPool == null)
            return;

        _enemyPool.AddDeadBody(this);
    }//OnEnemyDeath


    public Damageable DamagableCmp {
        get {
            if (_damagable == null)
                _damagable = GetComponent<Damageable>();
            return _damagable;
        }//get
    }//DamagableCmp

}//class
