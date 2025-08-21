using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public UnityEvent<int> OnDamageTaken;
    public UnityEvent OnDeath;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        OnDamageTaken.Invoke(currentHealth);
        
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        OnDeath.Invoke();
        
        Destroy(gameObject);
    }
}