using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordSpawner : MonoBehaviour
{
    [SerializeField] float SpawnTime, SpawnAmount;
    [SerializeField] GameObject SpawnPrefab;

    private float _timer, _amountSpawned;

    private void Start()
    {
        _timer = 0f;
        _amountSpawned = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > SpawnTime && _amountSpawned < SpawnAmount)
        {
            Instantiate(SpawnPrefab, this.transform);
            _timer = 0f;
            _amountSpawned++;
        }
    }
}
