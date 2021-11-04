using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/World", order = 1)]
public class WorldSO : ScriptableObject
{
    [SerializeField] private ArmorySO armory;

    public int ammountOfSpawns = 200;
    public List<Enemy> allEnemiesData;

    private const string GAME_SCENE = "CombatScene";

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

    private List<EnemyView> aliveEnemies;
    public List<EnemyView> AliveEnemies { get { return aliveEnemies; } }

    public Action WorldIsSetupAction;
    public Action<Weapon, Vector3> LootDroppedAction;

    private void OnEnable()
    {
        IsWorldSetup = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if(string.Equals(scene.name, GAME_SCENE))
        {
            aliveEnemies = new List<EnemyView>();
        }
    }

    private void OnEnemyDeath(EnemyView enemy)
    {
        RemoveEnemy(enemy);
        LootDroppedAction?.Invoke(armory.GetRandomWeapon(), enemy.transform.position);
        BoostEnemiesPower();
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

    public void AddEnemy(EnemyView enemy)
    {
        aliveEnemies.Add(enemy);
        enemy.EnemyDeathAction += OnEnemyDeath;
    }

    private void RemoveEnemy(EnemyView enemy)
    {
        aliveEnemies.Remove(enemy);
        enemy.EnemyDeathAction -= OnEnemyDeath;
    }

    private void BoostEnemiesPower()
    {
        for(int i = 0; i < aliveEnemies.Count; i++)
        {
            aliveEnemies[i].MakeStronger();
        }
    }
}