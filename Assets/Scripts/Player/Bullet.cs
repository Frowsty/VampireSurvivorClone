using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] DamagePopup damage_prefab;
    [SerializeField] Sprite[] bullet_sprites;
    
    private Weapon weapon;
    private Player player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    private float bullet_speed = 6.0f;
    private float travel_time;

    private Vector3 target_pos;

    private ObjectPool<Bullet> bullet_pool;

    /*
     * PUBLIC VARIABLES
     */
    public bool is_disabled = false;

    
    /*
     * PRIVATE FUNCTIONS
     */
    private void Awake()
    {
        weapon = GameObject.Find("Weapon").GetComponent<Weapon>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!player.game_started)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (travel_time > 1.5)
        {
            bullet_pool.Release(this);
            travel_time = 0;
            return;
        }

        rb.velocity = transform.right * bullet_speed;
        
        travel_time += Time.deltaTime;
    }
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void setPool(ObjectPool<Bullet> pool) => bullet_pool = pool;

    public void updateSprite()
    {
        int player_level = player.getLevel();
        
        if (player_level >= 1 && player_level < 5)
            sr.sprite = bullet_sprites[0];
        else if (player_level >= 5 && player_level < 10)
            sr.sprite = bullet_sprites[1];
        else
            sr.sprite = bullet_sprites[2];
    }

    /*
     * COLLISION CALLBACKS
     */
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && player.game_started && !is_disabled)
        {
            if (collision.gameObject.activeSelf)
            {
                int damage = weapon.getDamage();
                collision.GetComponent<Monster>().takeDamage(damage);
                DamagePopup damage_popup = Instantiate(damage_prefab, new Vector3(collision.gameObject.transform.position.x - 0.5f,
                                                                                 collision.gameObject.transform.position.y - 0.25f, -3),
                                                                                 Quaternion.identity);
                damage_popup.Setup(damage);
            }

            is_disabled = true;
            bullet_pool.Release(this);
            travel_time = 0;
        }
    }
}
