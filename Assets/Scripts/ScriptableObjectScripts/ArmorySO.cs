using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armory", menuName = "ScriptableObjects/Armory", order = 1)]
public class ArmorySO : ScriptableObject
{
    public List<Weapon> weapons;

    public Weapon GetWeaponByType(WeaponType type)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            if(type == weapons[i].weaponType)
            {
                return weapons[i];
            }
        }

        return null;
    }

    public Weapon GetRandomWeapon()
    {
        return weapons[Random.Range(0, weapons.Count)];
    }
}
