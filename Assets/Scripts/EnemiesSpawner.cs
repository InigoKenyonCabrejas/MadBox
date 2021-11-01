using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private WorldSO worldData;
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private Transform topLeftCorner;
    [SerializeField] private Transform bottomRightCorner;
    [SerializeField] private List<EnemyType> enemyTypesToSpawn;
    
    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        GameObject enemyObj;
        Enemy enemyData;
        EnemyManager enemy;
        EnemyType enemyType; 
        
        for(int i = 0; i < worldData.ammountOfSpawns; i++)
        {
            enemyType = enemyTypesToSpawn[Random.Range(0, enemyTypesToSpawn.Count)];
            enemyData = worldData.GetEnemyData(enemyType);
            
            enemyObj = Instantiate(enemyData.enemyPrefab, enemiesContainer);
            enemyObj.transform.position = CalculateRandomSpawnPosition();
            
            enemy = enemyObj.GetComponent<EnemyManager>();
            enemy.Init(enemyData);
            
            worldData.AddEnemy(enemy);
        }
    }

    private Vector3 CalculateRandomSpawnPosition()
    {
        Rect availableRect = new Rect(topLeftCorner.position.x, topLeftCorner.position.z, bottomRightCorner.position.x - topLeftCorner.position.x,
            bottomRightCorner.position.z - topLeftCorner.position.z);

        float x = Random.Range(topLeftCorner.position.x, bottomRightCorner.position.x);
        float z = Random.Range(topLeftCorner.position.z, bottomRightCorner.position.z);
        
        return new Vector3(x, 0f, z);
    }
}