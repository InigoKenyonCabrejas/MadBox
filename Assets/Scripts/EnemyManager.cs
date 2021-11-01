using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy enemyData;
     

    public Action<EnemyManager> EnemyDeathAction;

    private void OnDestroy()
    {
        EnemyDeathAction?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyData.health--;
        if(enemyData.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Enemy _enemyData)
    {
        enemyData = _enemyData;
    }
}