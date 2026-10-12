using UnityEngine;

public class InGameManager : ManagerBase
{
    const string CharacterTableAddress = "CharacterTableSO";
    const string MapTableAddress = "MapTableSO";
    const string InputDebugAddress = "InputDebugUI";
    const int StageTid = 1;
    const int PlayerTid = 1;

    CharacterTableSO characterTable;
    MapTableSO mapTable;
    GameObject stageObject;
    GameObject playerObject;
    GameObject inputDebugObject;
    CharacterBase player;
    int runId;
    bool loading;

    public CharacterBase Player => player;

    public void StartInGame()
    {
        if (loading)
            return;

        if (characterTable != null && mapTable != null)
        {
            BeginStage();
            return;
        }

        ResourceManager resource = GameManager.Instance != null ? GameManager.Instance.Resource : null;
        if (resource == null)
        {
            Debug.LogError("리소스 매니저 없음.");
            return;
        }

        loading = true;
        resource.Load<CharacterTableSO>(CharacterTableAddress, loadedCharacter =>
        {
            resource.Load<MapTableSO>(MapTableAddress, loadedMap =>
            {
                loading = false;

                if (loadedCharacter == null)
                {
                    Debug.LogError("캐릭터 테이블 없음.");
                    resource.Release(CharacterTableAddress);
                }
                else
                    characterTable = loadedCharacter;

                if (loadedMap == null)
                {
                    Debug.LogError("맵 테이블 없음.");
                    resource.Release(MapTableAddress);
                }
                else
                    mapTable = loadedMap;

                if (characterTable == null || mapTable == null)
                    return;

                BeginStage();
            });
        });
    }

    void BeginStage()
    {
        MapData stage = mapTable.Find(StageTid);
        CharacterData playerData = characterTable.Find(PlayerTid);

        if (stage == null || string.IsNullOrEmpty(stage.prefabAddress))
        {
            Debug.LogError("스테이지 데이터 없음. tid " + StageTid);
            return;
        }

        if (playerData == null || string.IsNullOrEmpty(playerData.prefabName))
        {
            Debug.LogError("플레이어 데이터 없음. tid " + PlayerTid);
            return;
        }

        runId++;
        int id = runId;
        ClearStage();

        ResourceManager resource = GameManager.Instance.Resource;
        resource.Instantiate(stage.prefabAddress, null, mapObject =>
        {
            if (id != runId)
            {
                resource.ReleaseInstance(mapObject);
                return;
            }

            if (mapObject == null)
                return;

            MapBase map = mapObject.GetComponent<MapBase>();
            if (map == null)
            {
                Debug.LogError("맵 프리팹 MapBase 없음. " + stage.prefabAddress);
                resource.ReleaseInstance(mapObject);
                return;
            }

            stageObject = mapObject;
            mapObject.SetActive(true);
            SpawnPlayer(resource, map, playerData, id);
        });
    }

    void SpawnPlayer(ResourceManager resource, MapBase map, CharacterData playerData, int id)
    {
        resource.Instantiate(playerData.prefabName, map.transform, spawned =>
        {
            if (id != runId)
            {
                resource.ReleaseInstance(spawned);
                return;
            }

            if (spawned == null)
                return;

            CharacterBase character = spawned.GetComponent<CharacterBase>();
            if (character == null)
            {
                Debug.LogError("플레이어 프리팹 CharacterBase 없음. " + playerData.prefabName);
                resource.ReleaseInstance(spawned);
                return;
            }

            playerObject = spawned;
            player = character;
            character.Apply(playerData);
            map.PlacePlayer(character);
            spawned.SetActive(true);

            CameraManager cameraManager = GameManager.Instance.Camera;
            if (cameraManager != null)
                cameraManager.Move();

            ShowInputDebug(resource, id);
        });
    }

    void ShowInputDebug(ResourceManager resource, int id)
    {
        resource.Instantiate(InputDebugAddress, null, spawned =>
        {
            if (id != runId)
            {
                resource.ReleaseInstance(spawned);
                return;
            }

            if (spawned == null)
                return;

            InputDebugUI debugUi = spawned.GetComponent<InputDebugUI>();
            if (debugUi == null)
            {
                Debug.LogError("인풋 디버그 UI 없음. " + InputDebugAddress);
                resource.ReleaseInstance(spawned);
                return;
            }

            UIManager ui = GameManager.Instance.UI;
            if (ui == null)
            {
                Debug.LogError("UI 매니저 없음.");
                resource.ReleaseInstance(spawned);
                return;
            }

            inputDebugObject = spawned;
            ui.Open(debugUi);
        });
    }

    void ClearStage()
    {
        if (GameManager.Instance == null || GameManager.Instance.Resource == null)
            return;

        ResourceManager resource = GameManager.Instance.Resource;

        if (playerObject != null)
            resource.ReleaseInstance(playerObject);

        if (stageObject != null)
            resource.ReleaseInstance(stageObject);

        if (inputDebugObject != null)
            resource.ReleaseInstance(inputDebugObject);

        player = null;
        playerObject = null;
        stageObject = null;
        inputDebugObject = null;
    }
}
