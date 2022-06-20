using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LinesDisplay : MonoBehaviour
{
    public Board board;
    public TMP_Text linesText;

    private void Update() {
        linesText.text = "LINES: " + board.linesCleared;
    }
}
