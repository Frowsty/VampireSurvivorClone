using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBallMovement : MonoBehaviour
{
    [SerializeField] Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.game_started)
            return;
        if (Vector3.Distance(player.transform.position, transform.position) <= player.getAttractionDistance())
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 5f);
    }
}
