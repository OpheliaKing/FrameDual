using UnityEngine;

public class MapBase : MonoBehaviour
{
    [SerializeField] Transform playerStart;
    [SerializeField] Transform bossStart;

    public Transform PlayerStart => playerStart;
    public Transform BossStart => bossStart;

    public void PlacePlayer(CharacterBase character)
    {
        Place(character, playerStart, "플레이어");
    }

    public void PlaceBoss(CharacterBase character)
    {
        Place(character, bossStart, "보스");
    }

    void Place(CharacterBase character, Transform point, string label)
    {
        if (character == null)
            return;

        if (point == null)
        {
            Debug.LogWarning(name + " " + label + " 시작 지점 없음. 맵 위치 사용.");
            point = transform;
        }

        character.transform.SetPositionAndRotation(point.position, point.rotation);
    }
}
