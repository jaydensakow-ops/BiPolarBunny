using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// This script watches for all enemies to be gone, then spawns a new one
// after a random delay between MinSpawnDelay and MaxSpawnDelay seconds
// Enemies only spawn on tiles belonging to the Grass tilemap
public class EnemySpawner : MonoBehaviour
{
    // Drag the enemy prefab here in the Inspector
    public GameObject enemyPrefab;

    // Drag the Grass Tilemap object here in the Inspector
    public Tilemap grassTilemap;

    // The shortest possible wait time before spawning the next enemy (in seconds)
    public float minSpawnDelay = GameParameters.EnemyMinSpawnDelay;

    // The longest possible wait time before spawning the next enemy (in seconds)
    public float maxSpawnDelay = GameParameters.EnemyMaxSpawnDelay;

    // How many times to try finding a valid grass tile before giving up
    public int maxSpawnAttempts = GameParameters.EnemyMaxSpawnAttempts;

    // Tracks whether a spawn countdown is already running
    // Prevents us from starting multiple countdowns at the same time
    private bool isSpawnCountdownRunning = false;

    private void Update()
    {
        // If all enemies are gone and no spawn is already scheduled, start the countdown
        if (AreAllEnemiesGone() && isSpawnCountdownRunning == false)
        {
            StartCoroutine(SpawnAfterDelay());
        }
    }

    // Returns true if there are no GameObjects with the "Enemy" tag in the scene
    private bool AreAllEnemiesGone()
    {
        // FindGameObjectsWithTag returns an array of every active Enemy in the scene
        // If the array is empty, all enemies have been destroyed
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }

    // Waits a random delay, then spawns a new enemy
    // This is a coroutine — it runs alongside the rest of the game without blocking it
    private IEnumerator SpawnAfterDelay()
    {
        // Mark the countdown as running so Update doesn't start a second one
        isSpawnCountdownRunning = true;

        // Pick a random wait time between the min and max values
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);

        // Pause this coroutine for 'delay' seconds, then continue from here
        // The rest of the game keeps running normally during the wait
        yield return new WaitForSeconds(delay);

        // Spawn the enemy, then mark the countdown as finished
        SpawnEnemy();
        isSpawnCountdownRunning = false;
    }

    // Spawns a new enemy at a random grass tile position
    private void SpawnEnemy()
    {
        // Use SpawnTools to get a random world position, then check if it lands on a grass tile
        // We retry up to maxSpawnAttempts times since random points may miss the tilemap
        Vector3? spawnPosition = GetRandomGrassPosition();

        // If we couldn't find a valid grass tile, log a warning and skip this spawn
        if (spawnPosition == null)
        {
            Debug.LogWarning("EnemySpawner: Could not find a valid grass tile to spawn on after " + maxSpawnAttempts + " attempts.");
            return;
        }

        // Create a new enemy from the prefab at that position
        // Quaternion.identity means no rotation (same as default)
        Instantiate(enemyPrefab, spawnPosition.Value, Quaternion.identity);
    }

    // Tries to find a random world position that lands on a grass tile
    // Uses SpawnTools.RandomLocationWorldSpace() and retries if the point misses a tile
    // Returns null if no valid position was found within maxSpawnAttempts tries
    private Vector3? GetRandomGrassPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Use SpawnTools to get a random world position on screen
            Vector3 randomWorldPosition = SpawnTools.RandomLocationWorldSpace();

            // Convert that world position to a cell coordinate on the tilemap
            // WorldToCell figures out which tile grid cell contains that world point
            Vector3Int cell = grassTilemap.WorldToCell(randomWorldPosition);

            // Check if there is actually a grass tile at that cell
            if (grassTilemap.HasTile(cell))
            {
                // Return the center of the tile in world space so the enemy lands neatly on it
                return grassTilemap.GetCellCenterWorld(cell);
            }
        }

        // Ran out of attempts without finding a grass tile
        return null;
    }
}