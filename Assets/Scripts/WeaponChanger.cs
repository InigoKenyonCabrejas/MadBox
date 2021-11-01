using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private Button previousWeaponButton;
    [SerializeField] private Button nextWeaponButton;

    private void Awake()
    {
        previousWeaponButton.onClick.AddListener(ChangeToPreviousWeapon);
        nextWeaponButton.onClick.AddListener(ChangeToNextWeapon);
    }

    private void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                ChangeToNextWeapon();
            }
            else
            {
                ChangeToPreviousWeapon();
            }
        }
    }

    private void ChangeToPreviousWeapon()
    {
        WeaponType currentWeaponType = characterData.EquipedWeapon.weaponType;
        if(currentWeaponType == 0)
        {
            currentWeaponType = (WeaponType) characterData.weapons.Count - 1;
        }
        else
        {
            currentWeaponType--;
        }

        characterData.SwapWeapon(currentWeaponType);
    }

    private void ChangeToNextWeapon()
    {
        WeaponType currentWeaponType = characterData.EquipedWeapon.weaponType;
        if(currentWeaponType == (WeaponType) characterData.weapons.Count - 1)
        {
            currentWeaponType = 0;
        }
        else
        {
            currentWeaponType++;
        }

        characterData.SwapWeapon(currentWeaponType);
    }
}