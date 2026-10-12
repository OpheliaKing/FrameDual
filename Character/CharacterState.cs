public abstract class CharacterState
{
    public CharacterBase Owner { get; private set; }

    public void Bind(CharacterBase owner)
    {
        Owner = owner;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Tick()
    {
    }

    public virtual void FixedTick()
    {
    }
}
