using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour {
    public int health = 100;

	public void DealDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
