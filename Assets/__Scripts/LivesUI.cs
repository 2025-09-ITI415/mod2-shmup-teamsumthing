using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Image[] hearts;
    public int lives = 3;

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            UpdateHearts();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < lives;
        }
    }
}
