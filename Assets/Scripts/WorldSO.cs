﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/World", order = 1)]
public class WorldSO : ScriptableObject
{
    public int ammountOfSpawns = 200;
    public List<Enemy> allEnemiesData;

    private List<EnemyManager> aliveEnemies;

    public List<EnemyManager> AliveEnemies { get { return aliveEnemies; } }

    private void OnEnable()
    {
        aliveEnemies = new List<EnemyManager>();
    }
    
    private void OnEnemyDeath(EnemyManager enemy)
    {
        RemoveEnemy(enemy);
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