using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        PISTOL = 0,
        RIFLE,
        MACHINE_GUN
    }

    public enum PowerUp
    {
        NONE = 0,
        TRI_SHOT,
        LAZER_BEAM
    }

    [SerializeField] Bullet[] bullet_prefabs;
    [SerializeField] Player player;
    
    private ObjectPool<Bullet> pistol_pool;
    private ObjectPool<Bullet> rifle_pool;
    private ObjectPool<Bullet> machine_pool;

    private WeaponType current_weapon;
    private PowerUp current_powerup;

    private Vector3 direction;
    
    private float time_since_shot = 0;
    private float fire_rate = 0.25f;
    private float powerup_timer = 0;
    private float powerup_duration = 30f;

    private void Awake()
    {
        pistol_pool = new ObjectPool<Bullet>(createBulletPistol, onTakeBullet, onReturnBullet, onDestroyBullet, true, 50, 150);
        rifle_pool = new ObjectPool<Bullet>(createBulletRifle, onTakeBullet, onReturnBullet, onDestroyBullet, true, 50, 200);
        machine_pool = new ObjectPool<Bullet>(createBulletMachine, onTakeBullet, onReturnBullet, onDestroyBullet, true, 100, 300);
    }
    
    private Bullet createBulletPistol()
    {
        // create new instance
        Bullet bullet = Instantiate(bullet_prefabs[0], transform.position, transform.rotation);
        
        bullet.GetComponent<Bullet>().setPool(pistol_pool);

        return bullet.GetComponent<Bullet>();;
    }
    
    private Bullet createBulletRifle()
    {
        // create new instance
        Bullet bullet = Instantiate(bullet_prefabs[1], transform.position, transform.rotation);
        
        bullet.GetComponent<Bullet>().setPool(rifle_pool);

        return bullet.GetComponent<Bullet>();;
    }
    
    private Bullet createBulletMachine()
    {
        // create new instance
        Bullet bullet = Instantiate(bullet_prefabs[2], transform.position, transform.rotation);
        
        bullet.GetComponent<Bullet>().setPool(machine_pool);

        return bullet.GetComponent<Bullet>();
    }

    private void onTakeBullet(Bullet bullet)
    {
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.is_disabled = false;
        
        bullet.gameObject.SetActive(true);
    }

    private void onReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void onDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
    
    void Start()
    {
        time_since_shot = Time.time;
        if (bullet_prefabs[0] == null)
            Debug.Log("No bullet prefab assigned to weapon");
        current_weapon = WeaponType.PISTOL;
        current_powerup = PowerUp.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        resetWeaponState();
        
        if (!player.game_started)
            return;

        if (player.checkFireRateUpdate() && current_powerup == PowerUp.NONE)
        {
            fire_rate = getWeaponFireRate();
            player.resetUpdateFireRate();
        }

        if ((Time.time - powerup_timer) >= powerup_duration)
            setPowerUp((int)PowerUp.NONE);
        
        if ((Time.time - time_since_shot) >= fire_rate)
        {
            Bullet temp;
            switch (current_powerup)
            {
                case PowerUp.NONE:
                    if ((int)current_weapon == 0)
                        temp = pistol_pool.Get();
                    else if ((int)current_weapon == 1)
                        temp = rifle_pool.Get();
                    else
                        temp = machine_pool.Get();

                    if (temp)
                    {
                        temp.transform.position = transform.position;
                        temp.transform.rotation = transform.rotation;
                    }
                    break;
                case PowerUp.TRI_SHOT:
                    Vector3 rot = transform.rotation.eulerAngles;
                    rot.z -= 10f;
                    for (int i = 0; i < 3; i++)
                    {
                        if ((int)current_weapon == 0)
                            temp = pistol_pool.Get();
                        else if ((int)current_weapon == 1)
                            temp = rifle_pool.Get();
                        else
                            temp = machine_pool.Get();

                        if (temp)
                        {
                            temp.transform.position = transform.position;
                            temp.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
                        }

                        rot.z += 10f;
                    }
                    break;
                case PowerUp.LAZER_BEAM:
                    if ((int)current_weapon == 0)
                        temp = pistol_pool.Get();
                    else if ((int)current_weapon == 1)
                        temp = rifle_pool.Get();
                    else
                        temp = machine_pool.Get();

                    if (temp)
                    {
                        temp.transform.position = transform.position;
                        temp.transform.rotation = transform.rotation;
                    }
                    break;
            }
            time_since_shot = Time.time;
        }
    }

    public void resetWeaponState()
    {
        if (player.current_health <= 0 && !player.game_started)
        {
            current_weapon = WeaponType.PISTOL;
            fire_rate = getWeaponFireRate();
            current_powerup = PowerUp.NONE;
            powerup_timer = 0;
        }
    }

    public void setPowerUp(int power_up)
    {
        powerup_timer = Time.time;
        current_powerup = (PowerUp)power_up;

        if (current_powerup == PowerUp.NONE || current_powerup == PowerUp.TRI_SHOT)
            fire_rate = getWeaponFireRate();
        else if (current_powerup == PowerUp.LAZER_BEAM)
            fire_rate = 0.0f;
    }
    
    public float getWeaponFireRate()
    {
        switch (current_weapon)
        {
            case WeaponType.PISTOL: 
                return 0.25f - (player.getFireRatePoints() * 0.01f);
            case WeaponType.RIFLE:
                return 0.2f - (player.getFireRatePoints() * 0.01f);
            case WeaponType.MACHINE_GUN:
                return 0.15f - (player.getFireRatePoints() * 0.01f);
        }

        return 0.25f - (player.getFireRatePoints() * 0.01f);
    }

    public void upgradeWeapon(int level)
    {
        if (level >= 5 && level < 10)
            current_weapon = WeaponType.RIFLE;
        else if (level >= 10)
            current_weapon = WeaponType.MACHINE_GUN;
        else
            current_weapon = WeaponType.PISTOL;
        
        if (current_powerup == PowerUp.NONE) 
            fire_rate = getWeaponFireRate();
    }

    public int getDamage()
    {
        switch (current_weapon)
        {
            case WeaponType.PISTOL:
                return Random.Range(80, 100 + player.GetComponent<Player>().getLevel()) + (player.getDamagePoints() * 2);
            case WeaponType.RIFLE:
                return Random.Range(95, 115 + player.GetComponent<Player>().getLevel()) + (player.getDamagePoints() * 2);
            case WeaponType.MACHINE_GUN:
                return Random.Range(105, 135 + player.GetComponent<Player>().getLevel()) + (player.getDamagePoints() * 2);
        }
        return 0;
    }
}
