using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Windows.WebCam;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] private ArmorySO armory;

    public int maxHealth;
    public float movementSpeed = 0.1f;
    public List<WeaponType> ownedWeapons;

    private int maxScore;
    private int currentScore;
    
    private const string GAME_SCENE = "CombatScene";
    private const string GAME_OVER_SCENE = "GameOverScene";

    public int CurrentScore
    {
        get { return currentScore; }
        set
        {
            currentScore = value;
            if(currentScore > maxScore)
            {
                maxScore = currentScore;
            }

            ScoreChangedAction?.Invoke(currentScore);
        }
    }

    private int currentHealth;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            HealthChangedAction?.Invoke(currentHealth, maxHealth);

            if(currentHealth <= 0)
            {
                if(currentHealth < 0)
                {
                    currentHealth = 0;
                }
                
                KillPlayer();
            }
        }
    }

    private Weapon equipedWeapon;

    public Weapon EquipedWeapon
    {
        get { return equipedWeapon; }
        set
        {
            equipedWeapon = value;
            WeaponChangedAction?.Invoke();
        }
    }

    public Action WeaponChangedAction;
    public Action<int, int> HealthChangedAction;
    public Action<int> ScoreChangedAction;
    public Action CharacterDeadAction;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if(string.Equals(scene.name, GAME_SCENE))
        {
            equipedWeapon = armory.GetWeaponByType(ownedWeapons[Random.Range(0, ownedWeapons.Count)]);
            CurrentScore = 0;
            CurrentHealth = maxHealth;
        }
    }

    public void SwapWeapon(WeaponType weaponType)
    {
        for(int i = 0; i < ownedWeapons.Count; i++)
        {
            if(weaponType == ownedWeapons[i])
            {
                EquipedWeapon = armory.GetWeaponByType(ownedWeapons[i]);
            }
        }
    }

    public void AddNewWeapon(WeaponType weaponType)
    {
        for(int i = 0; i < ownedWeapons.Count; i++)
        {
            if(weaponType == ownedWeapons[i])
            {
                EquipedWeapon = armory.GetWeaponByType(weaponType);
                return;
            }
        }

        ownedWeapons.Add(weaponType);
        EquipedWeapon = armory.GetWeaponByType(weaponType);
    }

    public void KillPlayer()
    {
        CharacterDeadAction?.Invoke();
        CoroutineRunner.instance.StartCoroutine(DelayedSceneChange());
    }

    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(5f);
        
        SceneManager.LoadScene(GAME_OVER_SCENE);
    }
}