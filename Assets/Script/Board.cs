using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Collections;
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    //public BlockData currentBlockData { get; private set; }

    // Array of block data to draw on tilemap
    public BlockData[] blocks;

    // Array of block prefabs to spawn
    public GameObject[] blockPrefab;

    public Vector2Int boardSize = new Vector2Int(5, 6);
    public Vector3Int spawnPosition = new Vector3Int();

    private ScoreController scoreController;
    bool waiting = false;
    private bool isGameOver = false;

    [SerializeField] private AudioClip mergeSound;
    //[SerializeField] private AudioClip gameOverSound;

    public UnityEvent OnGameOver;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        scoreController = GetComponent<ScoreController>();
    }

    private void Start()
    {
        SpawnRandomBlock(spawnPosition);
    }

    private void Update()
    {
        if (isGameOver) return;
        //if has no blocks, spawn a new one
        if (GetComponentInChildren<Block>() == null)
        {
            SpawnRandomBlock(spawnPosition);
        }
    }

    public void SpawnRandomBlock(Vector3Int gridPos)
    {
        //print($"Spawning block at {spawnPosition}");
        int random = Random.Range(0, blocks.Length/2);

        // Instantiate the block GameObject at the spawn position
        Vector3 worldPos = tilemap.CellToWorld((gridPos));
        worldPos.y += 0.5f; // Adjust the Y position to place the block above the grid
        worldPos.x += 0.5f; // Adjust the X position to place the block in the center of the cell

        GameObject blockObj = Instantiate(blockPrefab[random], worldPos, Quaternion.identity, this.transform);
        blockObj.GetComponent<Block>().canMove = true;
    }

    private void SpawnBlock(Vector3Int gridPos, int Value)
    {
        // Find the prefab index that matches the merged value
        int prefabIndex = -1;
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].value == Value)
            {
                prefabIndex = i;
                break;
            }
        }

        if (prefabIndex != -1)
        {
            Vector3 worldPos = tilemap.CellToWorld(gridPos);
            worldPos.y += 0.5f;
            worldPos.x += 0.5f;

            GameObject newBlockObj = Instantiate(blockPrefab[prefabIndex], worldPos, Quaternion.identity, this.transform);
            newBlockObj.GetComponent<Block>().SetGridPosition(gridPos);
        }
        else
        {
            Debug.LogWarning($"No prefab found for value {Value}");
        }
    }

    public bool IsEmpty(Vector3Int position)
    {
        return tilemap.GetTile(position) == null;
    }

    public bool IsValidMove(Vector3Int position, Vector3Int des)
    {
        if (position.y != des.y)
        {
            print($"Invalid move from {position} to {des}: y coordinates do not match.");
            return false;
        }

        // Use Mathf.Min and Mathf.Max to determine the range for the loop
        int minX = Mathf.Min(position.x, des.x);
        int maxX = Mathf.Max(position.x, des.x);

        for (int x = minX; x <= maxX; x++)
        {
            Vector3Int checkPos = new Vector3Int(x, position.y, 0);
            if (!IsEmpty(checkPos))
            {
                print($"Invalid move from {position} to {des}: Tile at {checkPos} is not empty.");
                return false;
            }
        }
        return true;
    }

    public bool IsValidBoard()
    {
        // Check if the board is empty
        for (int x = Bounds.xMin; x < Bounds.xMax; x++)
        {
            Vector3Int pos = new Vector3Int(x, Bounds.yMax - 1, 0);
            if (!IsEmpty(pos))
            {
                return false; // Board is invalid if top road is occupied
            }
        }
        return true; 
    }

    public bool Merge(Block block)
    {
        Vector3Int[] checkPos = new Vector3Int[4];
        Vector3Int gridPos = block.gridPosition;
        int blockValue = block.data.value;
        int multiplier = 1;

        // Up
        checkPos[0] = new Vector3Int(gridPos.x, gridPos.y + 1, gridPos.z);
        // Down
        checkPos[1] = new Vector3Int(gridPos.x, gridPos.y - 1, gridPos.z);
        // Left
        checkPos[2] = new Vector3Int(gridPos.x - 1, gridPos.y, gridPos.z);
        // Right
        checkPos[3] = new Vector3Int(gridPos.x + 1, gridPos.y, gridPos.z);

        // Check each position around the block
        foreach (Vector3Int check in checkPos)
        {
            if (tilemap.GetTile(check) != null)
            {
                Tile tile = tilemap.GetTile<Tile>(check);
                var blockData = GetBlockDataFromTile(tile);
                if (blockData.HasValue && blockData.Value.value == blockValue)
                {
                    ClearTile(check);
                    multiplier *= 2;
                }
            }
        }

        //Initialize a new block with the merged value at position if multiplier is greater than 1
        if (multiplier > 1)
        {
            SoundFXmanager.Instance.PlaySound(mergeSound, block.transform, 1.0f);

            int newValue = blockValue * multiplier;
            scoreController.AddScore(newValue);

            //Spawn a new block at the original position of the merged block
            SpawnBlock(gridPos, newValue);
            return true;
        }
        else
        {
            // If no merge happened, just set the tile back to the original block's tile
            tilemap.SetTile(gridPos, block.data.tile);
            return false;
        }
    }

    private BlockData? GetBlockDataFromTile(Tile tile)
    {
        foreach (var data in blocks)
        {
            if (data.tile == tile)
                return data;
        }
        return null;
    }

    public Tile GetTileFromValue(int value)
    {
        foreach (var blockData in blocks)
        {
            if (blockData.value == value)
                return blockData.tile;
        }
        return null; // Or handle the case where no tile is found
    }

    public void UpdateSpawnPos(Vector3Int position)
    {
        spawnPosition = new Vector3Int(position.x, spawnPosition.y, 0);
    }

    public void GameOver()
    {
        if (isGameOver) return; // Prevent multiple activations
        isGameOver = true;

        //SoundFXmanager.Instance.PlaySound(gameOverSound, transform, 1.0f);
        OnGameOver.Invoke();
    }

    public void Set(Block Block)
    {
        tilemap.SetTile(Block.gridPosition, Block.data.tile);
    }

    public void ClearTile(Vector3Int pos)
    {
        tilemap.SetTile(pos, null);

        //Check if there are still non-empty tiles above
        for (int y = pos.y + 1; y < Bounds.yMax; y++)
        {
            Vector3Int checkPos = new Vector3Int(pos.x, y, 0);
            print($"Checking for blocks above at {checkPos}");
            if (tilemap.GetTile(checkPos) != null)
            {
                print($"Block found above at {checkPos}");
                var blockData = GetBlockDataFromTile(tilemap.GetTile<Tile>(checkPos));
                tilemap.SetTile(checkPos, null);
                SpawnBlock(checkPos, blockData.HasValue ? blockData.Value.value : 0);
            }
        }
    }

    public void Stop(float duration, float timeScale)
    {
        if (waiting)
            return;
        Time.timeScale = timeScale;
        StartCoroutine(Wait(duration));
    }

    public void Stop(float duration)
    {
        Stop(duration, 0.0f);
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}
