using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public int tid;
    public string prefabName;
    public int maxHp = 100;
    public int attack = 10;
    public float moveSpeed = 5f;
}

[CreateAssetMenu(fileName = "CharacterTableSO", menuName = "FrameDual/Character Table")]
public class CharacterTableSO : ScriptableObject
{
    [SerializeField] List<CharacterData> characters = new List<CharacterData>();

    Dictionary<int, CharacterData> byTid;

    void OnEnable()
    {
        Build();
    }

    void OnValidate()
    {
        Build();
    }

    public CharacterData Find(int tid)
    {
        if (byTid == null)
            Build();

        byTid.TryGetValue(tid, out CharacterData data);
        return data;
    }

    void Build()
    {
        byTid = new Dictionary<int, CharacterData>();

        for (int i = 0; i < characters.Count; i++)
        {
            CharacterData data = characters[i];
            if (data == null)
                continue;

            if (byTid.ContainsKey(data.tid))
            {
                Debug.LogError("CharacterTableSO 중복 tid. " + data.tid);
                continue;
            }

            byTid.Add(data.tid, data);
        }
    }
}
