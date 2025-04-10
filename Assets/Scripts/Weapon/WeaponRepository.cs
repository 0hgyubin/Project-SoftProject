using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData {
    public int weaponID;
    public GameObject weaponPrefab; //무기 프리팹
    public GameObject projectilePrefab; //투사체 프리팹
    public float attackPower;
    public float projectileLifetime; //무기마다 lifetime을 다르게 설정하기 위한 변수
    public WeaponGrade grade;
}

[CreateAssetMenu(fileName = "WeaponRepository", menuName = "Game/Weapon Repository")]
public class WeaponRepository : ScriptableObject {
    public List<WeaponData> weapons;

    public GameObject GetWeaponPrefabByID(int id) {
        foreach (var weapon in weapons) {
            if (weapon.weaponID == id) {
                return weapon.weaponPrefab;
            }
        }
        Debug.LogError("Weapon with ID " + id + " not found!");
        return null;
    }
    
    public WeaponData GetWeaponDataByID(int id) {
        foreach (var weapon in weapons) {
            Debug.Log("weapon.weaponID:" + weapon.weaponID);
            if (weapon.weaponID == id) {
                return weapon;
            }
        }
        Debug.LogError("Weapon data with ID " + id + " not found!");
        return null;
    }
}
