using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] Monster monster_prefab;
    [SerializeField] Monster boss_prefab;

    private ObjectPool<Monster> object_pool;
    private ObjectPool<Monster> boss_pool;

    private Player player;

    private int max_spawn = 100;

    private int spawned_monsters = 0;
    
    private float spawn_timer = 0f;

    private bool did_spawn = false;

    private void Awake()
    {
        object_pool = new ObjectPool<Monster>(createMonster, onTakeMonster, onReturnMonster, onDestroyMonster, true, 200, 1500);
        boss_pool = new ObjectPool<Monster>(createBoss, onTakeBoss, onReturnBoss, onDestroyBoss, true, 10, 100);
    }
    
    private Monster createBoss()
    {
        Monster monster = Instantiate(boss_prefab, pointOutsideScreen(), Quaternion.identity);
        
        monster.GetComponent<Monster>().setPool(boss_pool);
        monster.GetComponent<Monster>().setBossState(true);

        return monster;
    }

    private void onTakeBoss(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.GetComponent<Monster>().setHealth(2000 + (player.getLevel() * 20));
        
        monster.gameObject.SetActive(true);
    }
    
    private void onReturnBoss(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void onDestroyBoss(Monster monster)
    {
        Destroy(monster.gameObject);
    }

    private Monster createMonster()
    {
        // create new instance
        Monster monster = Instantiate(monster_prefab, pointOutsideScreen(), Quaternion.identity);
        
        monster.GetComponent<Monster>().setPool(object_pool);

        return monster;
    }

    private void onTakeMonster(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.GetComponent<Monster>().setHealth(100 + (player.getLevel() * 10));
        
        monster.gameObject.SetActive(true);
    }

    private void onReturnMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void onDestroyMonster(Monster monster)
    {
        Destroy(monster.gameObject);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnMonsters();
    }
    
    private Vector3 pointOutsideScreen()
    {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        if (x >= 0f) x += 1f;
        if (y >= 0f) y += 1f;
         
        Vector3 random_point = new Vector3(x, y);
        Vector3 world_point = Camera.main.ViewportToWorldPoint(random_point);

        world_point.z = 0;
        
        return world_point;
    }

    private void spawnMonsters()
    {
        if (!player.game_started)
            return;
        if (spawned_monsters < (max_spawn + (player.getLevel() * 10)) && !did_spawn)
        {
            if (Random.Range(0f, 100f) < 2.5f)
                boss_pool.Get();
            else
                object_pool.Get();
            increaseSpawnedMonsters();

            spawn_timer = 0f;
        }
        else
            did_spawn = true;

        if (spawn_timer >= 35f && did_spawn)
            did_spawn = false;
        
        if (did_spawn)
            spawn_timer += Time.deltaTime;
    }
    
    public void increaseSpawnedMonsters() => spawned_monsters += 1;

    public void decreaseSpawnedMonsters() => spawned_monsters -= 1;
}
