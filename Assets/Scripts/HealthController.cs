using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    float maxHealth = 100.0F;

    [SerializeField]
    Slider healthBar;

    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameObject.CompareTag("Player"))
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void Damage(float value)
    {
        currentHealth -= value;
        
        if (gameObject.CompareTag("Player"))
        {
            if (healthBar != null)
            {
                healthBar.value =
                   currentHealth > healthBar.maxValue
                            ? healthBar.maxValue
                            : currentHealth < healthBar.minValue
                                 ? healthBar.minValue
                                 : currentHealth;
            }
        }

        if (currentHealth <= 0.0)
        {
            currentHealth = 0.0F;
            if (gameObject.CompareTag("Player"))
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentScene);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
}
