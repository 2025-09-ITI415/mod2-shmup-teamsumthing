
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For heart UI images

[RequireComponent(typeof(BoundsCheck))]
public class Main : MonoBehaviour
{
    static private Main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2.0f;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.blaster,
        eWeaponType.spread, eWeaponType.shield
    };

    [Header("Player / UI")]
    [Tooltip("Drag the Hero prefab here (NOT a scene instance)")]
    public GameObject heroPrefab;
    [Tooltip("Optional: where the hero should spawn. If left empty, (0,0,0) will be used")]
    public Transform heroSpawnPoint;

    [Tooltip("Assign the 3 heart Image UI elements (1 = left/top-most)")]
    public Image heart1;
    public Image heart2;
    public Image heart3;

    [Header("Lives")]
    public int heroLives = 3;        // initial lives

    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        // spawn enemies
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

        // build dict
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }

        // sanity: ensure UI hearts reflect starting lives
        UpdateHeartsUI();
    }

    public void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;
        BoundsCheck bc = go.GetComponent<BoundsCheck>();
        if (bc != null) enemyInset = Mathf.Abs(bc.radius);

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene("__Scene_0");
    }

    // Called externally when the hero dies (for example: Hero.cs calls Main.HERO_DIED())
    static public void HERO_DIED()
    {
        if (S == null)
        {
            Debug.LogError("Main singleton S is null when HERO_DIED called.");
            return;
        }
        S.HandleHeroDeath();
    }

    // Non-static handler (updates lives, UI, respawn or restart)
    private void HandleHeroDeath()
    {
        heroLives = Mathf.Max(0, heroLives - 1);
        Debug.Log("Main: HERO_DIED called. Lives remaining: " + heroLives);

        UpdateHeartsUI();

        if (heroLives > 0)
        {
            // Respawn after a short delay so player can see death
            StartCoroutine(RespawnCoroutine(1.0f));
        }
        else
        {
            // No lives left -> restart game
            DelayedRestart();
        }
    }

    private IEnumerator RespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // instantiate hero prefab at spawn point or origin
        if (heroPrefab == null)
        {
            Debug.LogError("Main: heroPrefab not assigned in Inspector!");
            yield break;
        }

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (heroSpawnPoint != null)
        {
            spawnPos = heroSpawnPoint.position;
            spawnRot = heroSpawnPoint.rotation;
        }

        GameObject newHero = Instantiate(heroPrefab, spawnPos, spawnRot);
        newHero.name = "Hero"; // consistent name if other code expects it

        // Optionally, you can add a small invincibility or visual effect here
    }

    private void UpdateHeartsUI()
    {
        if (heart1 != null) heart1.enabled = heroLives >= 1;
        if (heart2 != null) heart2.enabled = heroLives >= 2;
        if (heart3 != null) heart3.enabled = heroLives >= 3;
    }

    // weapon definition accessor
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
            return WEAP_DICT[wt];
        return new WeaponDefinition();
    }

    static public void SHIP_DESTROYED(Enemy e)
    {
        if (S == null) return;

        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            GameObject go = Instantiate(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            if (pUp != null) pUp.SetType(pUpType);
            go.transform.position = e.transform.position;
        }
    }
}
