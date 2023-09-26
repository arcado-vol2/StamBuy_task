using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 50.0f;

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        Vector3 worldPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (worldPosition.x < 0f || worldPosition.x > 1f || worldPosition.y < 0f || worldPosition.y > 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent(out PlayerNetwork playerNetwork))
        {
            playerNetwork.DecreseHealth(1);
        }

        Destroy(gameObject);
    }
}
