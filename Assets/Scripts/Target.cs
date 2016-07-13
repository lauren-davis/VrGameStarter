using UnityEngine;

public class Target : MonoBehaviour
{
    public float Health = 100f;
    public bool CanDamage = true;

    public void Hit(Projectile projectile)
    {
        if (!CanDamage) return;

        Health -= projectile.Damage;

        if (Health <= 0f)
        {
            OnKill();
        }
    }

    private void OnKill()
    {
        Destroy(gameObject);
    }
}
