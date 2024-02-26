using StackBasedMenuSystem;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuFactory : MonoBehaviour
{
    public static MenuFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    [SerializeField] private Dictionary<Type, BaseMenu> menuPrefabs = new Dictionary<Type, BaseMenu>();

    public void RegisterMenuPrefab<T>(T prefab) where T : BaseMenu
    {
        menuPrefabs[typeof(T)] = prefab;
    }


    public T CreateInstance<T>() where T : BaseMenu
    {
        Type type = typeof(T);
        if (menuPrefabs.TryGetValue(type, out BaseMenu prefab))
        {
            BaseMenu instance = Instantiate(prefab);
            return instance as T;
        }
        else
        {
            Debug.LogError($"No prefab registered for {type.Name}.");
            return null;
        }
    }

}
