using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 按钮点击和悬停音效播放脚本
[RequireComponent(typeof(Button))]
public class PlayButtonClick : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButtonClick();
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayButtonHover();
    }
}
