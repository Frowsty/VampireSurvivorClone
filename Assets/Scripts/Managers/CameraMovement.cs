using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] GameObject target;
    private Vector3 target_direction;
    private Vector3 target_pos;
    private float interp_velocity;

    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        target_pos = transform.position;
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 pos = transform.position;
            pos.z = target.transform.position.z;
            target_direction = (target.transform.position - pos);
            
            interp_velocity = target_direction.magnitude * 5f;
            target_pos = transform.position + ((target_direction.normalized * interp_velocity) * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, target_pos, 1f);
        }
    }
}
