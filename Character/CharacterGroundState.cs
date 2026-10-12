public class CharacterGroundState : CharacterState
{
    readonly CharacterAirState air;

    public CharacterGroundState(CharacterAirState airState)
    {
        air = airState;
    }

    public override void FixedTick()
    {
        Owner.Move(Owner.IntentMoveX);

        if (!Owner.Grounded())
        {
            Owner.ConsumeJumpIntent();
            Owner.ChangeState(air);
            return;
        }

        if (!Owner.ConsumeJumpIntent())
            return;

        air.ArmJump();
        Owner.ChangeState(air);
    }
}
