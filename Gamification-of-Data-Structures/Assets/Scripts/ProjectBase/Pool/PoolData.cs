using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// fatherGameObject 是这一类对象的容器 父节点
/// poolList 是池中的对象容器
/// </summary>
public class PoolData
{ 
    public GameObject fatherGameObject;
    public List<GameObject> poolList;

    public PoolData(GameObject go, GameObject poolGO)
    {
        fatherGameObject = new GameObject(go.name);
        fatherGameObject.transform.SetParent(poolGO.transform, false);
        poolList = new List<GameObject>() { };
        PushGameObject(go);
    }

    public GameObject GetGameObject()
    {
        if (poolList.Count == 0 || poolList[0] == null)
        {
            Debug.LogWarning("Pool is empty or object is destroyed");
            return null;
        }

        GameObject go = poolList[0];
        poolList.RemoveAt(0);
        go.SetActive(true);
        go.transform.SetParent(null, false);
        return go;
    }

    public void PushGameObject(GameObject go)
    {
        if (go == null)
        {
            Debug.LogWarning("Trying to push null object to pool");
            return;
        }

        go.SetActive(false);
        poolList.Add(go);
        if (go.transform is RectTransform)
        {
            go.transform.SetParent(fatherGameObject.transform, false);
            RectTransform rectTrans = go.transform as RectTransform;
            rectTrans.localPosition = Vector3.zero;
            rectTrans.localScale = Vector3.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;
        }
        else
        {
            go.transform.SetParent(fatherGameObject.transform, false);
        }
    }
}
