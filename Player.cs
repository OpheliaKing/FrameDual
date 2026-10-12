using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
public class Player : MonoBehaviour
{
    PlayerInputReader input;

    public PlayerInputReader Input => input;

    void Awake()
    {
        input = GetComponent<PlayerInputReader>();
    }
}
