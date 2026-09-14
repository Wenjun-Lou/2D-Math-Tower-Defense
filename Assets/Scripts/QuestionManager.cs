using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private QuestionData[] questions;

    public static QuestionManager Instance { get; private set; }

    private int _currentQuestionIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public QuestionData GetNextQuestion()
    {
        if (questions == null || questions.Length == 0) return null;

        QuestionData question = questions[_currentQuestionIndex];

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= questions.Length)
        {
            _currentQuestionIndex = 0;
        }

        return question;
    }
}