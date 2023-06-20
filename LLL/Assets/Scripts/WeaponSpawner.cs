using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject pistolPrefab;
    public GameObject riflePrefab;
    public GameObject shotgunPrefab;
    public GameObject uziPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;

    private Dictionary<string, int> weaponCounts = new Dictionary<string, int>();

    void Start()
    {
        weaponCounts["pistol"] = 0;
        weaponCounts["rifle"] = 0;
        weaponCounts["shotgun"] = 0;
        weaponCounts["uzi"] = 0;

        SpawnWeapon(spawnPoint1);
        SpawnWeapon(spawnPoint2);
        SpawnWeapon(spawnPoint3);
        SpawnWeapon(spawnPoint4);
    }

    private void SpawnWeapon(Transform spawnPoint)
    {
        int weaponChoice = Random.Range(1, 5);

        switch (weaponChoice)
        {
            case 1:
                if (weaponCounts["pistol"] < 2)
                {
                    Instantiate(pistolPrefab, spawnPoint.position, spawnPoint.rotation);
                    weaponCounts["pistol"]++;
                }
                else
                {
                    SpawnWeapon(spawnPoint);
                }
                break;
            case 2:
                if (weaponCounts["rifle"] < 2)
                {
                    Instantiate(riflePrefab, spawnPoint.position, spawnPoint.rotation);
                    weaponCounts["rifle"]++;
                }
                else
                {
                    SpawnWeapon(spawnPoint);
                }
                break;
            case 3:
                if (weaponCounts["shotgun"] < 2)
                {
                    Instantiate(shotgunPrefab, spawnPoint.position, spawnPoint.rotation);
                    weaponCounts["shotgun"]++;
                }
                else
                {
                    SpawnWeapon(spawnPoint);
                }
                break;
            case 4:
                if (weaponCounts["uzi"] < 2)
                {
                    Instantiate(uziPrefab, spawnPoint.position, spawnPoint.rotation);
                    weaponCounts["uzi"]++;
                }
                else
                {
                    SpawnWeapon(spawnPoint);
                }
                break;
        }
    }

    public void SpawnWeaponAtRandomPoint()
    {
        int spawnPointChoice = Random.Range(1, 5);

        switch (spawnPointChoice)
        {
            case 1:
                SpawnWeapon(spawnPoint1);
                break;
            case 2:
                SpawnWeapon(spawnPoint2);
                break;
            case 3:
                SpawnWeapon(spawnPoint3);
                break;
            case 4:
                SpawnWeapon(spawnPoint4);
                break;
        }
    }
}
