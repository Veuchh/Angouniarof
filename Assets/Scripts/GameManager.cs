using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public Queue<InputType> playerInputQueue = new();

    [Header("Game Settings")]
    [SerializeField] int roundTurnsAmount = 15;
    [SerializeField] float turnDuration = 5f;
    [SerializeField] int pointsToWinGame = 3;
    
    [Header("Audio")]
    [SerializeField] SFXData playerPlayedSFX;
    [SerializeField] SFXData roundStartSFX;
    [SerializeField] SFXData roundEndSFX;
    [SerializeField] SFXData winSFX;

    int player1Score = 0;
    int player2Score = 0;
    int currentTurnIndex = 0;
    PlayerID currentWinningSide, startWinningSide;
    PlayerID currentPlayerTurn = PlayerID.Player1;
    GameState currentGameState = GameState.Starting;

    bool isPlayer1Ready = false;
    bool isPlayer2Ready = false;

    private float playerTurnedStartTime;
    private InputType currentInputType;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UIManager.Instance.UpdateGameStateDebug(currentGameState, currentPlayerTurn, currentWinningSide, player1Score,
            player2Score);

        if (isPlayer1Ready && isPlayer2Ready)
        {
            if (currentGameState == GameState.Starting)
                StartGame();
            else if (currentGameState == GameState.ScoreRecap)
                StartRound();
            else if (currentGameState == GameState.GameOver)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (currentGameState == GameState.InGame)
        {
            if (Time.time - playerTurnedStartTime >= turnDuration)
                OnPlayerInput(currentPlayerTurn, InputType.Fake);
            else
                UIManager.Instance.DisplayTimeLeft(- Time.time + playerTurnedStartTime + turnDuration);
        }
    }

    void StartGame()
    {
        currentGameState = GameState.Starting;
        UIManager.Instance.SetupBoardGame(pointsToWinGame + 1);
        player1Score = 0;
        player2Score = 0;

        StartRound();
    }

    void StartRound()
    {
        isPlayer1Ready = false;
        isPlayer2Ready = false;
        currentTurnIndex = 0;

        UIManager.Instance.ToggleScoreRecapScreen(false);
        UIManager.Instance.ResetStep();
        UIManager.Instance.ResetLastMove();
        UIManager.Instance.UpdateRoundText();

        currentGameState = GameState.InTransition;

        currentWinningSide = Random.Range(0, 2) == 1 ? PlayerID.Player1 : PlayerID.Player2;
        startWinningSide = currentWinningSide;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => AudioManager.Instance.ChangeMusicType(true));
        sequence.Append(Hourglass.Instance.SetDefaultHourglass(currentWinningSide));
        //TODO => Indicate clearly to the player which side is going to win if the hourglass is not flipped
        sequence.AppendInterval(1);
        sequence.AppendCallback(() => currentGameState = GameState.InGame);
        sequence.AppendCallback(() => Hourglass.Instance.ToggleHourglass(false));
        sequence.AppendCallback(() => StartPlayerTurn(PlayerID.Player1));
        
        AudioManager.Instance.PlaySFX(roundStartSFX);
    }

    void EndRound()
    {
        currentGameState = GameState.InTransition;
        UIManager.Instance.HideCenterText();
        UIManager.Instance.ResetPlayerScreen();

        if (currentWinningSide == PlayerID.Player1)
        {
            player1Score++;
        }
        else
        {
            player2Score++;
        }

        Sequence roundEndSequence = DOTween.Sequence();
        roundEndSequence.Append(Hourglass.Instance.ShowHourglassResult(startWinningSide));
        roundEndSequence.AppendCallback(() => AudioManager.Instance.ChangeMusicType(false));
        roundEndSequence.Append(UIManager.Instance.ToggleScoreRecapScreen(true));
        roundEndSequence.Append(UpdatePlayerScoreUI());

        playerInputQueue.Clear();

        if (currentGameState != GameState.GameOver)
        {
            roundEndSequence.AppendCallback(() => currentGameState = GameState.ScoreRecap);
        }
        
        AudioManager.Instance.PlaySFX(roundEndSFX);
    }

    Tween UpdatePlayerScoreUI()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(UIManager.Instance.UpdateScores(player1Score, player2Score));

        if (player1Score >= pointsToWinGame)
        {
            sequence.Append(OnPlayerVictory(PlayerID.Player1));
        }
        else if (player2Score >= pointsToWinGame)
        {
            sequence.Append(OnPlayerVictory(PlayerID.Player2));
        }

        return sequence;
    }

    Tween OnPlayerVictory(PlayerID winningPlayer)
    {
        //currentGameState = GameState.GameOver;

        Sequence sequence = DOTween.Sequence();
        
        sequence.AppendCallback(() => UIManager.Instance.ShowPlayerWinUI(winningPlayer));
        sequence.AppendCallback(() => AudioManager.Instance.PlaySFX(winSFX));
        sequence.Append(UIManager.Instance.ShowVictoryScreen(winningPlayer));
        sequence.Join(DOTween.Sequence().AppendCallback(() => currentGameState = GameState.GameOver));

        return sequence;
    }

    void OnPlayerPlayedTurn()
    {
        UIManager.Instance.UpdateStep(currentPlayerTurn, currentTurnIndex);
        UIManager.Instance.UpdateLastMove(currentPlayerTurn, currentInputType);
        currentTurnIndex++;

        if (currentTurnIndex >= roundTurnsAmount)
        {
            EndRound();
            return;
        }

        PlayerID nextPlayer = currentPlayerTurn == PlayerID.Player1 ? PlayerID.Player2 : PlayerID.Player1;

        StartPlayerTurn(nextPlayer);
    }

    void StartPlayerTurn(PlayerID nextPlayerTurn)
    {
        currentPlayerTurn = nextPlayerTurn;
        playerTurnedStartTime = Time.time;
        UIManager.Instance.MakeTextAppear(nextPlayerTurn);
        UIManager.Instance.PlayerScreen(nextPlayerTurn);
    }

    public void OnPlayerInput(PlayerID inputtingPlayer, InputType inputType)
    {
        if (currentGameState == GameState.InGame && inputtingPlayer == currentPlayerTurn)
        {
            Sequence sequence = DOTween.Sequence();
            playerInputQueue.Enqueue(inputType);

            if (inputType == InputType.Rotate)
            {
                currentWinningSide = currentWinningSide == PlayerID.Player1 ? PlayerID.Player2 : PlayerID.Player1;
                sequence.AppendCallback(() => currentGameState = GameState.InTransition);
                sequence.Append(Hourglass.Instance.RotateHourglassToPlayerWinningState(inputtingPlayer));
                sequence.AppendCallback(() => currentGameState = GameState.InGame);
            }
            else
            {
                sequence.AppendCallback(() => currentGameState = GameState.InTransition);
                sequence.Append(Hourglass.Instance.BluffHourglass());
                sequence.AppendCallback(() => currentGameState = GameState.InGame);
            }

            AudioManager.Instance.PlaySFX(playerPlayedSFX);

            currentInputType = inputType;
            sequence.AppendCallback(OnPlayerPlayedTurn);
        }
        else if (currentGameState == GameState.ScoreRecap || currentGameState == GameState.Starting)
        {
            if (inputtingPlayer == PlayerID.Player1)
                isPlayer1Ready = inputType == InputType.Rotate;
            else
                isPlayer2Ready = inputType == InputType.Rotate;
        }
        else if (currentGameState == GameState.GameOver)
        {
            if (inputtingPlayer == PlayerID.Player1)
                isPlayer1Ready = inputType == InputType.Rotate;
            else
                isPlayer2Ready = inputType == InputType.Rotate;
        }
    }
}
