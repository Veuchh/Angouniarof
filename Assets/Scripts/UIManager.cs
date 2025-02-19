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

    [SerializeField] Transform player1TokenPrefab;
    [SerializeField] Transform player1TokenParent;
    [SerializeField] Transform player2TokenPrefab;
    [SerializeField] Transform player2TokenParent;
    [SerializeField] Transform boardTilePrefab;
    [SerializeField] Transform player1BoardParent;
    [SerializeField] Transform player2BoardParent;
    [SerializeField] TextMeshProUGUI debugGameStateText;
    [SerializeField] RectTransform scoreRecapScreen;

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

    private void Update()
    {
        if (player1BoardParent.childCount != 0)
            player1TokenParent.position = player1BoardParent.GetChild(0).position;

        if (player2BoardParent.childCount != 0)
            player2TokenParent.position = player2BoardParent.GetChild(0).position;
    }

    [Button]
    void SetupBoardGames3Points()
    {
        SetupBoardGame(3);
    }

    [Button]
    void SetupBoardGames5Points()
    {
        SetupBoardGame(5);
    }

    [Button]
    void SetupBoardGames6Points()
    {
        SetupBoardGame(6);
    }

    public void SetupBoardGame(int neededPointsToWin)
    {
        for (int i = 0; i < neededPointsToWin; i++)
        {
            Instantiate(boardTilePrefab, player1BoardParent);
            Instantiate(boardTilePrefab, player2BoardParent);
        }

        player1TokenParent.transform.position = player1BoardParent.GetChild(0).position;
        player2TokenParent.transform.position = player2BoardParent.GetChild(0).position;

        player1Instance = Instantiate(player1TokenPrefab, player1TokenParent);
        player2Instance = Instantiate(player2TokenPrefab, player2TokenParent);
    }

    public Tween ToggleScoreRecapScreen(bool toggle)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(scoreRecapScreen.DOMoveY(
                toggle ? scoreRecapShownPosition : scoreRecapHiddenPosition,
                scoreRecapMoveDuration)
            .SetEase(Ease.InOutBack));

        return sequence;
    }

    public Tween UpdateScores(int player1Score, int player2Score)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(player1Instance.DOMoveX(player1BoardParent.GetChild(player1Score).position.x,
            playerMovementDuration));
        sequence.Join(player2Instance.DOMoveX(player2BoardParent.GetChild(player2Score).position.x,
            playerMovementDuration));
        return sequence;
    }

    public Tween ShowPlayerWinUI(PlayerID winningPlayer)
    {
        Sequence sequence = DOTween.Sequence();
        //TODO
        return sequence;
    }

    public void UpdateGameStateDebug(GameState newState, PlayerID currentPlayerTurn, PlayerID currentWinningState,
        int player1Score, int player2Score)
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
}
