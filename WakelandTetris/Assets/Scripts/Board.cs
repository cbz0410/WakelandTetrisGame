using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public Tilemap tilemap {get; private set;}
    public Piece activePiece {get; private set;}
    public Piece nextPiece {get; private set;}
    public Piece savedPiece {get; private set;}

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector3Int previewPosition = new Vector3Int(9, 6, 0);
    public Vector3Int holdPosition = new Vector3Int(-10, 6, 0);

    public int linesCleared = 0;
    private bool[] levels = new bool[3];

    public RectInt Bounds {
        get {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake() {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        nextPiece = gameObject.AddComponent<Piece>();
        nextPiece.enabled = false;

        savedPiece = gameObject.AddComponent<Piece>();
        savedPiece.enabled = false;

        for(int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void SetNextPiece() {
        if(nextPiece.cells != null) {
            Clear(nextPiece);
        }

        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        nextPiece.Initialize(this, previewPosition, data);
        Set(nextPiece);
    }

    private void Start() {
        SetNextPiece();
        SpawnPiece();
    }

    public void Restart() {
        SceneManager.LoadScene("Tetris");
    }

    public void SpawnPiece() {
        activePiece.Initialize(this, spawnPosition, nextPiece.data);

        if(IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }

        SetNextPiece();
    }

    public void SwapPiece() {
        TetrominoData savedData = savedPiece.data;

        if(savedData.cells == null) {
            savedPiece.Initialize(this, holdPosition, activePiece.data);
            Set(savedPiece);

            Clear(activePiece);
            activePiece.Initialize(this, spawnPosition, nextPiece.data);
            Set(activePiece);
            
            SetNextPiece();
        } else {
            Clear(savedPiece);
            savedPiece.Initialize(this, holdPosition, activePiece.data);
            Set(savedPiece);

            Clear(activePiece);
            activePiece.Initialize(this, spawnPosition, savedData);
            Set(activePiece);
        }
    }

    private void GameOver() {
        tilemap.ClearAllTiles();
        linesCleared = 0;
    }

    public void Set(Piece piece) {
        for(int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece) {
        for(int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position) {
        RectInt bounds = Bounds;

        for(int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            if(tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines() {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax) {
            if(IsLineFull(row)) {
                LineClear(row);
                linesCleared++;
            } else {
                row++;
            }
        }
    }

    public void SetLevel() {
        if(linesCleared == 4 & !levels[0]) {
            levels[0] = true;
            activePiece.stepDelay -= 0.3f;
        } else if(linesCleared == 16 & !levels[1]) {
            levels[1] = true;
            activePiece.stepDelay -= 0.3f;
        } else if(linesCleared == 64 & !levels[2]) {
            levels[2] = true;
            activePiece.stepDelay -= 0.3f;
            activePiece.quickDropDelay -= 0.01f;
        }
    }

    private bool IsLineFull(int row) {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++) {
            Vector3Int position = new Vector3Int(col, row, 0);

            if(!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row) {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++) {
            Vector3Int position = new Vector3Int(col, row, 0);

            tilemap.SetTile(position, null);
        }

        while(row < bounds.yMax) {
            for(int col = bounds.xMin; col <= bounds.xMax; col++) {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}