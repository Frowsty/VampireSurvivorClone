using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    private Bullet bullet;
    
    private void Start()
    {
        bullet = transform.parent.GetComponent<Bullet>();
    }

    /*
     * COLLISION CALLBACKS
     */
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && bullet.player.game_started && !bullet.is_disabled)
        {
            //Debug.Log("Entered magnetic field");
            if (collision.gameObject.activeSelf)
            {
                if (bullet.getFinalAngle() == 0)
                {
                    Vector3 target_pos = collision.gameObject.transform.position;
                    Vector3 rotation = target_pos - transform.position;
                    float look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                    bullet.setFinalAngle(look_angle);
                }
            }
        }
    }
}
