using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void PlayAgain() {
        SceneManager.LoadScene("Tetris");
        Board.linesCleared = 0;
    }

    public void Quit() {
        Application.Quit();
    }

    public void EndGame() {
        SceneManager.LoadScene("EndScreen");
    }
}
