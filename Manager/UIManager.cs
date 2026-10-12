using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase
{
    readonly List<UIBase> commons = new List<UIBase>();
    readonly List<UIBase> popups = new List<UIBase>();

    public void Open(UIBase ui)
    {
        if (ui == null)
            return;

        if (ui.Type == UIType.Common)
            OpenCommon(ui);
        else if (ui.Type == UIType.Popup)
            OpenPopup(ui);
        else
            Debug.LogWarning("None 타입 UI는 아직 스택에 넣지 않아.");
    }

    public void Close()
    {
        if (popups.Count > 0)
            CloseTop(popups, false);
        else if (commons.Count > 0)
            CloseTop(commons, true);

        RefreshInput();
    }

    public void Close(UIBase ui)
    {
        if (ui == null)
            return;

        if (ui.Type == UIType.Popup)
            ClosePopup(ui);
        else if (ui.Type == UIType.Common)
            CloseCommon(ui);

        RefreshInput();
    }

    void OpenCommon(UIBase ui)
    {
        CloseAllPopups();

        int index = commons.IndexOf(ui);
        if (index >= 0)
            commons.RemoveAt(index);

        if (commons.Count > 0)
            Hide(commons[commons.Count - 1]);

        commons.Add(ui);
        Show(ui);
        RefreshInput();
    }

    void OpenPopup(UIBase ui)
    {
        if (popups.Contains(ui))
        {
            ui.transform.SetAsLastSibling();
            RefreshInput();
            return;
        }

        popups.Add(ui);
        Show(ui);
        RefreshInput();
    }

    void CloseCommon(UIBase ui)
    {
        int index = commons.IndexOf(ui);
        if (index < 0)
            return;

        bool wasTop = index == commons.Count - 1;
        commons.RemoveAt(index);
        ui.NotifyClosed();
        ui.gameObject.SetActive(false);

        if (wasTop && commons.Count > 0)
            Show(commons[commons.Count - 1]);
    }

    void ClosePopup(UIBase ui)
    {
        int index = popups.IndexOf(ui);
        if (index < 0)
            return;

        popups.RemoveAt(index);
        ui.NotifyClosed();
        ui.gameObject.SetActive(false);
    }

    void CloseAllPopups()
    {
        for (int i = popups.Count - 1; i >= 0; i--)
        {
            UIBase ui = popups[i];
            ui.NotifyClosed();
            ui.gameObject.SetActive(false);
        }

        popups.Clear();
    }

    void CloseTop(List<UIBase> stack, bool showNext)
    {
        int last = stack.Count - 1;
        UIBase ui = stack[last];
        stack.RemoveAt(last);
        ui.NotifyClosed();
        ui.gameObject.SetActive(false);

        if (showNext && stack.Count > 0)
            Show(stack[stack.Count - 1]);
    }

    static void Show(UIBase ui)
    {
        ui.transform.SetAsLastSibling();
        ui.gameObject.SetActive(true);
        ui.NotifyShown();
    }

    static void Hide(UIBase ui)
    {
        ui.NotifyHidden();
        ui.gameObject.SetActive(false);
    }

    void RefreshInput()
    {
        if (GameManager.Instance == null || GameManager.Instance.Input == null)
            return;

        bool uiOpen = commons.Count > 0 || popups.Count > 0;
        GameManager.Instance.Input.SetRoute(uiOpen ? InputRoute.UI : InputRoute.Player);
    }
}
