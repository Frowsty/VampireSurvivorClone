using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] MenuController menu_controller;
    [SerializeField] Healthbar health_bar;
    [SerializeField] Expbar exp_bar;
    [SerializeField] GameObject level_text;
    [SerializeField] Sprite[] character_sprites;
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject flashlight;
    [SerializeField] GameObject spotlight;

    private Rigidbody2D rb;
    
    public bool game_started = false;
    private int max_health = 100;
    public int current_health;
    private float move_speed = 2.0f;
    private int player_level = 1;
    private int last_player_level = 0;
    private int experience = 0;
    private float attraction_distance = 0f;

    private int upgrade_points = 100;
    private int attraction_points = 0;
    private int fire_rate_points = 0;
    private int damage_points = 0;

    private bool update_fire_rate = false;

    // Start is called before the first frame update
    void Start()
    {
        current_health = max_health;
        health_bar.setMaxHealth(max_health);
        gameObject.GetComponent<SpriteRenderer>().sprite = character_sprites[0];
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && menu_controller.allow_bind)
        {
            game_started = !game_started;
            
            menu_controller.setMenuState(MenuController.MenuState.PlayMenu);
            menu_controller.toggleMenu(!game_started);
        }

        if (game_started)
            movePlayer();
        else
            rb.velocity = Vector2.zero;
    }

    private void movePlayer()
    {
        if (current_health <= 0)
        {
            menu_controller.setMenuState(MenuController.MenuState.DeathMenu); // Menu state 1 = death menu
            menu_controller.toggleMenu(true);
            game_started = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = target_pos - transform.position;
            float look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, look_angle);

            target_pos.z = 0;
            rb.velocity = transform.right * move_speed;
        }
        else
            rb.velocity = Vector2.zero;
        
        if (Input.GetMouseButton(1))
        {
            Vector3 target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = target_pos - transform.position;
            float look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, look_angle);
        }
    }

    public void increaseExp(bool is_boss)
    {
        if (experience >= 100 * player_level)
        {
            player_level++;
            exp_bar.setMaxExp(100 * player_level);
            level_text.GetComponent<TMPro.TextMeshProUGUI>().SetText(player_level.ToString());
            
            experience = 0;
            upgrade_points += 2;
            
            if (player_level == 5 || player_level == 10)
                if (last_player_level == 3 || last_player_level == 8)
                    weapon.upgradeWeapon(player_level);

            switch (player_level)
            {
                case 5:
                    gameObject.GetComponent<SpriteRenderer>().sprite = character_sprites[1];
                    break;
                case 10:
                    gameObject.GetComponent<SpriteRenderer>().sprite = character_sprites[2];
                    break;
            }
            last_player_level++;
            
            
            // if we level up, toggle menu and set our menu state to upgrade menu
            menu_controller.toggleMenu(true);
            menu_controller.setMenuState(MenuController.MenuState.UpgradeMenu);
            game_started = !game_started;
        }
        else
            experience += is_boss ? 75 : 25;
        
        exp_bar.setExp(experience);
    }

    public void updateFireRate() => update_fire_rate = true;
    public void resetUpdateFireRate() => update_fire_rate = false;
    public bool checkFireRateUpdate() => update_fire_rate;
    
    public float getAttractionDistance() => attraction_distance + (attraction_points * 0.1f);
    public void setAttractionPoints(int points) => attraction_points = points;

    public int getFireRatePoints() => fire_rate_points;
    public void setFireRatePoints(int points) => fire_rate_points = points;

    public int getDamagePoints() => damage_points;
    public void setDamagePoints(int points) => damage_points = points;
    public void takeDamage(int damage)
    {
        if (current_health <= 0)
            return;
        current_health -= damage;
        health_bar.setHealth(current_health);
    }

    public int getLevel() => player_level;

    public int getUpgradePoints() => upgrade_points;

    public void decrementUpgradePoints()
    {
        if (upgrade_points > 0)
            upgrade_points--;
    }

    public void incrementUpgradePoints() => upgrade_points++;
    
    private void giveRandomUpgrade() => GameObject.Find("Weapon").GetComponent<Weapon>().setPowerUp(Random.Range((int)1, (int)3));
    

    public void disableLights()
    {
        flashlight.SetActive(false);
        spotlight.SetActive(false);
    }

    public void enableLights()
    {
        flashlight.SetActive(true);
        spotlight.SetActive(true);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!game_started)
            return;
        if (collision.CompareTag("Enemy"))
            takeDamage(collision.gameObject.GetComponent<Monster>().doDamage());
        
        if (collision.CompareTag("Powerup"))
        {
            giveRandomUpgrade();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("ExpOrb"))
        {
            increaseExp(false);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("ExpOrbBoss"))
        {
            increaseExp(true);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!game_started)
            return;
        
        if (collision.CompareTag("Powerup"))
        {
            giveRandomUpgrade();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("ExpOrb"))
        {
            increaseExp(false);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("ExpOrbBoss"))
        {
            increaseExp(true);
            Destroy(collision.gameObject);
        }
    }
}
