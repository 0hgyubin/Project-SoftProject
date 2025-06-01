
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BossDragonController : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHealth = 100;
    private float currentHealth;

    public Sprite idleSprite;
    public Sprite howlSprite;
    public Sprite castSprite;
    public Sprite fireSprite;

    public GameObject monsterPrefab;
    public GameObject firePrefab;
    public GameObject rockPrefab;
    public GameObject warningIndicatorPrefab;

    public float howlDuration = 3f;
    public float castDelay = 2f;
    public float fireCount = 5;
    public float fireInterval = 0.5f;
    public float warningDuration = 1.5f;
    public int rockDropCount = 20;
    public float rockDropInterval = 0.5f;

    public Image healthBarImage;

    private SpriteRenderer sr;

    private void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(BossRoutine());

        UpdateHealthUI();
    }

    private IEnumerator BossRoutine()
    {
        while (true)
        {
            // Idle
            sr.sprite = idleSprite;
            yield return new WaitForSeconds(2f);

            // Pattern 1: Howl and spawn 5 monsters
            sr.sprite = howlSprite;
            CameraShake.Instance.ShakeCamera(howlDuration);
            yield return new WaitForSeconds(howlDuration);

            for (int i = 0; i < 5; i++)
            {
                float x = Random.Range(-18f, 3f);
                Vector3 spawnPos = new Vector3(x, 10f, 0f);
                Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(2f);

            // Pattern 2: Rock Drop (Y = 5)
            sr.sprite = castSprite;
            yield return new WaitForSeconds(castDelay);
            List<float> usedX = new List<float>();
            for (int i = 0; i < rockDropCount; i++)
            {
                float x;
                int attempts = 0;
                do
                {
                    x = Mathf.Round(Random.Range(-18f, 3f));
                    attempts++;
                    if (attempts > 100) break;
                } while (usedX.Exists(prev => Mathf.Abs(prev - x) < 1f));

                usedX.Add(x);
                Vector3 dropPosition = new Vector3(x, 5f, 0); // Y 위치를 5로 변경
                GameObject warning = Instantiate(warningIndicatorPrefab, new Vector3(x, -3.5f, 0), Quaternion.identity);
                StartCoroutine(FadeAndDrop(warning, dropPosition));
                yield return new WaitForSeconds(rockDropInterval);
            }

            yield return new WaitForSeconds(2f);

            // Pattern 3: Fire Breath after 2s delay
            sr.sprite = fireSprite;
            yield return new WaitForSeconds(2f); // 2초 대기 후 불 내뿜기 시작
            for (int i = 0; i < fireCount; i++)
            {
                Instantiate(firePrefab, transform.position + Vector3.down * 0.5f, Quaternion.identity);
                yield return new WaitForSeconds(fireInterval);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator FadeAndDrop(GameObject warning, Vector3 dropPosition)
    {
        SpriteRenderer wsr = warning.GetComponent<SpriteRenderer>();
        Color startColor = wsr.color;
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            wsr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, elapsed / warningDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(warning);
        Instantiate(rockPrefab, dropPosition, Quaternion.identity);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= (int)damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }

    private void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
