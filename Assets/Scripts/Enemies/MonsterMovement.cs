using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    private Player player;
    private Rigidbody2D rb;
    private GameObject bullet;
    
    private bool evading = false;
    private bool did_evade = false;
    private float evade_duration = 0f;
    
    private float look_angle;
    private int[] dodge_angles = { -45, 45 };
    
    private float move_speed = 0.75f;
    
    /*
     * PUBLIC VARIABLES
     */
    
    public float CONST_MOVE_SPEED = 0.75f;

    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        player = GetComponent<Monster>().getPlayer();
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

    private void evadeBullet()
    {
        if (bullet && !evading && !did_evade)
        {
            look_angle += dodge_angles[Random.Range((int)0, (int)2)];
            move_speed = CONST_MOVE_SPEED + 1.25f;
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
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void resetStates()
    {
        did_evade = false;
        evading = false;
    }
    
    /*
     * COLLISION CALLBACKS
     */
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
