using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPlacer : MonoBehaviour
{
    [SerializeField] private WorldSO worldData;
    [SerializeField] private float yOffset = 0.8f;

    private void Awake()
    {
        worldData.LootDroppedAction += OnLootDropped;
    }

    private void OnDestroy()
    {
        worldData.LootDroppedAction -= OnLootDropped;
    }

    private void OnLootDropped(Weapon weapon, Vector3 pos)
    {
        DropWeapon(weapon, pos);
    }

    private void DropWeapon(Weapon weapon, Vector3 pos)
    {
        GameObject lootObj;
        Loot loot;
        
        lootObj = Instantiate(weapon.weaponPrefab, transform);
        pos.y += yOffset;
        lootObj.transform.position = pos;
        lootObj.transform.localScale *= 3;
        lootObj.transform.Rotate(new Vector3(90f, 0f, 90f));
        loot = lootObj.AddComponent<Loot>();
        loot.weaponType = weapon.weaponType;
    }
}