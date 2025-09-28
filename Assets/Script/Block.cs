using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Block : MonoBehaviour
{
    public Board board { get; private set; }
    public Vector3Int gridPosition;

    public BlockData data;

    private Rigidbody2D rb;

    public float fallSpeed;

    public bool canMove;
    private bool stopping;

    //Sound effect
    [SerializeField] private AudioClip lockSound;

    [SerializeField] private ParticleSystem lockEffect;

    private void Awake()
    {
        // Get the Board component from the parent GameObject
        board = GetComponentInParent<Board>();
        rb = GetComponent<Rigidbody2D>();

        canMove = false;
        stopping = false;
    }
    void OnEnable()
    {
        InputManager.OnClick += OnLeftClickReceived;
    }

    void OnDisable()
    {
        InputManager.OnClick -= OnLeftClickReceived;
    }

    private void FixedUpdate()
    {
        //if (stopping) return;
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    private void Update()
    {
        CalculateGridPosition();
    }

    private void CalculateGridPosition()
    {
        Vector3 offset = new Vector3(0f, 0.5f, 0f);
        Vector3Int newGridPosition = board.tilemap.WorldToCell(transform.position - offset);
        newGridPosition.x = Mathf.Clamp(newGridPosition.x, board.Bounds.xMin - 1, board.Bounds.xMax - 1);
        newGridPosition.y = Mathf.Clamp(newGridPosition.y, board.Bounds.yMin - 1, board.Bounds.yMax - 1);
        if (board.IsEmpty(newGridPosition) && board.Bounds.Contains((Vector2Int)newGridPosition))
            gridPosition = newGridPosition;

        if (!stopping)
        {
            // If the block is at the top of the board, it should not move down
            if (gridPosition.y == board.Bounds.yMax - 1 && !board.IsEmpty(newGridPosition))
            {
                Lock();
                return;
            }

            // Check if the next tile is occupied or if current grid is at the bottom of the board
            if (!board.IsEmpty(newGridPosition) || !board.Bounds.Contains((Vector2Int)newGridPosition))
            {
                Lock();
                return;
            }
        }
    }

    void OnLeftClickReceived(Vector2 mousePosition)
    {
        Vector3 mouseScreenPos = new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector3Int newGridPos = board.tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mouseScreenPos));

        //is the mouse is out of bounds of the board, ignore
        if (!board.Bounds.Contains((Vector2Int)newGridPos))
        {
            //print($"Mouse click at {newGridPos} is out of bounds of the board, ignoring.");
            return;
        }

        newGridPos = new Vector3Int(newGridPos.x, gridPosition.y, 0);

        // If there is an existing block on the way to the destination, do not move
        if (!board.IsValidMove(gridPosition, newGridPos)) return;

        if (!canMove) return;
        canMove = false;

        // Update the spawn position of the block
        board.UpdateSpawnPos(newGridPos);

        //Only move the block on the X axis
        Vector3 des = board.tilemap.GetCellCenterWorld(newGridPos);
        des = new Vector3(des.x, transform.position.y, 0);
        transform.position = des;

        HardDrop();
    }

    private void HardDrop()
    {
        fallSpeed = 15.0f;
    }

    private void Lock()
    {
        //print($"Locking block at {gridPosition}");
        //board.Set(this);
        // If merge was successful, destroy the block and dont need to spawn a random block
        transform.position = board.tilemap.GetCellCenterWorld(gridPosition);
        fallSpeed = 0.0f;
        SoundFXmanager.Instance.PlaySound(
            lockSound,
            transform,
            0.5f
        );

        SpawnParticle();
        Stop(0.3f);
    }

    private void SpawnParticle()
    {
        if (lockEffect == null) return;
        ParticleSystem effect = Instantiate(
            lockEffect,
            transform.position,
            Quaternion.Euler(-90f, 0f, 0f)
        );
    }

    public void Stop(float duration)
    {
        if (stopping)
            return;

        StartCoroutine(Wait(duration));
    }

    public void SetGridPosition(Vector3Int newGridPosition)
    {
        gridPosition = newGridPosition;
        transform.position = board.tilemap.GetCellCenterWorld(gridPosition);
    }

    IEnumerator Wait(float duration)
    {
        stopping = true;
        board.Set(this);
        yield return new WaitForSecondsRealtime(duration);
        board.tilemap.SetTile(gridPosition, null);
        if(!board.Merge(this))
            if (!board.IsValidBoard())
                board.GameOver();
        Destroy(gameObject);
    }
}