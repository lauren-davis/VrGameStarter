using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public Wand Wand;
    public GameObject ProjectilePrefab;
    public AudioClip Shootsound;
    void Update()
    {
        if (Wand.TriggerButton.JustPressed)
        {
            Projectile.Shoot(ProjectilePrefab, transform.position, transform.forward);
            var audiosource=GetComponent<AudioSource>();
            audiosource.PlayOneShot(Shootsound);
        }
    }
}
