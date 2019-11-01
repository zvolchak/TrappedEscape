using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Handy in 99% cases in situations where you need to reuse the same
/// type of GameObject\Prefab many times per second(s). E.g. bullet hell game where
/// you can pool projectiles and save CPU cicles in creating new gameojbects many times
/// per second(s).
/// </summary>
public class ObjectPool: MonoBehaviour {

    [Tooltip("Game object to be created into the pool.")]
    public PoolableObject ObjectToPool;
    [Tooltip("Number of objects instanciated on start.")]
    public int PoolSize = 5;
    [Tooltip("Allow pool to grow if there is not enough objects.")]
    public bool IsDynamic = true;
    [Tooltip("When IsDynamic=false, this will allow (or not) the re-use of the objects when they are active.")]
    public bool IsAllowLoopSpawn = true;
    [Tooltip("How many objects of the pool can be active simultaneously. '0' means no limit.")]
    public int SpawnLimit = 0;
    [Tooltip("Name of the GameObject in the scene to where to store pool instancies. If object not found - it will be created with that name.")]
    public string StorageBucketName = "Bucket";
    public GameObject ObjectStorage;


    public virtual int InactiveCount { get {
            int count = 0;
            foreach (PoolableObject go in this.pool)
                count = (go.IsActive) ? count : count + 1;
            return count;
        }//get
    }//InactiveCount

    public virtual int ActiveCount {
        get {
            if (this.pool == null)
                pool = new List<PoolableObject>();

            int count = 0;
            foreach (PoolableObject go in this.pool)
                count = (go.IsActive) ? count + 1: count;
            return count;
        }//get
    }//ActiveCount


    protected List<PoolableObject> pool;
    protected GameObject bucket; //parent for all the clones objects.

    private int spawnedIndex = 0;

    /* ------------------------------------------------------------------------------- */

    public virtual void Start() {
        if(this.pool == null)
            pool = new List<PoolableObject>();
        createBucket();
        for (int i = 0; i < PoolSize; i++) {
            AddToPool();
        }//for

        //this.name = "Pool_" + ObjectToPool.name;
    }//Start


    /// <summary>
    ///  Pick an object from the Inactive pool. This will just return an object and
    /// will not activate or do any other operations with it.
    /// When "IsDynamic" variable is set to True - this function will create new object 
    /// instance if there are no inactive available.
    /// </summary>
    /// <returns>GameObject instanc or null when pool has no inactive objects.</returns>
    public virtual PoolableObject GetInactive() {
        for (int i = 0; i < this.pool.Count; i++) {
            PoolableObject go = this.pool[i];
            if (go == null)
                continue;
            if (!go.IsActive) {
                this.spawnedIndex = i;
                return go;
            }
        }//foreach

        //at this point, there is no avaailable objects to pool from.
        //This, create a new one to supply the demand.
        if (!IsDynamic) {
            if(IsAllowLoopSpawn)
                return GetFirstActive();
            else
                return null;
        }
        PoolableObject newObj = AddToPool();
        return newObj;
    }//GetInactive


    /// <summary>
    ///  Instantiate an object from the ObjectToPool prefab and add it to inactive pool,
    /// which can be picked from during the runtime.
    /// </summary>
    /// <returns> Instantiated game object that has been added to the pool. </returns>
    public virtual PoolableObject AddToPool() {
        if (SpawnLimit > 0) {
            if (ActiveCount >= SpawnLimit)
                return null;
        }//if
        PoolableObject go = Instantiate(ObjectToPool);
        go.name = go.name + "_" + (this.pool.Count + 1);

        if(bucket != null)
            go.transform.SetParent(bucket.transform);

        go.transform.position = new Vector3(0f, 0f, 0f);
        go.transform.localScale = go.transform.localScale;
        //go.gameObject.SetActive(false);
        go.SetActive(false);
        this.pool.Add(go);
        if(go.OwnedByPool == null)
            go.SetOwnerPool(this);

        return go;
    }//AddToPool


    /// <summary>
    ///  Get an object from an inactive pool and spawn it at the
    /// desiered location.
    /// </summary>
    /// <param name="spawnAt">Coords at which object will be spawned.</param>
    /// <returns>Spawned GameObject instance or Null, when no inactive found.</returns>
    public virtual PoolableObject SpawnAt(Vector3 spawnAt, bool dontEnable = false) {
        PoolableObject go = GetInactive();
        if (go == null)
            return null;
        go.transform.position = spawnAt;
        if (!dontEnable) {
            go.SetActive(true);
            //go.gameObject.SetActive(true);
        }
        return go;
    }//SpawnAt


    /// <summary>
    ///  Return game object back to inactive and into the Bucket parent.
    /// </summary>
    /// <param name="go"></param>
    public void Return(PoolableObject go) {
        if (!pool.Contains(go)) {
            pool.Add(go);
        }

        //go.gameObject.SetActive(false);
        go.SetActive(false);
        go.transform.SetParent(bucket.transform);
    }//Return


    /// <summary>
    ///  Path null if want to return All of the pooled objects
    /// </summary>
    /// <param name="toReturn"></param>
    public void ReturnAll(List<PoolableObject> toReturn) {
        if(toReturn == null)
            toReturn = new List<PoolableObject>(this.pool);
        for (int i = 0; i < toReturn.Count; i++) {
            //toReturn[i].gameObject.SetActive(false);
            toReturn[i].SetActive(false);
        }
    }//ReturnAll


    public List<PoolableObject> GetPool() { return this.pool; }


    /// <summary>
    ///  Return the first Active object in the pool.
    /// </summary>
    /// <returns>PoolableOBject on success. Null - no active objects.</returns>
    public PoolableObject GetFirstActive() {
        this.spawnedIndex += 1;
        if(this.spawnedIndex >= this.pool.Count - 1 || this.spawnedIndex < 0)
            this.spawnedIndex = 0;

        return this.pool[spawnedIndex];
    }//GetFirstActive


    /// <summary>
    ///  Create a "dummy" empty game object in the scene to where
    /// all the Pool instancies will be placed.
    /// </summary>
    protected void createBucket() {
        if (ObjectStorage != null) {
            this.bucket = ObjectStorage;
            return;
        }
        if (StorageBucketName == "") {
            bucket = this.gameObject;
            return;
        }
        bucket = GameObject.Find(StorageBucketName);
        if (bucket == null) {
            bucket = new GameObject();
            bucket.name = StorageBucketName;
        }//if null
    }//createBucket


    public void SetObjectToPool(PoolableObject prefabGo) {
        ObjectToPool = prefabGo;
    }//SetObjectToPool


    public void SetPoolSize(int size) {
        PoolSize = size;
    }//SetPoolSize

}//Class