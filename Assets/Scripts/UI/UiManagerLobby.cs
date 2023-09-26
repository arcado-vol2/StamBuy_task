using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerLobby : MonoBehaviour
{
    [SerializeField] private GameObject page1;

    [SerializeField] private GameObject page2;
    [SerializeField] private TMP_InputField lobbyNameField;
    [SerializeField] private GameObject lobbyNotFoundLabel;

    [SerializeField] private GameObject page3;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerCountText;



    private void Start()
    { 
        lobbyNotFoundLabel.SetActive(false);
        LobbyManager.OnJoinedLobby += UpdateLobby_Event;
        ActivatePage1();
    }

 
    
public void ActivatePage1()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
    }

    public void ActivatePage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        page3.SetActive(false);
    }

    public void ActivatePage3()
    {
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(true);
    }

    public async void OnCreateLobby()
    {
        await LobbyManager.CreateLobby(lobbyNameField.text);
        JoinCurrentLobby();
    }

    public async void OnGetLobby()
    {

        string lobbyId = await LobbyManager.GetLobbyIdByItName(lobbyNameField.text);
        if (lobbyId == null)
        {
            StartCoroutine(showLobbyNotFoundText());
        }
        else
        {
            await LobbyManager.JoinLobbyViaId(lobbyId);
            JoinCurrentLobby();
        }
    }

    public void JoinCurrentLobby()
    {
        ActivatePage3();
        lobbyName.text = LobbyManager.currentLobby.Name;
    }
    IEnumerator showLobbyNotFoundText()
    {
        lobbyNotFoundLabel.SetActive(true);
        yield return new WaitForSeconds(1f);
        lobbyNotFoundLabel.SetActive(false);
    }

    public async void ExitLobby()
    {
        if (LobbyManager.selfType == clientType.client)
            await LobbyManager.LeaveCurrentLobby();
        else
            await LobbyManager.LeaveCurrentLobby();
        ActivatePage2();
    }

    public async void SignIn()
    {
        await LobbyManager.SignIn();
        ActivatePage2();
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void UpdateLobby()
    {
        playerCountText.text = LobbyManager.currentLobby.Players.Count + "/" + LobbyManager.currentLobby.MaxPlayers;
    }

    private void UpdateLobby_Event(object sender, int e)
    {
        UpdateLobby();
    }
}
