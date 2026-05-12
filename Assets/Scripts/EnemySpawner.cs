using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemySpawner : MonoBehaviour
{
    public Tilemap GrassTilemap;
    public GameObject EnemyPrefab;
    
    private bool isWaitingtoSpawn = false;
    void Update()
    {
        if (AreAllEnemiesGone() && !isWaitingtoSpawn)
        {
            StartCoroutine(SpawnAfterDelay());
        }
    }

    private IEnumerator SpawnAfterDelay()
    {
        isWaitingtoSpawn = true;
        float delay = Random.Range(GameParameters.EnemyMinimumSpawnDelay, GameParameters.EnemyMaximumSpawnDelay);
        yield return new WaitForSeconds(delay);
        Spawn();
        isWaitingtoSpawn = false;
    }

    private void Spawn()
    {
        Vector3 spawnPosition = GetRandomGrassPosition();
        
        Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomGrassPosition()
    {
        while (true)
        {
            Vector3 randomWorldPosition = SpawnTools.RandomLocationWorldSpace();
            
            Vector3Int cell = GrassTilemap.WorldToCell(randomWorldPosition);

            if (GrassTilemap.HasTile(cell))
            {
                return GrassTilemap.GetCellCenterWorld(cell);
            }
        }
    }

    private bool AreAllEnemiesGone()
    {
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        if (enemies.Count == 0)
            return true;
        return false;
    }
}