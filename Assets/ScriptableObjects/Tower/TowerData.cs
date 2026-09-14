using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public float range;
    public float shootInterval;
    public float projectileSpeed;
    public float projectileDuration;
    public float projectileSize;
    public float damage;
    public int cost;

    public float damageGrowth;
    public float rangeGrowth;
    public float attackSpeedGrowth;
    public float upgradeCostGrowth;

    public Sprite sprite;
    public GameObject prefab;
    public AudioClip attackSound;
}
