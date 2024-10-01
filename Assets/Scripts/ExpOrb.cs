using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] Player player;

    private ObjectPool<ExpOrb> pool;

    private int experience = 0;

    private float timer = 0f;
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
        if (timer >= 35f)
            pool.Release(this);

        timer += Time.deltaTime;
        
        if (Vector3.Distance(player.transform.position, transform.position) <= player.getAttractionDistance())
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 5f);
    }

    public void setPool(ObjectPool<ExpOrb> pool) => this.pool = pool;
    
    /*
     * PUBLIC FUNCTIONS
     */
    public void resetTimer() => timer = 0f;
    
    public int getExp() => experience;

    public void setExp(int exp) => experience = exp;
}
