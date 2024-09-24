using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] DamagePopup damage_prefab;
    
    private Weapon weapon;
    private Player player;
    private Rigidbody2D rb;
    
    private float bullet_speed = 6.0f;
    private float travel_time;

    private Vector3 target_pos;

    private ObjectPool<Bullet> bullet_pool;

    public bool is_disabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        weapon = GameObject.Find("Weapon").GetComponent<Weapon>();
        player = GameObject.Find("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
    
    public void setPool(ObjectPool<Bullet> pool) => bullet_pool = pool;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && player.game_started && !is_disabled)
        {
            if (collision.gameObject.activeSelf)
            {
                int damage = weapon.getDamage();
                collision.GetComponent<Monster>().takeDamage(damage);
                DamagePopup damage_popup = Instantiate(damage_prefab, new Vector3(collision.gameObject.transform.position.x - 0.5f,
                                                                                 collision.gameObject.transform.position.y - 0.25f, 0),
                                                                                 Quaternion.identity);
                damage_popup.Setup(damage);
            }

            is_disabled = true;
            bullet_pool.Release(this);
            travel_time = 0;
        }
    }
}
