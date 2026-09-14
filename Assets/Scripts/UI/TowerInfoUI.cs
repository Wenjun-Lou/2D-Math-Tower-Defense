using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 防御塔信息UI脚本---负责显示防御塔的详细信息
public class TowerInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image towerImage;

    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private TMP_Text speedText;

    [SerializeField] private TMP_Text upgradeCostText;

    public void UpdateTowerInfo(Tower tower)
    {
        nameText.text = tower.Data.towerName + " - " + tower.Level;
        towerImage.sprite = tower.Data.sprite;
        damageText.text = "Damage: " + tower.CurrentDamage.ToString("0.0");
        rangeText.text = "Range: " + tower.CurrentRange.ToString("0.0");
        speedText.text = "Speed: " + (1f / tower.CurrentShootInterval).ToString("0.0");
        upgradeCostText.text = tower.UpgradeCost.ToString();
        if(tower.UpgradeCost > GameManager.Instance.Resources)
        {
            upgradeCostText.color = Color.red;
        }
        else
        {
            upgradeCostText.color = Color.green;
        }
    }
}
