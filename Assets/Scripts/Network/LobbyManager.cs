using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum clientType
{
    none,
    host,
    client,
    server
}
public class LobbyManager : NetworkBehaviour
{
    public static Lobby currentLobby; 
    public static clientType selfType = clientType.none;
    float heartbitTimer =0;
    static float lobbyPollTimer;

    private static LobbyManager instance;

    public static event EventHandler<int> OnJoinedLobby;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        if (selfType == clientType.host ||  selfType == clientType.server)
        {
            LobbyHeartbeat();

        }
        LobbyPolling();


    }

    private static async void LobbyPolling()
    {
        if (currentLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                OnJoinedLobby?.Invoke(instance, 0);

      

            }
        }
    }

    public static async Task<int> SignIn()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                //for editor tests
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(UnityEngine.Random.Range(0, 999).ToString());

                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return 1;
        }
        return 0;
    }

    public static async Task<int> CreateLobby(string lobbyName)
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 7);
            selfType = clientType.host;
            OnJoinedLobby?.Invoke(instance, 0);
        }
        catch (LobbyServiceException ex)
        {
            
            Debug.LogError(ex.Message);
            return 1;
        }
        return 0;
    }

    public static async Task<string> GetLobbyIdByItName(string lobbyName)
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        foreach (var response in queryResponse.Results)
        {
            if (response.Name == lobbyName);
            {
                return response.Id;
            }
        }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
        }
        return null;
    }

    public static async Task<int> JoinLobbyViaId(string lobbyId)
    {
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            selfType = clientType.client;
            OnJoinedLobby?.Invoke(instance, 0);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return 1;
        }
        return 0;
    }

    public static async Task<int> LeaveCurrentLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            currentLobby = null;
            
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return 1;
        }
        return 0;
    }

    public async static Task<int> EnterTheGame()
    {
        try
        {
            switch (selfType)
            {
                case clientType.client:
                    NetworkManager.Singleton.StartClient();
                    currentLobby = null;
                    break;
                case clientType.host:
                    NetworkManager.Singleton.StartHost();
                    currentLobby = null;
                    break;
                case clientType.server:
                    NetworkManager.Singleton.StartServer();
                    currentLobby = null;
                    break;
                default:
                    return 1;
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return 1;
        }

        return 0;
    }

    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            await EnterTheGame();
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    private async void LobbyHeartbeat()
    {
        if (currentLobby != null)
        {
            heartbitTimer -= Time.deltaTime; ;
            if (heartbitTimer < 0)
            {
                heartbitTimer = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }

    public static async Task<int> DeleteCurrentLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            currentLobby = null;
            OnJoinedLobby?.Invoke(instance, 1);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return 1;
        }
        return 0;
        
    }

}
