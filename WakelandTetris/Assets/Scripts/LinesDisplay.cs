using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LinesDisplay : MonoBehaviour
{
    public TMP_Text linesText;

    private void Update() {
        linesText.text = "LINES: " + Board.linesCleared;
    }
}
