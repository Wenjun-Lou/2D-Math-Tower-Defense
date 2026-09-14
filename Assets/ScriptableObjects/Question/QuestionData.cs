using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData")]
public class QuestionData : ScriptableObject
{
    public Sprite questionSprite;
    [Range(0, 3)]
    public int correctAnswerIndex;
}
