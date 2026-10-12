using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    public float MoveX => Input != null ? Input.MoveX : 0f;
    public bool JumpPressed => Input != null && Input.JumpPressed;
    public bool AttackPressed => Input != null && Input.AttackPressed;

    static InputManager Input => GameManager.Instance != null ? GameManager.Instance.Input : null;
}
