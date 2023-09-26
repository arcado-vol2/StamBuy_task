using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private static int playersCount = 0;
    static List<Color> colors = new List<Color> { Color.yellow, Color.red, Color.green, Color.blue, Color.magenta, Color.white, Color.black };
    static List<String> names = new List<String> { "Anakin", "Obi-Wan", "Yoda", "Plo Kun", "Vindow", "Oppo Rancisis", "Even Piell" };

    private static GameManager instance;
    public static void AddPlayer()
    {
        playersCount++;
    }

    public static Color GetColor()
    {
        return colors[playersCount - 1];
    }

    public static int GetIndex()
    {
        return playersCount - 1;
    }

    public static void KillPlayer()
    {
        playersCount--;
        if (playersCount == 1)
        {
            PlayerNetwork player = FindObjectOfType<PlayerNetwork>();
            FindObjectOfType<UiManagerGame>().ShowEndPopUp(names[player.playerIndex.Value], player.score.Value);
        }
    }
}
