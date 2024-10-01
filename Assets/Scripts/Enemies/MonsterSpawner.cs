using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] float base_spawn_delay;
    
    private Monster monster_prefab;
    private Monster strong_monster_prefab;
    private Monster boss_prefab;
    
    private ObjectPool<Monster> monster_pool;
    private ObjectPool<Monster> strong_monster_pool;
    private ObjectPool<Monster> boss_pool;

    private Player player;

    private int max_spawn = 100;

    private int spawned_monsters = 0;
    
    private float spawn_timer = 0f;

    private bool did_spawn = false;
    
    
    /*
     * PUBLIC VARIABLES
     */
    public ExpBallManager exp_ball_manager;

    /*
     * PRIVATE FUNCTIONS
     */
    private void Awake()
    {
        monster_prefab = Resources.Load<Monster>("Monster");
        strong_monster_prefab = Resources.Load<Monster>("StrongMonster");
        boss_prefab = Resources.Load<Monster>("Boss");
        
        if (!monster_prefab || !strong_monster_prefab || !boss_prefab)
            Debug.Log("ERROR: Prefabs do not have a monster prefab!");
        monster_pool = new ObjectPool<Monster>(createMonster, onTakeMonster, onReturnMonster, onDestroyMonster, true, 200, 1500);
        boss_pool = new ObjectPool<Monster>(createBoss, onTakeBoss, onReturnMonster, onDestroyMonster, true, 10, 100);
        strong_monster_pool = new ObjectPool<Monster>(createStrongMonster, onTakeStrongMonster, onReturnMonster, onDestroyMonster, true, 50, 500);
    }
    
    void Start() => player = GameObject.Find("Player").GetComponent<Player>();

    void Update() => spawnMonsters();
    
    private Monster createBoss()
    {
        Monster monster = Instantiate(boss_prefab, pointOutsideScreen(), Quaternion.identity);
        
        monster.setPool(boss_pool);

        return monster;
    }

    private void onTakeBoss(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.setHealth(monster.getMaxHealth() + (player.getLevel() * 20));
        
        monster.gameObject.SetActive(true);
    }
    
    private Monster createStrongMonster()
    {
        Monster monster = Instantiate(strong_monster_prefab, pointOutsideScreen(), Quaternion.identity);
        
        monster.setPool(boss_pool);

        return monster;
    }

    private void onTakeStrongMonster(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.setHealth(monster.getMaxHealth() + (player.getLevel() * 20));
        
        monster.gameObject.SetActive(true);
    }

    private Monster createMonster()
    {
        // create new instance
        Monster monster = Instantiate(monster_prefab, pointOutsideScreen(), Quaternion.identity);
        
        monster.setPool(monster_pool);

        return monster;
    }

    private void onTakeMonster(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.setHealth(monster.getMaxHealth() + (player.getLevel() * 10));
        
        monster.gameObject.SetActive(true);
    }

    private void onReturnMonster(Monster monster) => monster.gameObject.SetActive(false);

    private void onDestroyMonster(Monster monster) => Destroy(monster.gameObject);
    
    private Vector3 pointOutsideScreen()
    {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        if (x >= 0f) x += 1f;
        if (y >= 0f) y += 1f;
         
        Vector3 random_point = new Vector3(x, y);
        Vector3 world_point = Camera.main.ViewportToWorldPoint(random_point);

        world_point.z = -2;
        
        return world_point;
    }

    private void spawnMonsters()
    {
        if (!player.game_started)
            return;
        if (spawned_monsters < max_spawn && !did_spawn)
        {
            if (Random.Range(0f, 100f) < 2.5f)
                boss_pool.Get();
            else if (Random.Range(0f, 100f) < 10f)
                strong_monster_pool.Get();
            else
                monster_pool.Get();
            increaseSpawnedMonsters();

            spawn_timer = 0f;
        }
        else
            did_spawn = true;

        if (spawn_timer >= get_spawn_time_limit() && did_spawn)
            did_spawn = false;
        
        if (did_spawn)
            spawn_timer += Time.deltaTime;
    }
    
    float get_spawn_time_limit() => (base_spawn_delay - player.getLevel()) > 5f ? base_spawn_delay - player.getLevel() : 5f;
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void increaseSpawnedMonsters() => spawned_monsters += 1;

    public void decreaseSpawnedMonsters() => spawned_monsters -= 1;

    public void updateMaxSpawn() => max_spawn = max_spawn + (player.getLevel() * 10);
}
