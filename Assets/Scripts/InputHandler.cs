using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public void OnPlayer1Fake(InputValue value)
    {
        GameManager.Instance.OnPlayerInput(PlayerID.Player1, InputType.Fake);
    }

    public void OnPlayer1Rotate(InputValue value)
    {
        GameManager.Instance.OnPlayerInput(PlayerID.Player1, InputType.Rotate);
    }

    public void OnPlayer2Fake(InputValue value)
    {
        GameManager.Instance.OnPlayerInput(PlayerID.Player2, InputType.Fake);
    }

    public void OnPlayer2Rotate(InputValue value)
    {
        GameManager.Instance.OnPlayerInput(PlayerID.Player2, InputType.Rotate);
    }
}
