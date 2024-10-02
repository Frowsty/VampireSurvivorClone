using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    private TextMeshPro text_mesh;

    private float text_move_speed = 0.5f;

    private float spawn_time = 0f;
    
    /*
     * PRIVATE FUNCTIONS
     */
    private void Awake() => text_mesh = transform.GetComponent<TextMeshPro>();
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void Setup(int damage_amount) => text_mesh.SetText(damage_amount.ToString());
    public void Start() => spawn_time = Time.time;
    public void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + text_move_speed * Time.deltaTime, -3);
        
        if (Time.time - spawn_time > 1.0f)
            Destroy(gameObject);
    }
}
