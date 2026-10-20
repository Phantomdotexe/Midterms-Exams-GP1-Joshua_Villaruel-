
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject spawnPrefab;
    public Camera gameCamera;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text healthText;
    public TMP_Text timerText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public Button spawnButton;

    [Header("Health Regeneration")]
    public float minRegenPerSecond = 0.6f;
    public float maxRegenPerSecond = 1.5f;
    public float maxHealth = 100f;

    private int score = 0;
    private float health = 100f;
    private float timeRemaining = 30f;
    private bool gameEnded = false;

    private float totalRegenPerSecond = 0f;

    private void Start()
    {
        score = 0;
        health = 100f;
        timeRemaining = 30f;
        gameEnded = false;
        totalRegenPerSecond = 0f;

        gameOverPanel.SetActive(false);
        spawnButton.interactable = true;

        UpdateUI();
        StartCoroutine(Countdown());
        StartCoroutine(HealthRegeneration());
    }

    public void SpawnObject()
    {
        if (gameEnded)
            return;

        score += 1;
        health = Mathf.Max(0f, health - 10f);

        if (health <= 0f)
        {
            UpdateUI();
            EndGame();
            return;
        }

        float depth = Mathf.Abs(gameCamera.transform.position.z);

        Vector3 spawnPosition = gameCamera.ViewportToWorldPoint(
            new Vector3(
                Random.Range(0.1f, 0.9f),
                Random.Range(0.2f, 0.8f),
                depth
            )
        );

        Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

        float cubeRegen = Random.Range(
            minRegenPerSecond,
            maxRegenPerSecond
        );

        totalRegenPerSecond += cubeRegen;

        UpdateUI();
    }

    private IEnumerator HealthRegeneration()
    {
        while (!gameEnded)
        {
            yield return new WaitForSeconds(1f);

            if (gameEnded)
                yield break;

            health = Mathf.Min(
                maxHealth,
                health + totalRegenPerSecond
            );

            UpdateUI();
        }
    }

    private IEnumerator Countdown()
    {
        while (timeRemaining > 0f && !gameEnded)
        {
            yield return new WaitForSeconds(1f);

            if (gameEnded)
                yield break;

            timeRemaining = Mathf.Max(
                0f,
                timeRemaining - 1f
            );

            UpdateUI();

            if (timeRemaining <= 0f)
                EndGame();
        }
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        healthText.text =
            "Health: " + Mathf.CeilToInt(health)
            + " / " + Mathf.CeilToInt(maxHealth);

        timerText.text =
            "Time: " + Mathf.CeilToInt(timeRemaining);
    }

    private void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        spawnButton.interactable = false;

        finalScoreText.text =
            "Game Over!\nFinal Score: " + score;

        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
