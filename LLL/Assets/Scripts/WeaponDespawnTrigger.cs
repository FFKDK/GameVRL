using UnityEngine;

public class WeaponDespawnTrigger : MonoBehaviour
{
    public WeaponSpawner weaponSpawner;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Destroy(other.gameObject);
            weaponSpawner.SpawnWeaponAtRandomPoint();
        }
    }
}
