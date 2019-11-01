using System.Collections.Generic;
using UnityEngine;


///<summery>
///</summery>
public class EnemyPool : ObjectPool {

    public int DeadBodiesLimit = 3;

    public List<PoolableEnemy> bodies = new List<PoolableEnemy>();


    public delegate void DeadBodyAddedEvent(PoolableEnemy addedBody, EnemyPool pool);
    public DeadBodyAddedEvent EOnDeadBodyAdded;


    public override void Start() {
        base.Start();
    }//Start


    public void AddDeadBody(PoolableEnemy toAdd) {
        if(this.bodies.Contains(toAdd))
            return;

        this.bodies.Add(toAdd);

        if (this.bodies.Count >= DeadBodiesLimit) {
            this.Return(this.bodies[0]);
            this.bodies.RemoveAt(0);
        }

        this.EOnDeadBodyAdded?.Invoke(toAdd, this);
    }//AddDeadBody


    public int DeadBodiesCount { get { return this.bodies.Count; } }

}//EnemyPool
