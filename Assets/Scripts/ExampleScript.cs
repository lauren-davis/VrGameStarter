using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public Wand Wand;
    public GameObject ProjectilePrefab;

    void Update()
    {
        if (Wand.TriggerButton.JustPressed)
        {
            Projectile.Shoot(ProjectilePrefab, transform.position, transform.forward);
        }
    }
}
