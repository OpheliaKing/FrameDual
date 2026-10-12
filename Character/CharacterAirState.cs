public class CharacterAirState : CharacterState
{
    CharacterGroundState ground;
    bool jump;

    public void BindGround(CharacterGroundState groundState)
    {
        ground = groundState;
    }

    public void ArmJump()
    {
        jump = true;
    }

    public override void Enter()
    {
        if (!jump)
            return;

        jump = false;
        Owner.Jump();
    }

    public override void FixedTick()
    {
        Owner.Move(Owner.IntentMoveX);

        if (Owner.Grounded() && Owner.VerticalSpeed <= 0f)
        {
            Owner.ChangeState(ground);
            return;
        }

        Owner.ConsumeJumpIntent();
    }
}
