using System.Collections.Generic;
using UnityEngine;


public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> m_Objects = new Queue<T>();
    private readonly T m_Prefab;
    private readonly Transform m_Parent;


    public ObjectPool(T prefab, Transform parent, int initialCount)
    {
        m_Prefab = prefab;
        m_Parent = parent;

        for (int i = 0; i < initialCount; i++)
        {
            var obj = Object.Instantiate(m_Prefab, parent);
            obj.gameObject.SetActive(false);
            m_Objects.Enqueue(obj);
        }
    }


    public T Get()
    {
        T obj;

        if (m_Objects.Count > 0)
        {
            obj = m_Objects.Dequeue();
        }
        else
        {
            obj = Object.Instantiate(m_Prefab, m_Parent);
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj, Transform parent)
    {
        if (!m_Objects.Contains(obj))
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parent);
            m_Objects.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"ObjectPool Return: Object {obj.name} is already in the pool!");
        }
    }
}