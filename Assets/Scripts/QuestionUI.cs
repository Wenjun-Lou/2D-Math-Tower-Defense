using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionUI : MonoBehaviour
{
    [SerializeField] private Image questionImage;

    [SerializeField] private Button[] answerButtons;

    public static event Action OnAnswerCorrect;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    [SerializeField] private Button upgradeButton;

    private QuestionData _currentQuestion;


    public void Initialize()
    {
        questionImage.sprite = null;
        questionImage.gameObject.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].image.color = normalColor;
            answerButtons[i].interactable = false;
            answerButtons[i].onClick.RemoveAllListeners();
        }

        _currentQuestion = null;
    }


    public void ShowNextQuestion()
    {
        _currentQuestion = QuestionManager.Instance.GetNextQuestion();

        if (_currentQuestion == null)
        {
            return;
        }

        // 显示题目图片
        questionImage.sprite = _currentQuestion.questionSprite;
        questionImage.gameObject.SetActive(true);

        // 设置答案按钮
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;
            answerButtons[i].image.color = normalColor;
            answerButtons[i].onClick.RemoveAllListeners();

            int buttonIndex = i;

            answerButtons[i].onClick.AddListener(
                () => SelectAnswer(buttonIndex)
            );
        }
    }


    private void SelectAnswer(int buttonIndex)
    {
        // 判断是否为正确答案
        if (buttonIndex == _currentQuestion.correctAnswerIndex)
        {
            // 正确答案 → 绿色
            answerButtons[buttonIndex].image.color = correctColor;

            OnAnswerCorrect?.Invoke();
        }
        else
        {
            // 错误答案 → 红色
            answerButtons[buttonIndex].image.color = wrongColor;

            // 正确答案 → 绿色
            answerButtons[
                _currentQuestion.correctAnswerIndex
            ].image.color = correctColor;
        }

        // 禁止继续点击
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
        }

        // 开启升级按钮
        upgradeButton.interactable = true;
    }
}