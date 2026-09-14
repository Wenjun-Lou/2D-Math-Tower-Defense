using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

// 塔牌类，负责显示塔的属性和处理点击事件
public class TowerCard : MonoBehaviour
{
    public static event Action<TowerData> OnTowerSelected;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text costText;

    private TowerData _towerData;

    public void Initialize(TowerData data)
    {
        _towerData = data;
        nameText.text = data.towerName;
        towerImage.sprite = data.sprite;
        damageText.text = "Damage: " + data.damage.ToString();
        rangeText.text = "Range: " + data.range.ToString("0.0");
        speedText.text = "Speed: " + (1f / data.shootInterval).ToString("0.0");
        costText.text = data.cost.ToString();
    }

    public void PlaceTower()
    {
        OnTowerSelected?.Invoke(_towerData);
    }
}
