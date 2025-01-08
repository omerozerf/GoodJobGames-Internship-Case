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
            var obj = Object.Instantiate(m_Prefab, m_Parent);
            obj.gameObject.SetActive(false);
            m_Objects.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (m_Objects.Count > 0)
        {
            var obj = m_Objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        var newObj = Object.Instantiate(m_Prefab, m_Parent);
        return newObj;
    }

    public void Return(T obj, Transform parent)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        m_Objects.Enqueue(obj);
    }
}