using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public int tid;
    public string prefabAddress;
}

[CreateAssetMenu(fileName = "MapTableSO", menuName = "FrameDual/Map Table")]
public class MapTableSO : ScriptableObject
{
    [SerializeField] List<MapData> maps = new List<MapData>();

    Dictionary<int, MapData> byTid;

    void OnEnable()
    {
        Build();
    }

    void OnValidate()
    {
        Build();
    }

    public MapData Find(int tid)
    {
        if (byTid == null)
            Build();

        byTid.TryGetValue(tid, out MapData data);
        return data;
    }

    void Build()
    {
        byTid = new Dictionary<int, MapData>();

        for (int i = 0; i < maps.Count; i++)
        {
            MapData data = maps[i];
            if (data == null)
                continue;

            if (byTid.ContainsKey(data.tid))
            {
                Debug.LogError("MapTableSO 중복 tid. " + data.tid);
                continue;
            }

            byTid.Add(data.tid, data);
        }
    }
}
