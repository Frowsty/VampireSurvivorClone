using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] MenuController menu_controller;
    [SerializeField] Healthbar health_bar;
    [SerializeField] Expbar exp_bar;
    [SerializeField] GameObject level_text;
    [SerializeField] Sprite[] character_sprites;
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject flashlight;
    [SerializeField] GameObject spotlight;
    [SerializeField] GameObject score_text;
    [SerializeField] TextMeshProUGUI score_tex_value;
    [SerializeField] ExpBallManager exp_ball_manager;
    [SerializeField] MonsterSpawner monster_spawner;

    private Rigidbody2D rb;
    
    private int max_health = 100;

    private float move_speed = 2f;
    private Vector2 next_position = Vector2.zero;
    
    private int player_level = 1;
    private int last_player_level = 0;
    private int experience = 0;
    private float attraction_distance = 0f;

    private int upgrade_points = 0;
    private int attraction_points = 0;
    private int fire_rate_points = 0;
    private int damage_points = 0;
    private int regen_points = 0;

    private float internal_regen_health = 0f;

    private bool update_fire_rate = false;

    private int score = 0;

    /*
     * PUBLIC VARIABLES
     */  
    public bool game_started = false;
    public int current_health;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        current_health = max_health;
        health_bar.setMaxHealth(max_health);
        gameObject.GetComponent<SpriteRenderer>().sprite = character_sprites[0];
        rb = GetComponent<Rigidbody2D>();

        resetScore();
        
        score_tex_value.SetText("0");
        score_text.SetActive(false);
        
        next_position = transform.position;
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
    }

    private void FixedUpdate()
    {
        if (game_started)
        {
            if (!score_text.activeSelf)
                score_text.SetActive(true);
            
            regen_health();
            
            if (current_health <= 0)
            {
                if (score > PlayerPrefs.GetInt("highscore"))
                {
                    PlayerPrefs.SetInt("highscore", score);
                    PlayerPrefs.Save();
                }
                
                score_text.SetActive(false);

                menu_controller.setMenuState(MenuController.MenuState.DeathMenu); // Menu state 1 = death menu
                menu_controller.toggleMenu(true);
                game_started = false;
            }
            else
                movePlayer();
        }
    }

    private void movePlayer()
    {
        // get next position to move towards when player presses LMB
        if (Input.GetMouseButton(0))
        {
            Vector3 target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = target_pos - transform.position;
            float look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, look_angle);

            target_pos.z = -3;

            next_position = target_pos;
        }
        
        // while player is holding RMB allow them to look around / rotate their player towards the mouse pointer
        if (Input.GetMouseButton(1))
        {
            Vector3 target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = target_pos - transform.position;
            float look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, look_angle);
        }
        
        rb.MovePosition(Vector2.MoveTowards(rb.position, next_position, move_speed * Time.deltaTime));
    }
    
    private void giveRandomUpgrade() => GameObject.Find("Weapon").GetComponent<Weapon>().setPowerUp(Random.Range((int)1, (int)4));


    /*
     * PUBLIC FUNCTIONS
     */
    public void regen_health()
    {
        if (current_health == max_health || regen_points == 0)
            return;
        
        internal_regen_health += (regen_points * 0.1f) * Time.deltaTime;

        if (internal_regen_health >= 1)
        {
            health_bar.setHealth(current_health++);
            internal_regen_health = 0;
        }
    }

    public void increaseExp(int amount)
    {
        experience += amount;
        
        if (experience >= 100 * player_level)
        {
            player_level++;
            exp_bar.setMaxExp(100 * player_level);
            level_text.GetComponent<TMPro.TextMeshProUGUI>().SetText(player_level.ToString());

            monster_spawner.updateMaxSpawn();
            
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
        
        increaseScore(amount);
        updateScoreText();
        
        exp_bar.setExp(experience);
    }

    public void updateFireRate() => update_fire_rate = true;
    public void resetUpdateFireRate() => update_fire_rate = false;
    public bool checkFireRateUpdate() => update_fire_rate;
    
    public float getAttractionDistance() => attraction_distance + (attraction_points * 0.2f);
    public void setAttractionPoints(int points) => attraction_points = points;

    public int getFireRatePoints() => fire_rate_points;
    public void setFireRatePoints(int points) => fire_rate_points = points;
    
    public int getHealthRegenPoints() => regen_points; // not used but if it's ever needed we have it here
    public void setHealthRegen(int points) => regen_points = points;

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

    public int getScore() => score;
    
    public void increaseScore(int points) => score += points;

    public void resetScore() => score = 0;
    
    public void updateScoreText() => score_tex_value.SetText(score.ToString());
    
    public void resetScoreText() => score_tex_value.SetText("");
    
    /*
     * COLLISION CALLBACKS
     */
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
            increaseExp(collision.gameObject.GetComponent<ExpOrb>().getExp());
            exp_ball_manager.getPool().Release(collision.gameObject.GetComponent<ExpOrb>());
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
            increaseExp(collision.gameObject.GetComponent<ExpOrb>().getExp());
            exp_ball_manager.getPool().Release(collision.gameObject.GetComponent<ExpOrb>());
        }
    }
}
