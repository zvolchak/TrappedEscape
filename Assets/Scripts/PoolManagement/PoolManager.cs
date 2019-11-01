using System.Collections.Generic;
using UnityEngine;


///<summery>
///</summery>
public class PoolManager : MonoBehaviour {

    public static PoolManager Instance;

    public List<ObjectPool> _allPools;


    public void Start() {
        if(Instance != null) return;
        Instance = this;

        var poolz = GetComponentsInChildren<ObjectPool>();
        this._allPools = new List<ObjectPool>(poolz);
    }//Start


    public ObjectPool FindPoolByPrefab(PoolableObject prefab) {
        for (int i = 0; i < _allPools.Count; i++) {
            ObjectPool thePool = _allPools[i];
            if(thePool.ObjectToPool.Equals(prefab))
                return thePool;
        }

        return null;
    }//FindPoolByPrefab


}//PoolManager
