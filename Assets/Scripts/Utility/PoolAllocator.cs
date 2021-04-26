using System.Collections.Generic;
using UnityEngine;

public class PoolAllocator<T> where T : new() 
{
    public List<GameObject> gameObjects;
    public List<T> metadata;
    public int active_object_count = 0;

    public GameObject prefab; 

    public PoolAllocator(GameObject prefab, int capacity = 0)
    {
        gameObjects = new List<GameObject>(capacity);
        metadata = new List<T>(capacity);
        this.prefab = prefab;
        for(int i = 0; i < capacity; i++) {
            var obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            gameObjects.Add(obj);
            metadata.Add(new T());
        }
    }
    
    public GameObject CreateInstance(T metadata)
    {
        if (active_object_count >= gameObjects.Count)
        {
            gameObjects.Add(GameObject.Instantiate(prefab));
            this.metadata.Add(metadata);
        }
        var obj = gameObjects[active_object_count];
        obj.SetActive(true);
        this.metadata[active_object_count] = metadata;
        active_object_count++;
        return obj;
    }

    public void DisableInstance(int index) // Veldig vanskeleg å handtera om ting blir destroyed ut of order? Sjekk.
    {
        if (index < 0 || active_object_count <= 0 || index >= gameObjects.Count || gameObjects[index].activeSelf == false)
        {
            Debug.LogWarning("This should not happen!!!!");
            Debug.Log("Index: " + index);
            Debug.Log("Aktive objekt: " + active_object_count);
            Debug.Log("Antal game objects: " + gameObjects.Count);
            Debug.Log("Er objektet aktivt? " + gameObjects[index].activeSelf);
            return;
        }
        active_object_count--;
        var obj = gameObjects[active_object_count];
        var data = metadata[active_object_count];
        gameObjects[index].SetActive(false);
        gameObjects[active_object_count] = gameObjects[index];
        gameObjects[index] = obj;
        metadata[index] = data;
    }

    public void DisableInstance(GameObject obj)
    {
        DisableInstance(GetIndex(obj));
    }

    public int GetIndex(GameObject obj)
    {
        return gameObjects.IndexOf(obj);
    }

    public void Destroy()
    {
        for (int i=0; i< gameObjects.Count; i++)
        {
            GameObject.Destroy(gameObjects[i]);
        }
        gameObjects.Clear();
        metadata.Clear();
        active_object_count = 0;
    }
}
