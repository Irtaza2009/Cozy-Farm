using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<FarmResourceType, int> resources =
        new Dictionary<FarmResourceType, int>();

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
    }

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
}
