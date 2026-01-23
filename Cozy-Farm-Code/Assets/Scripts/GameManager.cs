using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool HasInstance => Instance != null;
    public static event Action<GameManager> OnInstanceReady;

    private Dictionary<FarmResourceType, int> resources =
        new Dictionary<FarmResourceType, int>();

    [Serializable]
    public struct ResourceEntry
    {
        public FarmResourceType type;
        public int amount;
    }

    [Header("Debug / Balancing")]
    [SerializeField] private List<ResourceEntry> startingResources = new List<ResourceEntry>();

    public event Action<FarmResourceType, int> OnResourceChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (FarmResourceType type in Enum.GetValues(typeof(FarmResourceType)))
        {
            resources[type] = 0;
        }

        foreach (var entry in startingResources)
        {
            resources[entry.type] = entry.amount;
        }

        OnInstanceReady?.Invoke(this);
        OnInstanceReady = null; // ensure callbacks fire once
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Ensure there is at least one entry per resource type so it is always editable in the inspector.
        var existing = new HashSet<FarmResourceType>();
        for (int i = 0; i < startingResources.Count; i++)
        {
            existing.Add(startingResources[i].type);
        }

        foreach (FarmResourceType type in Enum.GetValues(typeof(FarmResourceType)))
        {
            if (!existing.Contains(type))
            {
                startingResources.Add(new ResourceEntry { type = type, amount = 0 });
            }
        }
    }
#endif

    public int GetResource(FarmResourceType type)
    {
        return resources[type];
    }

    public void AddResource(FarmResourceType type, int amount)
    {
        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);

        Debug.Log($"{type} +{amount} → {resources[type]}");
    }

    public static void WhenReady(Action<GameManager> callback)
    {
        if (Instance != null)
        {
            callback?.Invoke(Instance);
        }
        else
        {
            OnInstanceReady += callback;
        }
    }
}
