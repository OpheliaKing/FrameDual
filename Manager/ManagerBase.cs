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

        if (current == null && missing.Add(typeof(T)))
            Debug.LogError(typeof(T).Name + "가 GameManager 아래에 없어.");

        return current;
    }
}
