using UnityEngine;

public class Player : CharacterBase
{
    float moveX;
    bool jump;

    void Update()
    {
        ReadInput();
    }

    void ReadInput()
    {
        if (GameManager.Instance == null)
        {
            moveX = 0f;
            return;
        }

        InputManager input = GameManager.Instance.Input;
        moveX = input != null ? input.MoveX : 0f;
        if (input != null && input.JumpPressed)
            jump = true;
    }

    void FixedUpdate()
    {
        Move(moveX);
        if (!jump)
            return;

        Jump();
        jump = false;
    }
}
