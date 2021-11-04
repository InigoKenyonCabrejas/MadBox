using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] private ArmorySO armory;
    
    public float movementSpeed = 0.1f;
    public List<WeaponType> ownedWeapons;

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
        equipedWeapon = armory.GetWeaponByType(ownedWeapons[Random.Range(0, ownedWeapons.Count)]);
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
}