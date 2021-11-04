using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/World", order = 1)]
public class WorldSO : ScriptableObject
{
    [SerializeField] private ArmorySO armory;

    public int ammountOfSpawns = 200;
    public List<Enemy> allEnemiesData;

    private bool isWorldSetup;

    public bool IsWorldSetup
    {
        get
        {
            return isWorldSetup;
        }
        set
        {
            isWorldSetup = value;
            if(isWorldSetup)
            {
                WorldIsSetupAction?.Invoke();
            }
        }
    }

    private List<EnemyManager> aliveEnemies;
    public List<EnemyManager> AliveEnemies { get { return aliveEnemies; } }

    public Action WorldIsSetupAction;
    public Action<Weapon, Vector3> LootDroppedAction;

    private void OnEnable()
    {
        IsWorldSetup = false;
        aliveEnemies = new List<EnemyManager>();
    }
    
    private void OnEnemyDeath(EnemyManager enemy)
    {
        RemoveEnemy(enemy);
        LootDroppedAction?.Invoke(armory.GetRandomWeapon(), enemy.transform.position);
    }

    public Enemy GetEnemyData(EnemyType enemyType)
    {
        for(int i = 0; i < allEnemiesData.Count; i++)
        {
            if(enemyType == allEnemiesData[i].enemyType)
            {
                return allEnemiesData[i];
            }
        }

        return null;
    }

    public void AddEnemy(EnemyManager enemy)
    {
        aliveEnemies.Add(enemy);
        enemy.EnemyDeathAction += OnEnemyDeath;
    }

    public void RemoveEnemy(EnemyManager enemy)
    {
        aliveEnemies.Remove(enemy);
        enemy.EnemyDeathAction -= OnEnemyDeath;
    }
}