using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] SpriteRenderer coloredPart;

    public NetworkVariable<ushort> health = new (10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ushort> score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> color = new(new Color(1, 0, 0.913f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> playerIndex = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [CanBeNull] public static event System.Action<ushort> ChangedScoreEvent;
    [CanBeNull] public static event System.Action<ushort> ChangedHealthEvent;


    public override void OnNetworkSpawn()
    {
        GameManager.AddPlayer();
        if (!IsOwner)
            Destroy(gameObject.GetComponent<PlayerController>());
        if (!IsServer)
        {
            health.OnValueChanged += HealthChanged;
            score.OnValueChanged += ScoreChanged;
        }
        if (NetworkManager.Singleton.IsServer)
        {
            color.Value = GameManager.GetColor();
            playerIndex.Value = GameManager.GetIndex();
        }
        coloredPart.color = color.Value;
        
    }
    public void DecreseHealth(ushort amount)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            health.Value -= amount;
        }
        if (health.Value <= 0)
        {
            GameManager.KillPlayer();
            NetworkObject.Despawn();
        }
        InvokeChangeHealth();
    }

    public void IncreseScore(ushort amount)
    {
        score.Value += amount;
        InvokeChangeScore();
    }

    private void InvokeChangeScore()
    {
        if (!IsOwner) return;
        ChangedScoreEvent?.Invoke(score.Value);
    }
    private void InvokeChangeHealth()
    {
        if (!IsOwner) return;
        ChangedHealthEvent?.Invoke(health.Value);
    }


    private void HealthChanged(ushort prevValue, ushort newValue)
    {
        InvokeChangeHealth();
    }

    public void ScoreChanged(ushort prevValue, ushort newValue)
    {
        InvokeChangeScore();
    }


}
