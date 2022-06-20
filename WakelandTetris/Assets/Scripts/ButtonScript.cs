using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void PlayAgain() {
        SceneManager.LoadScene("Tetris");
    }

    public void Quit() {
        Application.Quit();
    }
}
