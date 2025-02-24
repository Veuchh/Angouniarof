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
    [SerializeField] private GameObject fakeHourglass;
    [Header("Hourglass settings")]
    [SerializeField] float singleRotationDuration = .3f;
    [SerializeField] float setupTweenDuration = .5f;
    [SerializeField] int setupTurnsAmount = 10;
    [SerializeField] float resultTweenDuration = 1.3f;
    [SerializeField] float xMovementWhenBluffing = 5;
    
    [Header("Audio")]
    [SerializeField] private SFXData rotateSFX;
    [SerializeField] private SFXData endRecapSFX;
    [SerializeField] private SFXData rotateRecapSFX;
    [SerializeField] private SFXData bluffRecapSFX;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem redVFX;
    [SerializeField] private ParticleSystem blueVFX;
    // [SerializeField] int resultTurnsAmount = 30; No need for it anymore, now storing inputs to know how many
    
    private void Awake()
    {
        Instance = this;
    }

    public void ResetHourglassRotation(PlayerID startWinningSide)
    {
        transform.rotation = Quaternion.identity;
        if(startWinningSide == PlayerID.Player2)
            transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public Tween SetDefaultHourglass(PlayerID startWinningSide)
    {
        ResetHourglassRotation(startWinningSide);
        
        Sequence sequence = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;

        //targetRotation.z = 360 * setupTurnsAmount + startWinningSide == PlayerID.Player1 ? 0 : 180;
        targetRotation.z = 360;
        
        float rotationTime = setupTweenDuration / setupTurnsAmount; 
        
        sequence.SetEase(Ease.InOutQuad);
        sequence.Append(transform.DORotate(targetRotation, rotationTime).SetRelative(true).SetLoops(setupTurnsAmount).SetEase(Ease.Linear));

        if (startWinningSide == PlayerID.Player2)
        {
            sequence.Append(transform.DORotate(new Vector3(0, 0, 180), rotationTime/2).SetEase(Ease.Linear));
        }

        //sequence.Append(transform.DORotate(targetRotation, setupTweenDuration));

        return sequence;
    }

    public void ToggleHourglass(bool toggle)
    {
        playerCamera.cullingMask = toggle ? playerCameraShownLayerMask : playerCameraHiddenLayerMask;
        fakeHourglass.transform.rotation = transform.rotation;
    }

    public Tween RotateHourglassToPlayerWinningState(PlayerID playerID)
    {
        Sequence sequence = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;

        targetRotation.z = playerID == PlayerID.Player1 ? 180 : -180;

        sequence.SetEase(Ease.InOutQuad);
        sequence.Append(transform.DORotate(targetRotation, singleRotationDuration).SetRelative(true));

        sequence.OnStart(() =>
        {
            (playerID == PlayerID.Player1 ? redVFX : blueVFX).Play();
            AudioManager.Instance.PlaySFX(rotateSFX);
        });

        return sequence;
    }

    public Tween BluffHourglass(PlayerID playerID)
    {
        Sequence sequence = DOTween.Sequence();
        
        sequence.SetEase(Ease.InOutQuad);
        sequence.Append(transform.DOMoveX(xMovementWhenBluffing, singleRotationDuration / 4).SetEase(Ease.Linear));
        sequence.Append(transform.DOMoveX(-xMovementWhenBluffing, singleRotationDuration / 2).SetEase(Ease.Linear));
        sequence.Append(transform.DOMoveX(0, singleRotationDuration / 4).SetEase(Ease.Linear));
        
        sequence.OnStart(() =>
        {
            (playerID == PlayerID.Player1 ? redVFX : blueVFX).Play();
            AudioManager.Instance.PlaySFX(rotateSFX);
        });
        
        return sequence;
    }

    public Tween ShowHourglassResult(PlayerID startWinningSide)
    {
        ResetHourglassRotation(startWinningSide);

        ToggleHourglass(true);

        Sequence sequenceFlip = DOTween.Sequence();

        Vector3 targetRotation = Vector3.zero;
        Queue<InputType> playerTurnStack = GameManager.Instance.playerInputQueue;
        int resultTurnsAmount = playerTurnStack.Count;
        float rotationTime = resultTweenDuration / resultTurnsAmount;
        
        sequenceFlip.SetEase(Ease.InOutFlash);

        int loop = 0;
        while (playerTurnStack.Count > 0)
        {
            InputType inputType = playerTurnStack.Dequeue();
            if (inputType == InputType.Rotate)
            {
                targetRotation.z = loop%2 == 0 ? 180 : -180;
                int temp = loop;
                sequenceFlip.Append(transform.DORotate(targetRotation, rotationTime).SetRelative(true).SetEase(Ease.InOutQuad)
                    .OnStart(() =>
                    {
                        AudioManager.Instance.PlaySFX(rotateRecapSFX);
                        UIManager.Instance.UpdateRecapMove(inputType, temp);
                    }));
            }
            else
            {
                int temp = loop;
                sequenceFlip.Append(transform.DOMoveX(xMovementWhenBluffing, rotationTime / 16).SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        AudioManager.Instance.PlaySFX(bluffRecapSFX);
                        UIManager.Instance.UpdateRecapMove(inputType, temp);
                    }));
                sequenceFlip.Append(transform.DOMoveX(-xMovementWhenBluffing, rotationTime / 8).SetEase(Ease.Linear));
                sequenceFlip.Append(transform.DOMoveX(0, rotationTime / 16).SetEase(Ease.Linear));
                sequenceFlip.AppendInterval(.15f);
            }
            ++loop;
        }

        // float finalResultTurnsDuration = rotationTime;
        // if (finalWinningSide == PlayerID.Player2)
        // {
        //     sequenceFlip.Append(transform.DORotate(new Vector3(0, 0, 180), rotationTime / 2)
        //         .SetRelative(true).SetEase(Ease.OutQuad));
        //     finalResultTurnsDuration *= resultTurnsAmount + 0.5f;
        // }
        // else
        //     finalResultTurnsDuration *= resultTurnsAmount;

        
        Sequence finalSequence = DOTween.Sequence();
        finalSequence.Append(sequenceFlip);
        finalSequence.OnComplete(() => AudioManager.Instance.PlaySFX(endRecapSFX));
        return finalSequence;
    }
}
