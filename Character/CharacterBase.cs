using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CharacterBase : MonoBehaviour
{
    int maxHp = 100;
    int attack = 10;
    float moveSpeed = 5f;
    const float JumpSpeed = 14f;

    Rigidbody2D body;
    readonly ContactPoint2D[] contacts = new ContactPoint2D[6];

    public int MaxHp => maxHp;
    public int Hp { get; private set; }
    public int Attack => attack;
    public float MoveSpeed => moveSpeed;

    void Reset()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 4f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.linearDamping = 0f;
        body.angularDamping = 0f;
        body.sleepMode = RigidbodySleepMode2D.StartAwake;

        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        capsule.isTrigger = false;
        capsule.direction = CapsuleDirection2D.Vertical;
        capsule.size = new Vector2(0.5f, 1f);
        capsule.offset = new Vector2(0f, 0.5f);
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Hp = maxHp;
    }

    public void Apply(CharacterData data)
    {
        if (data == null)
            return;

        maxHp = data.maxHp;
        attack = data.attack;
        moveSpeed = data.moveSpeed;
        Hp = maxHp;
    }

    public void Move(float direction)
    {
        if (body == null)
            return;

        Vector2 velocity = body.linearVelocity;
        velocity.x = Mathf.Clamp(direction, -1f, 1f) * moveSpeed;
        body.linearVelocity = velocity;
    }

    public void Jump()
    {
        if (body == null || !Grounded())
            return;

        Vector2 velocity = body.linearVelocity;
        velocity.y = JumpSpeed;
        body.linearVelocity = velocity;
    }

    bool Grounded()
    {
        if (body == null)
            return false;

        int count = body.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            if (contacts[i].normal.y > 0.5f)
                return true;
        }

        return false;
    }
}
