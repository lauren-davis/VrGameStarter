using UnityEngine;
using JetBrains.Annotations;

public class Projectile : MonoBehaviour
{
    public static Projectile Shoot(GameObject prefab, Vector3 startPos, Vector3 direction)
    {
        var projectile = Instantiate(prefab).GetComponent<Projectile>();
        projectile.Shoot(startPos, direction);
        return projectile;
    }

    public float StartSpeed = 8f;
    public float DropRate = 2f;
    public float Damage = 100f;
    public bool EmbedInTarget = true;
    public float DisappearTime = 5f;

    [Range(0f, 90f)]
    public float MaxRicochetAngle = 30f;

    private Vector3 _velocity;
    private bool _hasHit;
    private float _hitTime;

    public void Shoot(Vector3 startPos, Vector3 direction)
    {
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        _velocity = transform.forward*StartSpeed;
    }

    private Target FindTarget(Transform transform)
    {
        var target = transform.GetComponent<Target>();
        if (target != null) return target;

        if (transform.GetComponent<Rigidbody>() != null) return null;
        if (transform.parent != null) return FindTarget(transform.parent);

        return null;
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        // If the projectile has already hit something, wait until it should
        // disappear and then remove it.
        if (_hasHit)
        {
            if (Time.fixedTime >= _hitTime + DisappearTime)
            {
                Destroy(gameObject);
            }

            return;
        }

        // If the projectile has fallen below the world, destroy it.
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
            return;
        }

        // How far the projectile should travel this update
        var travel = _velocity.magnitude*Time.fixedDeltaTime;

        var curPos = transform.position;
        var ray = new Ray(curPos, _velocity);

        RaycastHit hit;
        var hasHit = Physics.Raycast(ray, out hit, travel);
        var hitAngle = 90f - Vector3.Angle(_velocity, -hit.normal);

        // If the projectile hasn't hit anything, or if it should ricochet...
        if (!hasHit || hitAngle >= 0f && hitAngle < MaxRicochetAngle)
        {
            // If it hit something, it should ricochet by reflecting the velocity
            if (hasHit)
            {
                _velocity = Vector3.Reflect(_velocity, hit.normal);
            }

            // Move by the velocity and rotate to face the direction of travel
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.rotation = Quaternion.LookRotation(_velocity, Vector3.up);

            // Simulate gravity on the projectile
            _velocity -= Vector3.up * DropRate * Time.fixedDeltaTime;
            return;
        }

        // If we're here, the projectile has hit something

        // Move the projectile to the hit position
        transform.position += _velocity.normalized * hit.distance;

        _hasHit = true;
        _hitTime = Time.fixedTime;

        // Try to find which target we hit, and tell that target
        // that it was hit
        var target = FindTarget(hit.collider.transform);
        if (target != null)
        {
            target.Hit(this);
        }

        // If this projectile shouldn't embed in the target, destroy it now
        if (!EmbedInTarget)
        {
            Destroy(gameObject);
            return;
        }

        // If we're here, we should embed in the target
        var targetTransform = target != null ? target.transform : hit.transform;
        transform.SetParent(targetTransform, true);
    }
}
