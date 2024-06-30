using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEdgeEnemySpawner : EnemySpawner
{
    public Player _player;
    public GameObject enemyPrefab;
    public Transform enemySpawnPoint;
    public float spawnInterval = 5f;

    private float timeSinceLastSpawn;
    public MapManager _MapManager;

    public override void SpawnEnemy()
    {
        Vector2 spawnPosition = GetRandomEdgePosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemySpawnPoint);
        enemy.GetComponent<EnemyAI>()._player = _player;
    }
    
    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    private Vector2 GetRandomEdgePosition()
    {
        float x, y;
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // ���
                x = Random.Range(_MapManager.edgeSizeMin.x, _MapManager.edgeSizeMax.x);
                y = _MapManager.edgeSizeMax.y;
                break;
            case 1: // ����
                x = Random.Range(_MapManager.edgeSizeMin.x, _MapManager.edgeSizeMax.x);
                y = _MapManager.edgeSizeMin.y;
                break;
            case 2: // ����
                x = _MapManager.edgeSizeMin.x;
                y = Random.Range(_MapManager.edgeSizeMin.y, _MapManager.edgeSizeMax.y);
                break;
            case 3: // �E��
                x = _MapManager.edgeSizeMax.x;
                y = Random.Range(_MapManager.edgeSizeMin.y, _MapManager.edgeSizeMax.y);
                break;
            default:
                x = 0;
                y = 0;
                break;
        }

        return new Vector2(x, y);
    }
}