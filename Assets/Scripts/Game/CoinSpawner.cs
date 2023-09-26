using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject coinPrefab;

    private void Awake()
    {
        
        NetworkManager.Singleton.OnServerStarted += SpawnCoinStart;
    }


    private void SpawnCoinStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnCoinStart;
        if (IsServer)
            StartCoroutine(SpawnOverTime());
    }

    IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return new WaitForSeconds(2f);
            SpawnCionServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnCionServerRpc()
    {
        SpawnCoin();
    }

    private void SpawnCoin()
    {
       GameObject coinInstance = Instantiate(coinPrefab, GetRandomPosOnMap(), Quaternion.identity);
       coinInstance.GetComponent<NetworkObject>().Spawn();
    }

    private Vector3 GetRandomPosOnMap()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height; 
        Vector3 spawnPosition = new Vector3(Random.Range(0, screenWidth), Random.Range(0, screenHeight), 10);
        spawnPosition = Camera.main.ScreenToWorldPoint(spawnPosition);
        return spawnPosition;
    }
}
