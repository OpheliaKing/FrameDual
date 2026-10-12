using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    static readonly HashSet<System.Type> missing = new HashSet<System.Type>();

    public static T Resolve<T>(ref T current, Component owner) where T : ManagerBase
    {
        if (current != null)
            return current;

        if (owner != null)
            current = owner.GetComponentInChildren<T>(true);

        if (current != null)
            return current;

        if (owner == null)
        {
            if (missing.Add(typeof(T)))
                Debug.LogError(typeof(T).Name + " 생성 실패. 상위 객체 없음.");

            return null;
        }

        GameObject child = new GameObject(typeof(T).Name);
        child.transform.SetParent(owner.transform, false);
        current = child.AddComponent<T>();
        return current;
    }
}
