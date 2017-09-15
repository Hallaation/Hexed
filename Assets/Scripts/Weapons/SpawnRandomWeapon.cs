using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnRandomWeapon : MonoBehaviour
{
    public float SpawnTime = 1.0f;
    public GameObject[] Weapons;
    public bool ContinuallySpawnWeapons = false;
    private bool m_bCanSpawnGun = false;
    Timer weaponSpawnTimer;

    // Use this for initialization
    void Start()
    {
        weaponSpawnTimer = new Timer(SpawnTime);
        SpawnARandomWepaon();
    }

    // Update is called once per frame
    void Update()
    {
        //If i want to spawn more guns
        if (ContinuallySpawnWeapons && m_bCanSpawnGun)
        {
            //If I can spawn a gun and can continually spawn weapons, tick the timer
            if (weaponSpawnTimer.Tick(Time.deltaTime))
            {
                SpawnARandomWepaon();
            }
        }
        else
        {
            weaponSpawnTimer.CurrentTime = 0;
        }
    }
    private void FixedUpdate()
    {
        if (ContinuallySpawnWeapons)
        {
            Collider2D[] Colliders = Physics2D.OverlapCircleAll(this.transform.position , 1.0f);
            //Loop through all the colliders, if any of them are a weapon, I cannot spawn a gun, break out of the loop .
            foreach (Collider2D item in Colliders)
            {
                if (item.GetComponentInParent<Weapon>())
                {
                    m_bCanSpawnGun = false;
                    break;
                }
                else
                {
                    m_bCanSpawnGun = true;
                    break;
                }
            }

        }
    }

    void SpawnARandomWepaon()
    {
        int randomIndex = Random.Range(0 , Weapons.Length - 1);
        GameObject spawnedWeapon = Instantiate(Weapons[randomIndex]);
        spawnedWeapon.transform.position = this.transform.position;
        spawnedWeapon.transform.rotation = this.transform.rotation;
    }
}
