using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Transform PoolsParentTransform;

    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }
    public List<Pool> Pools;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;

    #region Singleton
    public static ObjectPooler pool;
    private void Awake()
    {
        pool = this;
    }
    #endregion

    private void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();
        for(int i = 0; i < Pools.Count; i++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int j = 0; j < Pools[i].Size; j++)
            {
                GameObject obj = Instantiate(Pools[i].Prefab,Vector3.zero,Quaternion.identity);
                if(PoolsParentTransform)
                {
                    obj.transform.SetParent(PoolsParentTransform);
                }
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            PoolDictionary.Add(Pools[i].Tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Not have tag in this pool");
            return null;
        }
        else
        {
            for (int i = 0; i < PoolDictionary[tag].Count; i++)
            {
                GameObject temp = PoolDictionary[tag].Dequeue();

                if (!temp.activeInHierarchy)
                {
                    temp.transform.position = pos;
                    temp.transform.rotation = rot;
                    temp.SetActive(true);
                    PoolDictionary[tag].Enqueue(temp);
                    return temp;
                }
                else
                {
                    PoolDictionary[tag].Enqueue(temp);
                }
            }

            for(int i = 0; i < Pools.Count; i++)
            {
                if(Pools[i].Tag == tag)
                {
                    GameObject obj = Instantiate(Pools[i].Prefab, Vector3.zero, Quaternion.identity);
                    if (PoolsParentTransform)
                    {
                        obj.transform.SetParent(PoolsParentTransform);
                    }
                    obj.transform.position = pos;
                    obj.transform.rotation = rot;
                    obj.SetActive(true);
                    PoolDictionary[tag].Enqueue(obj);
                    return obj;
                }
            }

            Debug.LogError("ObjectPooler ERROR");
            return null;
        }
    }
    public GameObject GetFromPool(string tag)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Not have tag in this pool");
            return null;
        }
        else
        {
            for (int i = 0; i < PoolDictionary[tag].Count; i++)
            {
                GameObject temp = PoolDictionary[tag].Dequeue();

                if (!temp.activeInHierarchy)
                {
                    temp.SetActive(true);
                    PoolDictionary[tag].Enqueue(temp);
                    return temp;
                }
                else
                {
                    PoolDictionary[tag].Enqueue(temp);
                }
            }

            for(int i = 0; i < Pools.Count; i++)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                if(Pools[i].Tag == tag)
                {
                    GameObject obj = Instantiate(Pools[i].Prefab, Vector3.zero, Quaternion.identity);
                    if (PoolsParentTransform)
                    {
                        obj.transform.SetParent(PoolsParentTransform);
                    }
                    obj.SetActive(true);
                    objectPool.Enqueue(obj);
                    return obj;
                }
            }

            Debug.LogError("ObjectPooler ERROR");
            return null;
        }
    }
}