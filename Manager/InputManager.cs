using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public enum InputRoute
{
    None,
    Player,
    UI
}

public struct InputFrame
{
    public InputRoute Route;
    public Vector2 Move;
    public bool JumpPressed;
    public bool AttackPressed;
}

[DefaultExecutionOrder(-100)]
public class InputManager : ManagerBase
{
    [SerializeField] InputSystemUIInputModule uiModule;

    InputActionAsset actions;
    InputActionMap playerMap;
    InputActionMap uiMap;
    InputAction move;
    InputAction jump;
    InputAction attack;
    InputAction sprint;
    InputAction rightClick;
    bool ready;

    public InputRoute Route { get; private set; } = InputRoute.Player;
    public Vector2 Move { get; private set; }
    public float MoveX => Move.x;
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool MoveEnabled => move != null && move.enabled;
    public bool JumpEnabled => jump != null && jump.enabled;
    public bool AttackEnabled => attack != null && attack.enabled;
    public bool SprintEnabled => sprint != null && sprint.enabled;
    public bool JumpHeld => jump != null && jump.IsPressed();
    public bool AttackHeld => attack != null && attack.IsPressed();
    public bool SprintHeld => sprint != null && sprint.IsPressed();
    public bool RightClickHeld => rightClick != null && rightClick.IsPressed();

    public event Action<InputFrame> InputChanged;

    void Awake()
    {
        actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("Input Action Asset 없음.");
            return;
        }

        playerMap = actions.FindActionMap("Player", true);
        uiMap = actions.FindActionMap("UI", true);
        move = playerMap.FindAction("Move", true);
        jump = playerMap.FindAction("Jump", true);
        attack = playerMap.FindAction("Attack", true);
        sprint = playerMap.FindAction("Sprint", false);
        rightClick = uiMap.FindAction("RightClick", false);
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
            ClearPlayerInput();
        else
        {
            Move = move.ReadValue<Vector2>();
            JumpPressed = jump.WasPressedThisFrame();
            AttackPressed = attack.WasPressedThisFrame();
        }

        InputChanged?.Invoke(Capture());
    }

    InputFrame Capture()
    {
        return new InputFrame
        {
            Route = Route,
            Move = Move,
            JumpPressed = JumpPressed,
            AttackPressed = AttackPressed
        };
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
        SetAction(sprint, false);

        if (!uiOn && uiMap != null)
            uiMap.Disable();

        if (uiModule != null)
            uiModule.enabled = uiOn;
        else if (uiOn && uiMap != null)
            uiMap.Enable();

        SetAction(rightClick, playerOn || uiOn);

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
