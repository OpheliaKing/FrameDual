using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] InputManager input;
    [SerializeField] UIManager ui;
    [SerializeField] ResourceManager resource;

    public InputManager Input => ManagerBase.Resolve(ref input, this);
    public UIManager UI => ManagerBase.Resolve(ref ui, this);
    public ResourceManager Resource => ManagerBase.Resolve(ref resource, this);
}
