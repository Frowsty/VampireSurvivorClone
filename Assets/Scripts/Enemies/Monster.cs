using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Pool;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Monster : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion_prefab;
    [SerializeField] GameObject powerup_prefab;
    //[SerializeField] GameObject exp_orb;
    
    private const float CONST_MOVE_SPEED = 0.75f;
    private const float CONST_MOVE_SPEED_WITH_BOOST = 2f;
    
    private ObjectPool<Monster> monster_pool;
    private ParticleSystem death_explosion;
    private Player player;
    private MonsterSpawner monster_spawner;
    private Rigidbody2D rb;
    private GameObject bullet;
    
    public int current_health = 100;
    private int damage = 3;
    private float move_speed = CONST_MOVE_SPEED;

    private bool evading = false;
    private bool did_evade = false;
    private float evade_duration = 0f;

    private bool is_boss = false;

    private float look_angle;
    private int[] dodge_angles = { -45, 45 };
    
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        monster_spawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (!player.game_started)
        {
            rb.velocity = Vector3.zero;
            rb.rotation = look_angle;
            return;
        }
        
        Vector3 target_pos = player.transform.position;
        Vector3 rotation = target_pos - transform.position;

        if (!evading)
        {
            look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            move_speed = CONST_MOVE_SPEED;
        }

        evadeBullet();
        
        rb.rotation = look_angle;

        if (Vector3.Distance(transform.position, target_pos) < 14f)
            rb.velocity = transform.right * move_speed;
    }

    public void evadeBullet()
    {
        if (bullet && !evading && !did_evade)
        {
            look_angle += dodge_angles[Random.Range((int)0, (int)2)];
            move_speed = CONST_MOVE_SPEED_WITH_BOOST;
            evading = true;
        }

        if (evade_duration >= 0.5f)
        {
            bullet = null;
            evade_duration = 0;
            evading = false;
            did_evade = true;
        }
        
        if (evading)
            evade_duration += Time.deltaTime;
    }

    public void setPool(ObjectPool<Monster> pool) => monster_pool = pool;
    
    public int doDamage()
    {
        return is_boss ? damage + 5 : damage;
    }
    
    public void setHealth(int health) => current_health = health;

    public void setBossState(bool state) => is_boss = state;

    public void takeDamage(int damage)
    {
        current_health -= damage;

        if (current_health <= 0)
            onDeath();
    }

    private void resetStates()
    {
        did_evade = false;
        evading = false;
    }

    private void onDeath()
    {
        // particle system when monster dies
        if (!death_explosion)
        {
            death_explosion = Instantiate(explosion_prefab, transform.position, Quaternion.identity);
            death_explosion.transform.position = transform.position;
            death_explosion.Play();
        }

        monster_pool.Release(this);
        monster_spawner.decreaseSpawnedMonsters();
        
        player.increaseScore(1);
        player.updateScoreText();

        resetStates();
        
        if (Random.Range(0f, 100f) < 0.5f) // 0.5% chance to spawn a powerup drop
            Instantiate(powerup_prefab, transform.position, Quaternion.identity);

        ExpBallMovement exp_ball = monster_spawner.exp_ball_manager.getPool().Get();
        exp_ball.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        exp_ball.setPool(monster_spawner.exp_ball_manager.getPool());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("bullet"))
            if (!bullet)
                bullet = collider.gameObject;
    }
    
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("bullet"))
            bullet = null;
    }
}
