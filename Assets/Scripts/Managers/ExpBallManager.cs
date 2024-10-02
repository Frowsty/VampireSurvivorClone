using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ExpBallManager : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] ExpOrb exp_ball;
    ObjectPool<ExpOrb> exp_pool;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        exp_pool = new ObjectPool<ExpOrb>(createExp, onTakeExp, onReturnExp, onDestroyExp, true, 200, 1500);
    }
    
    private ExpOrb createExp()
    {
        ExpOrb exp = Instantiate(exp_ball, transform.position, Quaternion.identity);
        return exp;
    }

    private void onTakeExp(ExpOrb exp)
    {
        exp.transform.rotation = Quaternion.identity;
        
        exp.gameObject.SetActive(true);
    }
    
    private void onReturnExp(ExpOrb exp)
    {
        exp.gameObject.SetActive(false);
        exp.resetTimer();
    }

    private void onDestroyExp(ExpOrb exp)
    {
        Destroy(exp.gameObject);
    }

    /*
     * PUBLIC FUNCTIONS
     */
    public ObjectPool<ExpOrb> getPool() => exp_pool;
}
