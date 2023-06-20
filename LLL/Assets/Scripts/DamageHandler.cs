using UnityEngine;
using UnityEngine.UI;

public class DamageHandler : MonoBehaviour
{
    public float health = 100.0f;
    public Text healthText;
    public Transform[] respawnPoints;

    private void Start()
    {
        UpdateHealthText();
    }

    public void HandleHit(float hitForce)
    {
        float damage = hitForce * 0.1f; // Можете настроить множитель урона по вашему усмотрению
        health -= damage;
        UpdateHealthText();

        if (health <= 0)
        {
            Respawn();
        }
    }

    private void UpdateHealthText()
    {
        if (healthText)
        {
            healthText.text = $"Health: {health}";
        }
    }

    private void Respawn()
    {
        health = 100.0f;
        UpdateHealthText();
        Transform respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
    }
}
