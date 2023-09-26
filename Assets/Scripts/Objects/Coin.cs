using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!NetworkManager.Singleton.IsServer) return;


        if (collision.TryGetComponent(out PlayerNetwork playerNetwork))
        {
            playerNetwork.IncreseScore(1);
        }

        NetworkObject.Despawn();
    }
}
