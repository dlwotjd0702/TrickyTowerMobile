using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPoolItem
{
    string key { get; set; }
    GameObject obj { get; }

    void ReturnToPool();
}

public class ObjectPool : MonoBehaviour
{
    private Queue<IObjectPoolItem> Pool { get; set; }
    private IObjectPoolItem Item { get; set; }
    private Transform Parent { get; set; }
    private byte ExpandSize { get; set; }

    public void Initialize(IObjectPoolItem item, Transform parent, byte expandSize, string key)
    {
        Pool = new Queue<IObjectPoolItem>();
        Item = item;
        Item.obj.SetActive(false);
        Parent = parent;
        ExpandSize = expandSize;
        Item.key = key;
        Expand();
    }

    public IObjectPoolItem GetItem()
    {
        if (Pool.Count == 0)
        {
            Expand();
        }

        return Pool.Dequeue();
    }

    private void Expand()
    {
        for (int i = 0; i < ExpandSize; i++)
        {
            var instance = GameObject.Instantiate(Item.obj, Parent).GetComponent<IObjectPoolItem>();
            instance.key = Item.key;
            ReturnToPool(instance);
        }
    }

    public void ReturnToPool(IObjectPoolItem item)
    {
        item.obj.SetActive(false);
        Pool.Enqueue(item);
    }
}