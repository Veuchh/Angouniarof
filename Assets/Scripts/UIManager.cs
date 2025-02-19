using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] ScoreRecapScreen playerScoreRecapScreen;
    [SerializeField] ScoreRecapScreen audienceScoreRecapScreen;
    [SerializeField] TextMeshProUGUI debugGameStateText;

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
