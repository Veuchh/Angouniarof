using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    [SerializeField] int roundTurnsAmount = 15;
    [SerializeField] float turnDuration = 5f;
    [SerializeField] int pointsToWinGame = 3;

    int player1Score = 0;
    int player2Score = 0;
    int currentTurnIndex = 0;
    PlayerID currentWinningSide;
    PlayerID currentPlayerTurn = PlayerID.Player1;
    GameState currentGameState = GameState.Starting;

    bool isPlayer1Ready = false;
    bool isPlayer2Ready = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UIManager.Instance.UpdateGameStateDebug(currentGameState, currentPlayerTurn, currentWinningSide, player1Score, player2Score);

        if (isPlayer1Ready && isPlayer2Ready)
        {
            if (currentGameState == GameState.Starting)
                StartGame();
            else if (currentGameState == GameState.ScoreRecap)
                StartRound();
        }
    }

    void StartGame()
    {
        currentGameState = GameState.Starting;
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

        currentGameState = GameState.InTransition;

        currentWinningSide = Random.Range(0, 2) == 1 ? PlayerID.Player1 : PlayerID.Player2;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(Hourglass.Instance.SetDefaultHourglass(currentWinningSide));
        //TODO => Indicate clearly to the player which side is going to win if the hourglass is not flipped
        sequence.AppendInterval(1);
        sequence.AppendCallback(() => currentGameState = GameState.InGame);
        sequence.AppendCallback(() => Hourglass.Instance.ToggleHourglass(false));
        sequence.AppendCallback(() => StartPlayerTurn(PlayerID.Player1));
    }

    void EndRound()
    {
        currentGameState = GameState.InTransition;

        if (currentWinningSide == PlayerID.Player1)
        {
            player1Score++;
        }
        else
        {
            player2Score++;
        }

        Sequence roundEndSequence = DOTween.Sequence();
        roundEndSequence.Append(Hourglass.Instance.ShowHourglassResult(currentWinningSide));
        roundEndSequence.Append(UIManager.Instance.ToggleScoreRecapScreen(true));
        roundEndSequence.Append(UpdatePlayerScoreUI());

        if (currentGameState != GameState.GameOver)
        {
            roundEndSequence.AppendCallback(() => currentGameState = GameState.ScoreRecap);
        }
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
        currentGameState = GameState.GameOver;

        Sequence sequence = DOTween.Sequence();

        UIManager.Instance.ShowPlayerWinUI(winningPlayer);

        return sequence;
    }

    void OnPlayerPlayedTurn()
    {
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
    }

    public void OnPlayerInput(PlayerID inputtingPlayer, InputType inputType)
    {
        if (currentGameState == GameState.InGame && inputtingPlayer == currentPlayerTurn)
        {
            if (inputType == InputType.Rotate)
            {
                currentWinningSide = currentWinningSide == PlayerID.Player1 ? PlayerID.Player2 : PlayerID.Player1;
            }

            OnPlayerPlayedTurn();
        }
        else if (currentGameState == GameState.ScoreRecap || currentGameState == GameState.Starting)
        {
            if (inputtingPlayer == PlayerID.Player1)
                isPlayer1Ready = inputType == InputType.Rotate;

            else
                isPlayer2Ready = inputType == InputType.Rotate;
        }
    }
}
