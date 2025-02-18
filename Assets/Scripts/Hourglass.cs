using DG.Tweening;
using UnityEngine;

/// <summary>
/// By convention, the hourglass player 1 side has a z rotation of 0, and player 2 has a z rotation of 180
/// </summary>

public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [Header("Hourglass settings")]
    [SerializeField] float setupTweenDuration = .5f;
    [SerializeField] int setupTurnsAmount = 10;
    [SerializeField] float resultTweenDuration = 1.3f;
    [SerializeField] float resultHeightChange = 3;
    [SerializeField] int resultTurnsAmount = 30;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetHourglassRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public Tween SetDefaultHourglass(PlayerID startWinningSide)
    {
        ResetHourglassRotation();

        Sequence sequence = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;

        targetRotation.z = 360 * setupTurnsAmount + startWinningSide == PlayerID.Player1 ? 0 : 180;

        sequence.Append(transform.DORotate(targetRotation, setupTweenDuration));

        return sequence;
    }

    public void MaskHourglass()
    {

    }

    public Tween ShowHourglassResult(PlayerID finalWinningSide)
    {
        ResetHourglassRotation();

        Sequence sequence = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;

        targetRotation.z = 360 * resultTurnsAmount + finalWinningSide == PlayerID.Player1 ? 0 : 180;

        sequence.Append(transform.DORotate(targetRotation, resultTweenDuration));
        sequence.Join(transform.DOMoveY(resultHeightChange, resultTweenDuration / 2).SetEase(Ease.OutQuad));
        sequence.Join(transform.DOMoveY(0, resultTweenDuration / 2).SetEase(Ease.InQuad));
        return sequence;
    }
}
