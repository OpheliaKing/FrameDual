using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : ManagerBase
{
    class Slot
    {
        public bool Released;
        public bool Delivered;
        public System.Action<Object> Callback;
    }

    class LoadEntry
    {
        public AsyncOperationHandle Handle;
        public bool Started;
        public readonly List<Slot> Slots = new List<Slot>();
    }

    readonly Dictionary<string, LoadEntry> entries = new Dictionary<string, LoadEntry>();
    readonly Dictionary<GameObject, string> instances = new Dictionary<GameObject, string>();

    void Start()
    {
        Addressables.InitializeAsync().Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
                Debug.LogError("어드레서블 초기화에 실패했어.");
        };
    }

    public void Load<T>(string address, System.Action<T> onLoaded) where T : Object
    {
        if (string.IsNullOrEmpty(address) || onLoaded == null)
            return;

        if (!entries.TryGetValue(address, out LoadEntry entry))
        {
            entry = new LoadEntry();
            entries.Add(address, entry);
        }

        Slot slot = new Slot { Callback = asset => onLoaded(asset as T) };
        entry.Slots.Add(slot);

        if (entry.Started && entry.Handle.IsDone)
        {
            Deliver(slot, entry.Handle.Status == AsyncOperationStatus.Succeeded ? entry.Handle.Result as Object : null);
            return;
        }

        if (entry.Started)
            return;

        entry.Started = true;
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        entry.Handle = handle;
        handle.Completed += completed => Finish(address, completed.Status == AsyncOperationStatus.Succeeded ? completed.Result : null);
    }

    public void Release(string address)
    {
        if (string.IsNullOrEmpty(address) || !entries.TryGetValue(address, out LoadEntry entry))
            return;

        for (int i = 0; i < entry.Slots.Count; i++)
        {
            if (entry.Slots[i].Released)
                continue;

            entry.Slots[i].Released = true;
            break;
        }

        if (entry.Started && entry.Handle.IsDone && !HasLiveSlot(entry))
            Remove(address);
    }

    public void Instantiate(string address, Transform parent, System.Action<GameObject> onSpawned)
    {
        if (string.IsNullOrEmpty(address) || onSpawned == null)
            return;

        Load<GameObject>(address, prefab =>
        {
            if (prefab == null)
            {
                Release(address);
                onSpawned(null);
                return;
            }

            GameObject instance = Object.Instantiate(prefab);
            instance.SetActive(false);

            if (parent != null)
                instance.transform.SetParent(parent, false);

            instances[instance] = address;
            onSpawned(instance);
        });
    }

    public void ReleaseInstance(GameObject instance)
    {
        if (instance == null)
            return;

        if (!instances.TryGetValue(instance, out string address))
        {
            if (!Addressables.ReleaseInstance(instance))
                Destroy(instance);
            return;
        }

        instances.Remove(instance);
        Destroy(instance);
        Release(address);
    }

    void Finish(string address, Object asset)
    {
        if (!entries.TryGetValue(address, out LoadEntry entry))
            return;

        if (asset == null)
            Debug.LogError("어드레서블을 못 불러왔어. " + address);

        for (int i = 0; i < entry.Slots.Count; i++)
        {
            if (!entry.Slots[i].Released)
                Deliver(entry.Slots[i], asset);
        }

        if (!HasLiveSlot(entry))
            Remove(address);
    }

    static void Deliver(Slot slot, Object asset)
    {
        if (slot.Delivered || slot.Released)
            return;

        slot.Delivered = true;
        slot.Callback?.Invoke(asset);
    }

    static bool HasLiveSlot(LoadEntry entry)
    {
        for (int i = 0; i < entry.Slots.Count; i++)
        {
            if (!entry.Slots[i].Released)
                return true;
        }

        return false;
    }

    void Remove(string address)
    {
        if (!entries.TryGetValue(address, out LoadEntry entry))
            return;

        if (entry.Handle.IsValid())
            Addressables.Release(entry.Handle);

        entries.Remove(address);
    }
}
