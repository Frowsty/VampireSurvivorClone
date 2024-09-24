using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject target;
    
    public float interp_velocity;
    public float min_distance;
    public float follow_distance;
    public Vector3 offset;
    Vector3 target_pos;
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

            Vector3 target_direction = (target.transform.position - pos);

            interp_velocity = target_direction.magnitude * 5f;

            target_pos = transform.position + ((target_direction.normalized * interp_velocity) * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, target_pos + offset, 1f);

        }
    }
}
