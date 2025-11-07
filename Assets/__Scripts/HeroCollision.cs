using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroCollision : MonoBehaviour
{
    public int Lives = 3;
    public Image[] LifeIcons;
    public float InvincibilityTime = 1f;
    private bool isInvincible = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isInvincible) return;

        if (other.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    void LoseLife()
    {
        if (Lives <= 0) return;

        Lives--;

        Debug.Log("HERO_DIED called. Lives remaining: " + Lives);

        if (Lives >= 0 && Lives < LifeIcons.Length)
        {
            LifeIcons[Lives].gameObject.SetActive(false);
        }

        if (Lives <= 0)
        {
            Debug.Log("Game Over!");
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }
}
