using UnityEngine;

public enum UIType
{
    None,
    Common,
    Popup
}

public class UIBase : MonoBehaviour
{
    [SerializeField] UIType type = UIType.None;

    public UIType Type => type;

    public void NotifyShown()
    {
        OnShown();
    }

    public void NotifyHidden()
    {
        OnHidden();
    }

    public void NotifyClosed()
    {
        OnClosed();
    }

    protected virtual void OnShown()
    {
    }

    protected virtual void OnHidden()
    {
    }

    protected virtual void OnClosed()
    {
    }
}
