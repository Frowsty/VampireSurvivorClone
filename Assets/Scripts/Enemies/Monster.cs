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
    /*
     * PRIVATE VARIABLES
     */
    private ParticleSystem explosion_prefab;
    private GameObject powerup_prefab;
    
    private ObjectPool<Monster> monster_pool;
    private ParticleSystem death_explosion;
    private Player player;
    private MonsterSpawner monster_spawner;
    
    /*
     * PUBLIC VARIABLES
     */
    public int health;
    public int current_health = 0;
    public int damage = 3;

    public int experience = 0;

    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        explosion_prefab = Resources.Load<ParticleSystem>("DeathParticles");
        powerup_prefab = Resources.Load<GameObject>("PowerUp");
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        monster_spawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();
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

        GetComponent<MonsterMovement>().resetStates();
        
        if (Random.Range(0f, 100f) < 0.5f) // 0.5% chance to spawn a powerup drop
            Instantiate(powerup_prefab, transform.position, Quaternion.identity);

        ExpOrb exp_orb = monster_spawner.exp_ball_manager.getPool().Get();
        exp_orb.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        exp_orb.setPool(monster_spawner.exp_ball_manager.getPool());
        exp_orb.setExp(experience);
    }
    
    /*
     *PUBLIC FUNCTIONS
     */
    public Player getPlayer() => player;
    
    public void takeDamage(int damage)
    {
        current_health -= damage;

        if (current_health <= 0)
            onDeath();
    }
    
    public void setPool(ObjectPool<Monster> pool) => monster_pool = pool;
    
    public int doDamage()
    {
        return damage;
    }
    
    public void setHealth(int value) => current_health = value;
    
    public void setMaxHealth(int value) => health = value;
    
    public int getMaxHealth() => health;

    public int getExp() => experience;
}
