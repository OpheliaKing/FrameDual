using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    InputManager input;
    UIManager ui;
    ResourceManager resource;
    InGameManager inGame;
    CameraManager cameraManager;

    public InputManager Input => ManagerBase.Resolve(ref input, this);
    public UIManager UI => ManagerBase.Resolve(ref ui, this);
    public ResourceManager Resource => ManagerBase.Resolve(ref resource, this);
    public InGameManager InGame => ManagerBase.Resolve(ref inGame, this);
    public CameraManager Camera => ManagerBase.Resolve(ref cameraManager, this);

    private void Start()
    {
        TestGameStart();
    }

    public void TestGameStart()
    {
        if (InGame == null)
            return;

        InGame.StartInGame();
    }
}
