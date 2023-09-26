using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private Transform shootPoint;
    private float lastFired = float.MinValue;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        bool input = playerInput.actions["shoot"].IsPressed();
        if (input && lastFired + cooldown < Time.time)
        {
            lastFired = Time.time;
            
            RequestFireServerRpc();

            ExecuteShoot();
        }
    }

    [ServerRpc]
    private void RequestFireServerRpc()
    {
        FireClientRpc();
    }

    [ClientRpc]
    private void FireClientRpc()
    {
        if (!IsOwner) ExecuteShoot();
    }

    private void ExecuteShoot()
    {
        Instantiate(projectilePrefab, shootPoint.position,transform.rotation);
    }

}
