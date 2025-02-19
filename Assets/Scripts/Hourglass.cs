using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// By convention, the hourglass player 1 side has a z rotation of 0, and player 2 has a z rotation of 180
/// </summary>

public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;
    
    [SerializeField] LayerMask playerCameraHiddenLayerMask;
    [SerializeField] LayerMask playerCameraShownLayerMask;
    [SerializeField] Camera playerCamera;
    [Header("Hourglass settings")]
    [SerializeField] float setupTweenDuration = .5f;
    [SerializeField] int setupTurnsAmount = 10;
    [SerializeField] float resultTweenDuration = 1.3f;
    [SerializeField] float resultHeightChange = 3;
    [SerializeField] float xMovementWhenBluffing = 5;
    // [SerializeField] int resultTurnsAmount = 30; No need for it anymore, now storing inputs to know how many

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

        //targetRotation.z = 360 * setupTurnsAmount + startWinningSide == PlayerID.Player1 ? 0 : 180;
        targetRotation.z = 360;
        
        float rotationTime = setupTweenDuration / setupTurnsAmount; 
        
        sequence.SetEase(Ease.InOutQuad);
        sequence.Append(transform.DORotate(targetRotation, rotationTime).SetRelative(true).SetLoops(setupTurnsAmount).SetEase(Ease.Linear));

        if (startWinningSide == PlayerID.Player2)
        {
            sequence.Append(transform.DORotate(new Vector3(0, 0, 180), rotationTime/2)
                .SetRelative(true).SetEase(Ease.Linear));
        }

        //sequence.Append(transform.DORotate(targetRotation, setupTweenDuration));

        return sequence;
    }

    public void ToggleHourglass(bool toggle)
    {
        playerCamera.cullingMask = toggle ? playerCameraShownLayerMask : playerCameraHiddenLayerMask;
    }

    public Tween ShowHourglassResult(PlayerID finalWinningSide)
    {
        ResetHourglassRotation();

        ToggleHourglass(true);

        Sequence sequenceFlip = DOTween.Sequence();
        Sequence sequenceUpMovement = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;
        Stack<InputType> playerTurnStack = GameManager.Instance.playerInputStack;
        int resultTurnsAmount = playerTurnStack.Count;
        float rotationTime = resultTweenDuration / resultTurnsAmount;
        
        sequenceFlip.SetEase(Ease.InOutFlash);

        int loop = 0;
        while (playerTurnStack.Count > 0)
        {
            InputType inputType = playerTurnStack.Pop();
            if (inputType == InputType.Rotate)
            {
                targetRotation.z = loop%2 == 0 ? 360 : -360;
                sequenceFlip.Append(transform.DORotate(targetRotation, rotationTime).SetRelative(true).SetEase(Ease.InOutQuad));
            }
            else
            {
                sequenceFlip.Append(transform.DOMoveX(xMovementWhenBluffing, rotationTime / 4).SetEase(Ease.Linear));
                sequenceFlip.Append(transform.DOMoveX(-xMovementWhenBluffing, rotationTime / 2).SetEase(Ease.Linear));
                sequenceFlip.Append(transform.DOMoveX(0, rotationTime / 4).SetEase(Ease.Linear));
            }
            ++loop;
        }

        float finalResultTurnsDuration = rotationTime;
        if (finalWinningSide == PlayerID.Player2)
        {
            sequenceFlip.Append(transform.DORotate(new Vector3(0, 0, 180), rotationTime / 2)
                .SetRelative(true).SetEase(Ease.OutQuad));
            finalResultTurnsDuration *= resultTurnsAmount + 0.5f;
        }
        else
            finalResultTurnsDuration *= resultTurnsAmount;
        
        sequenceUpMovement.SetEase(Ease.InOutBounce);
        sequenceUpMovement.Append(transform.DOMoveY(resultHeightChange, finalResultTurnsDuration / 2).SetEase(Ease.Linear));
        sequenceUpMovement.Append(transform.DOMoveY(0, finalResultTurnsDuration / 2).SetEase(Ease.Linear));

        
        Sequence finalSequence = DOTween.Sequence();
        finalSequence.Append(sequenceFlip);
        finalSequence.Join(sequenceUpMovement);
        return finalSequence;
    }
}
