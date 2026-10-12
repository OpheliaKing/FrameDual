using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public enum InputRoute
{
    None,
    Player,
    UI
}

[DefaultExecutionOrder(-100)]
public class InputManager : ManagerBase
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] InputSystemUIInputModule uiModule;

    InputActionMap playerMap;
    InputActionMap uiMap;
    InputAction move;
    InputAction jump;
    InputAction attack;
    bool ready;

    public InputRoute Route { get; private set; } = InputRoute.Player;
    public Vector2 Move { get; private set; }
    public float MoveX => Move.x;
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    void Awake()
    {
        if (actions == null)
        {
            Debug.LogError("InputManager에 Input Action Asset을 넣어.");
            return;
        }

        playerMap = actions.FindActionMap("Player", true);
        uiMap = actions.FindActionMap("UI", true);
        move = playerMap.FindAction("Move", true);
        jump = playerMap.FindAction("Jump", true);
        attack = playerMap.FindAction("Attack", true);
    }

    void Start()
    {
        if (actions == null)
            return;

        if (uiModule == null)
            uiModule = FindAnyObjectByType<InputSystemUIInputModule>();

        if (uiModule != null)
            uiModule.actionsAsset = actions;

        ready = true;
        ApplyRoute();
    }

    void Update()
    {
        if (Route != InputRoute.Player || move == null)
        {
            ClearPlayerInput();
            return;
        }

        Move = move.ReadValue<Vector2>();
        JumpPressed = jump.WasPressedThisFrame();
        AttackPressed = attack.WasPressedThisFrame();
    }

    public void SetRoute(InputRoute route)
    {
        Route = route;
        if (ready)
            ApplyRoute();
    }

    public void Block()
    {
        SetRoute(InputRoute.None);
    }

    void ApplyRoute()
    {
        bool playerOn = Route == InputRoute.Player;
        bool uiOn = Route == InputRoute.UI;

        SetAction(move, playerOn);
        SetAction(jump, playerOn);
        SetAction(attack, playerOn);

        if (!uiOn && uiMap != null)
            uiMap.Disable();

        if (uiModule != null)
            uiModule.enabled = uiOn;
        else if (uiOn && uiMap != null)
            uiMap.Enable();

        if (!playerOn)
            ClearPlayerInput();
    }

    void ClearPlayerInput()
    {
        Move = Vector2.zero;
        JumpPressed = false;
        AttackPressed = false;
    }

    static void SetAction(InputAction action, bool enabled)
    {
        if (action == null)
            return;

        if (enabled)
            action.Enable();
        else
            action.Disable();
    }
}
