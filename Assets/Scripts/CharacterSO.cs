using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    public List<Weapon> weapons;

    private Weapon equipedWeapon;
    
    public Action WeaponChangedAction;
    
    public Weapon EquipedWeapon
    {
        get { return equipedWeapon; }
        set
        {
            equipedWeapon = value;
            WeaponChangedAction?.Invoke();
        }
    }
    
    private void OnEnable()
    {
        equipedWeapon = weapons[Random.Range(0, weapons.Count)];
    }

    public void SwapWeapon(WeaponType weaponType)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            if(weaponType == weapons[i].weaponType)
            {
                EquipedWeapon = weapons[i];
            }
        }
    }

    [Serializable]
    public class Weapon
    {
        public WeaponType weaponType;
        public GameObject weaponPrefab;
        public float attackSpeedMultiplier;
        public float heroMovementSpeed;
        public float attackRange;
    }
}