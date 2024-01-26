using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy2s : MonoBehaviour
{
    public float destroyTime = 3.0f;

    private void OnEnable()
    {
        ObjectPool.Instance.CollectGameObject(gameObject, 2F);
    }
}
