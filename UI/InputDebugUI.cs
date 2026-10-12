using UnityEngine;
using UnityEngine.UI;

public class InputDebugUI : UIBase
{
    [SerializeField] Text label;

    void Awake()
    {
        if (label == null)
            return;

        Font font = Font.CreateDynamicFontFromOSFont(new[] { "Malgun Gothic", "맑은 고딕", "Gulim" }, 20);
        if (font != null)
            label.font = font;

        Image panel = GetComponentInChildren<Image>();
        if (panel == null)
            return;

        Texture2D tex = Texture2D.whiteTexture;
        panel.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        panel.type = Image.Type.Simple;
        panel.color = new Color(0.35f, 0.35f, 0.35f, 0.55f);
    }

    void Update()
    {
        if (label == null || GameManager.Instance == null)
            return;

        InputManager input = GameManager.Instance.Input;
        if (input == null)
            return;

        label.text = Axis(input.MoveX) + "\n"
            + Flag("점프", input.JumpHeld) + "\n"
            + Flag("공격", input.AttackHeld) + "\n"
            + Flag("우클릭", input.RightClickHeld) + "\n"
            + Flag("스프린트", input.SprintHeld);
    }

    static string Axis(float x)
    {
        if (x < -0.1f)
            return "이동 왼쪽";

        if (x > 0.1f)
            return "이동 오른쪽";

        return "이동 없음";
    }

    static string Flag(string name, bool held)
    {
        return held ? name + " ●" : name;
    }
}
