using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject damageFloaterPrefab;
    [SerializeField] private Transform poolContainer;
    [SerializeField] private Transform floatersContainer;
    
    private DamageFloaterPool damageFloaterPool;
    
    private void Awake()
    {
        damageFloaterPool = new DamageFloaterPool(poolContainer);
    }

    private void OnDamageFloaterFinished(DamageFloater damageFloater)
    {
        damageFloaterPool.Push(damageFloater);
        
        damageFloater.FinishedFloatAction -= OnDamageFloaterFinished;
    }
    
    public void ShowDamageFloater(EnemyManager enemy,int dmg)
    {
        DamageFloater damageFloater = damageFloaterPool.Pull();

        if(damageFloater == null)
        {
            damageFloater = Instantiate(damageFloaterPrefab).GetComponent<DamageFloater>();
        }
        
        damageFloater.transform.SetParent(floatersContainer);
        damageFloater.transform.position = enemy.transform.position;
        
        damageFloater.FinishedFloatAction += OnDamageFloaterFinished;
        dmg *= -1;
        damageFloater.DisplayDamage(dmg.ToString(), enemy.transform.position);
    }
}

public class DamageFloaterPool
{
    private Queue<DamageFloater> queue;
    private Transform container;

    public DamageFloaterPool(Transform _container)
    {
        queue = new Queue<DamageFloater>();
        container = _container;
    }

    public void Push(DamageFloater damageFloater)
    {
        damageFloater.gameObject.SetActive(false);
        damageFloater.transform.SetParent(container);
        queue.Enqueue(damageFloater);
    }

    public DamageFloater Pull()
    {
        if(queue.Count == 0)
        {
            return null;
        }
        
        return queue.Dequeue();
    }
}
