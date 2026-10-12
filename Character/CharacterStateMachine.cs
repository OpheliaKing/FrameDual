public class CharacterStateMachine
{
    public CharacterState Current { get; private set; }

    public void Change(CharacterState next, CharacterBase owner)
    {
        if (next == null || Current == next)
            return;

        next.Bind(owner);
        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Tick()
    {
        Current?.Tick();
    }

    public void FixedTick()
    {
        Current?.FixedTick();
    }
}
