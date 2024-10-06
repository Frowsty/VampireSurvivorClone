using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] Bullet bullet_prefab;
    [SerializeField] Player player;
    
    private ObjectPool<Bullet> bullet_pool;
    private CustomPowerUpInfo powerup_info;

    private WeaponType current_weapon;
    private PowerUp current_powerup;

    private Vector3 direction;
    
    private float time_since_shot = 0;
    private float fire_rate = 0.25f;
    private float powerup_timer = 0;
    private float powerup_duration = 30f;
    
    // used for bullet z rotation offset
    private Vector3 rot;
 
    /*
     * PUBLIC VARIABLES
     */
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
        LAZER_BEAM,
        CUSTOM
    }
    
    /*
     * PRIVATE FUNCTIONS
     */
    private Bullet createBullet()
    {
        // create new instance
        Bullet bullet = Instantiate(bullet_prefab, transform.position, transform.rotation);
        
        bullet.setPool(bullet_pool);
        bullet.updateSprite();

        return bullet;
    }

    private void onTakeBullet(Bullet bullet)
    {
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y, -3); // -3 since bullets should be ontop of everything
        bullet.transform.rotation = transform.rotation;
        bullet.is_disabled = false;
        
        bullet.updateSprite();
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
        powerup_info = Resources.Load<CustomPowerUpInfo>("CustomPowerUp");
        if (!powerup_info)
            Debug.LogError("powerup_info not loaded");
        bullet_pool = new ObjectPool<Bullet>(createBullet, onTakeBullet, onReturnBullet, onDestroyBullet, true, 1000, 3000);
        time_since_shot = Time.time;
        current_weapon = WeaponType.PISTOL;
        current_powerup = PowerUp.NONE;;
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
                case PowerUp.LAZER_BEAM:
                case PowerUp.NONE:
                    bullet_pool.Get();
                    break;
                case PowerUp.TRI_SHOT:
                    rot = transform.rotation.eulerAngles;
                    rot.z -= 10f;
                    for (int i = 0; i < 3; i++)
                    {
                        temp = bullet_pool.Get();
                        if (temp)
                            temp.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);

                        rot.z += 10f;
                    }
                    break;
                case PowerUp.CUSTOM:
                    rot = transform.rotation.eulerAngles;
                    rot.z -= (powerup_info.bullet_count % 2 == 0 ? powerup_info.bullet_count / 2 : (powerup_info.bullet_count - 1) / 2) * powerup_info.bullet_rotation;
                    for (int i = 0; i < powerup_info.bullet_count; i++)
                    {
                        temp = bullet_pool.Get();
                        if (temp)
                            temp.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);

                        rot.z += powerup_info.bullet_rotation;
                    }
                    break;
            }
            time_since_shot = Time.time;
        }
    }

    /*
     * PUBLIC FUNCTIONS
     */
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
        else if (current_powerup == PowerUp.CUSTOM)
            fire_rate = powerup_info.fire_rate;
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
            current_weapon = WeaponType.RIFLE; // pistol uses standard bullet sprite already set as default during setup
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
