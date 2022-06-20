using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board {get; private set;}
    public Vector3Int position {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public int rotationIndex {get; private set;}

    public float stepDelay = 1f;
    public float lockDelay = 0.3f;

    public float quickDropDelay = 0.05f;
    public float holdDelay = 0.08f;
    
    private float quickDropTimer = 0f;
    private float holdTimer = 0f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;

        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        if(cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update() {
        board.Clear(this);
        board.SetLevel();

        lockTime += Time.deltaTime;

        CheckUserInput();

        if(Time.time >= stepTime) {
           Step();
        }

        board.Set(this);
    }

    private void CheckUserInput() {
        if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Z)) {
            Rotate(-1);
        }

        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.UpArrow)) {
            Rotate(1);
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            if(holdTimer < holdDelay) {
                holdTimer += Time.deltaTime;
                return;
            }

            holdTimer = 0f;
            
            Move(Vector2Int.left);
        }
        
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            if(holdTimer < holdDelay) {
                holdTimer += Time.deltaTime;
                return;
            }

            holdTimer = 0f;

            Move(Vector2Int.right);
        }
        
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            if(quickDropTimer < quickDropDelay) {
                quickDropTimer += Time.deltaTime;
                return;
            }

            quickDropTimer = 0f;
            
            Move(Vector2Int.down);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C)) {
            board.SwapPiece();
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            board.Restart();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            while(Move(Vector2Int.down)) {
                continue;
            }

            Lock();
        }
    }

    private void Step() {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if(lockTime >= lockDelay) {
            Lock();
        }
    }

    private void Lock() {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    private void Rotate(int direction) {
        int original = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 3);

        ApplyRotMatrix(direction);

        if(!TestWallKicks(rotationIndex, direction)) {
            rotationIndex = original;
            ApplyRotMatrix(-direction);
        }
    }

    private void ApplyRotMatrix(int direction) {
        for(int i = 0; i < cells.Length; i++) {
            Vector3 cell = cells[i];

            int x, y;

            switch(data.tetromino) {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;

                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection) {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++) {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection) {
        int wallKickIndex = 2 * rotationIndex;

        if(rotationIndex < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max) {
        if(input < min) {
            return max - (min - input) % (max - min);
        } else if(input >= max) {
            return min + (input - max) % (max - min);
        } else {
            return input;
        }
    }

    private bool Move(Vector2Int translation) {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if(valid) {
            position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }
} 