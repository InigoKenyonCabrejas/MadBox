using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersFloater : MonoBehaviour
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
    
    public void ShowDamageFloater(Vector3 pos,int dmg, bool isEnemyDamage)
    {
        DamageFloater damageFloater = damageFloaterPool.Pull();

        if(damageFloater == null)
        {
            damageFloater = Instantiate(damageFloaterPrefab).GetComponent<DamageFloater>();
        }
        
        damageFloater.transform.SetParent(floatersContainer);
        damageFloater.transform.position = pos;
        
        damageFloater.FinishedFloatAction += OnDamageFloaterFinished;
        dmg *= -1;
        damageFloater.DisplayDamage(dmg.ToString(), pos, isEnemyDamage);
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
