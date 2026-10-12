using UnityEngine;

public class Player : CharacterBase
{
    void Update()
    {
        ReadInput();
        base.Update();
    }

    void ReadInput()
    {
        float moveX = 0f;
        bool jumpPressed = false;

        if (GameManager.Instance != null)
        {
            InputManager input = GameManager.Instance.Input;
            if (input != null)
            {
                moveX = input.MoveX;
                jumpPressed = input.JumpPressed;
            }
        }

        SetIntent(moveX, jumpPressed);
    }
}
