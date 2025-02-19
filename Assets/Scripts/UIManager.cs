using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] ScoreRecapScreen playerScoreRecapScreen;
    [SerializeField] ScoreRecapScreen audienceScoreRecapScreen;
    [SerializeField] TextMeshProUGUI debugGameStateText;

    [Header("Tweening settings")] [SerializeField]
    float playerMovementDuration = .7f;

    [SerializeField] float scoreRecapMoveDuration = .2f;
    [SerializeField] float scoreRecapShownPosition;
    [SerializeField] float scoreRecapHiddenPosition;

    [Header("InGame UI")]
    [SerializeField] private List<Image> player1StepsMain;
    [SerializeField] private List<Image> player2StepsMain;
    [SerializeField] private TextMeshProUGUI roundText;

    [SerializeField] private List<Image> playersActionPublic;
    [SerializeField] private List<Sprite> rotationSprites;
    [SerializeField] private List<Sprite> cancelSprites;


    Transform player1Instance;
    Transform player2Instance;

    private int roundIndex = 1;

    private void Awake()
    {
        Instance = this;
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

    public void UpdateStep(PlayerID currentPlayerTurn, int currentTurnIndex)
    {
        if (currentPlayerTurn == PlayerID.Player1)
            player1StepsMain[currentTurnIndex / 2].color = Color.white;
        else
            player2StepsMain[(currentTurnIndex - 1) / 2].color = Color.white;
    }

    public void ResetStep()
    {
        for (int loop = 0; loop < player1StepsMain.Count; loop++)
        {
            player1StepsMain[loop].color = new Color(.5f, .5f, .5f, .5f);
            player2StepsMain[loop].color = new Color(.5f, .5f, .5f, .5f);
        }
    }

    public void UpdateLastMove(PlayerID currentPlayerTurn, InputType inputType)
    {
        int playerIndex = currentPlayerTurn == PlayerID.Player1 ? 0 : 1;
        playersActionPublic[playerIndex].color = Color.white;


        if (inputType == InputType.Rotate)
        {
            playersActionPublic[playerIndex].sprite = rotationSprites[playerIndex];
        }
        else
        {
            playersActionPublic[playerIndex].sprite = cancelSprites[playerIndex];
        }
    }

    public void ResetLastMove()
    {
        playersActionPublic[0].color = Color.clear;
        playersActionPublic[1].color = Color.clear;
    }

    public void UpdateRoundText()
    {
        roundText.text = $"Round {roundIndex}";
        ++roundIndex;
    }

    public void SetupBoardGame(int tilesAmount)
    {
        playerScoreRecapScreen.SetupBoardGame(tilesAmount);
        audienceScoreRecapScreen.SetupBoardGame(tilesAmount);
    }

    public Tween ToggleScoreRecapScreen(bool toggle)
    {

        Sequence sequence = DOTween.Sequence();

        sequence.Join(playerScoreRecapScreen.ToggleScoreRecapScreen(toggle));
        sequence.Join(audienceScoreRecapScreen.ToggleScoreRecapScreen(toggle));

        return sequence;
    }

    public Tween UpdateScores(int player1Score, int player2Score)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(playerScoreRecapScreen.UpdateScores(player1Score, player2Score));
        sequence.Join(audienceScoreRecapScreen.UpdateScores(player1Score, player2Score));

        return sequence;
    }

    public void ShowPlayerWinUI(PlayerID winningPlayer)
    {
        playerScoreRecapScreen.ShowPlayerWinUI(winningPlayer);
        audienceScoreRecapScreen.ShowPlayerWinUI(winningPlayer);
    }
}
