using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private Text scoreText;

    private void Awake()
    {
        characterData.ScoreChangedAction += OnScoreChanged;
    }

    private void OnDestroy()
    {
        characterData.ScoreChangedAction -= OnScoreChanged;
    }

    private void OnScoreChanged(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
