using UnityEngine;


///<summery>
/// Something that spawns as a result of lunching a Projectile.
/// Could be a Shell from a bullet or Feather from a bow.
///</summery>
public class ProjectileShell : MonoBehaviour {

    public ShellProps Props;

    private ObjectPool _pool;
    private bool bPoolHasChecked = false;


    public void Spawn(float direction=1) {
        if (Props.Prefab == null)
            return;
        Rigidbody2D shell = null;

        //Do this once. Try to find a Pool with the given prefab
        //to use as a spawning mechanism.
        if (_pool == null && !bPoolHasChecked) {
            bPoolHasChecked = true;
            _pool = PoolMngrCmp.FindPoolByPrefab(Props.Prefab);
        }//if pool

        if (_pool != null) {
            shell = _pool.GetInactive().RigidBodyCmp;
        }

        //No pool found? No soup for u. Just instantiate, even tho it suck.
        if (shell == null) {
#if UNITY_EDITOR
            Debug.LogWarning(this.name + " instantiating the shell! No Pool for " + Props.Prefab);
#endif
            shell = Instantiate(Props.Prefab).RigidBodyCmp;
        }

        shell.transform.position = this.transform.position;

        float randX = Random.Range(Props.ForceX.x, Props.ForceX.y);
        float randY = Random.Range(Props.ForceY.x, Props.ForceY.y);
        float randTorque = Random.Range(Props.TorqueRange.x, Props.TorqueRange.y);

        Vector2 dir = direction * this.transform.right * randX + shell.transform.up * randY;

        shell.gameObject.SetActive(true);
        shell.AddForce(dir);

        shell.AddTorque(randTorque);
    }//SpawnBulletShell


    public PoolManager PoolMngrCmp => PoolManager.Instance;


}//ProjectileShell


[System.Serializable]
public class ShellProps {

    public PoolableObject Prefab;
    public Vector2 ForceX;
    public Vector2 ForceY;
    public Vector2 TorqueRange;

}//class