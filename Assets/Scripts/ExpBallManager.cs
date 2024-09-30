using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ExpBallManager : MonoBehaviour
{
    [SerializeField] ExpBallMovement exp_ball;
    ObjectPool<ExpBallMovement> exp_pool;
    
    void Start()
    {
        exp_pool = new ObjectPool<ExpBallMovement>(createExp, onTakeExp, onReturnExp, onDestroyExp, true, 200, 1500);
    }
    
    private ExpBallMovement createExp()
    {
        ExpBallMovement exp = Instantiate(exp_ball, transform.position, Quaternion.identity);
        return exp;
    }

    private void onTakeExp(ExpBallMovement exp)
    {
        exp.transform.rotation = Quaternion.identity;
        
        exp.gameObject.SetActive(true);
    }
    
    private void onReturnExp(ExpBallMovement exp)
    {
        exp.gameObject.SetActive(false);
        exp.resetTimer();
    }

    private void onDestroyExp(ExpBallMovement exp)
    {
        Destroy(exp.gameObject);
    }

    public ObjectPool<ExpBallMovement> getPool() => exp_pool;

    // Update is called once per frame
    void Update()
    {
        
    }
}
