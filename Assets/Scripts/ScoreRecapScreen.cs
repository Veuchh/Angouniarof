using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ScoreRecapScreen : MonoBehaviour
{
    [SerializeField] Transform player1TokenPrefab;
    [SerializeField] Transform player1TokenParent;
    [SerializeField] Transform player2TokenPrefab;
    [SerializeField] Transform player2TokenParent;
    [SerializeField] Transform boardTilePrefab;
    [SerializeField] Transform player1BoardParent;
    [SerializeField] Transform player2BoardParent;
    [SerializeField] RectTransform scoreRecapScreen;

    [Header("Tweening settings")]
    [SerializeField] float playerMovementDuration = .7f;
    [SerializeField] float scoreRecapMoveDuration = .2f;
    [SerializeField] float scoreRecapShownPosition;
    [SerializeField] float scoreRecapHiddenPosition;

    Transform player1Instance;
    Transform player2Instance;


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
            toggle ?
                scoreRecapShownPosition :
                scoreRecapHiddenPosition,
            scoreRecapMoveDuration)
                .SetEase(Ease.InOutBack));

        return sequence;
    }

    public Tween UpdateScores(int player1Score, int player2Score)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(player1Instance.DOMoveX(player1BoardParent.GetChild(player1Score).position.x, playerMovementDuration));
        sequence.Join(player2Instance.DOMoveX(player2BoardParent.GetChild(player2Score).position.x, playerMovementDuration));
        return sequence;
    }

    public Tween ShowPlayerWinUI(PlayerID winningPlayer)
    {
        Sequence sequence = DOTween.Sequence();
        //TODO
        return sequence;
    }
}
