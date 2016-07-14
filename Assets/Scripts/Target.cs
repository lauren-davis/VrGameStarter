using UnityEngine;

public class Target : MonoBehaviour

{
    public int points = 1;
    public float reappearTime = 5;
    public float Health = 100f;
    public bool CanDamage = true;
    public AudioClip hitsound;
    public AudioSource audiosource;
    private bool isvisible = true;
    public void Hit(Projectile projectile)
    {
        if (!isvisible) return;
        audiosource.PlayOneShot(hitsound);
        if (!CanDamage) return;
        var scoreboard = FindObjectOfType<Scoreboard>();
        scoreboard.addpoints(points);

        Health -= projectile.Damage;

        if (Health <= 0f)
        {
            OnKill();
        }
    }

    private void OnRespawn()
    {
        var renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        isvisible = true;
    }
    private void OnKill()
    {
        var renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        Invoke("OnRespawn",reappearTime);
        isvisible = false;
    }
}
