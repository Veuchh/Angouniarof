using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] TextMeshProUGUI debugGameStateText;
    [SerializeField] RectTransform scoreRecapScreen;
    [SerializeField] float scoreRecapMoveDuration = .2f;
    [SerializeField] float scoreRecapShownPosition;
    [SerializeField] float scoreRecapHiddenPosition;

    private void Awake()
    {
        Instance = this;
    }

    public Tween ToggleScoreRecapScreen(bool toggle)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(scoreRecapScreen.DOMoveY(
            toggle ? 
                scoreRecapShownPosition : 
                scoreRecapHiddenPosition, 
            scoreRecapMoveDuration)
                .SetEase(Ease.OutBack));

        return sequence;
    }

    public Tween UpdateScores(int player1Score, int player2Score)
    {
        Sequence sequence = DOTween.Sequence();
        //TODO
        return sequence;
    }

    public Tween ShowPlayerWinUI(PlayerID winningPlayer)
    {
        Sequence sequence = DOTween.Sequence();
        //TODO
        return sequence;
    }

    public void UpdateGameStateDebug(GameState newState, PlayerID currentPlayerTurn, PlayerID currentWinningState, int player1Score, int player2Score)
    {
        string output = newState.ToString();
        if (newState == GameState.InGame)
        {
            output += $" : {currentPlayerTurn.ToString()}\nCurrentWinningState : {currentWinningState}";
        }

        output += $"\nPlayer1 Score : {player1Score}\nPlayer2 Score : {player2Score}";

        debugGameStateText.text = output;
    }
}
