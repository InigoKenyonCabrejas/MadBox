using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwaperView : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private Button previousWeaponButton;
    [SerializeField] private Button nextWeaponButton;
    [SerializeField] private Text equipedWeaponText;

    private void Awake()
    {
        previousWeaponButton.onClick.AddListener(ChangeToPreviousWeapon);
        nextWeaponButton.onClick.AddListener(ChangeToNextWeapon);

        characterData.WeaponChangedAction += OnWeaponChanged;
    }

    private void Start()
    {
        equipedWeaponText.text = characterData.EquipedWeapon.weaponType.ToString();
    }

    private void OnDestroy()
    {
        previousWeaponButton.onClick.RemoveAllListeners();
        nextWeaponButton.onClick.RemoveAllListeners();

        characterData.WeaponChangedAction -= OnWeaponChanged;
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

    private void OnWeaponChanged()
    {
        equipedWeaponText.text = characterData.EquipedWeapon.weaponType.ToString();
    }

    private void ChangeToPreviousWeapon()
    {
        WeaponType currentWeaponType = characterData.EquipedWeapon.weaponType;
        if(currentWeaponType == 0)
        {
            currentWeaponType = (WeaponType)characterData.ownedWeapons.Count - 1;
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
        if(currentWeaponType == (WeaponType)characterData.ownedWeapons.Count - 1)
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