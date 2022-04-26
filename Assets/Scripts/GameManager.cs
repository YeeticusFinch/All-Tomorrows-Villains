﻿
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public MatchSettings matchSettings;
    public SoundLib sound = new SoundLib();
    public int charId = 1;
    public GameObject[] playables;
    public GameObject sceneCamera;

    private void Start()
    {
        playables = sceneCamera.GetComponent<SceneCameraRotate>().playables;
        charId = sceneCamera.GetComponent<SceneCameraRotate>().charId;
        //ReadFile();
        matchSettings.respawnTime = sceneCamera.GetComponent<SceneCameraRotate>().shit[1];
        matchSettings.speedMult = 6/sceneCamera.GetComponent<SceneCameraRotate>().shit[2];
        matchSettings.moveSpeedMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[3];
        matchSettings.scaleMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[4];
        matchSettings.damageMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[5];
        sound.volume = sceneCamera.GetComponent<SceneCameraRotate>().shit[6];
        sound.pitch = sceneCamera.GetComponent<SceneCameraRotate>().shit[7];
        sound.distMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[8];
        sound.init();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        } else {
            instance = this;
        }
        charId = sceneCamera.GetComponent<SceneCameraRotate>().charId;
    }

    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string netId, Player player)
    {
        string playerId = PLAYER_ID_PREFIX + netId;
        players.Add(playerId, player);
        player.transform.name = playerId;
    }

    public static void UnregisterPlayer(string playerId)
    {
        players.Remove(playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string playerId in players.Keys)
        {
            GUILayout.Label((players[playerId] != null && players[playerId].chara != null ? players[playerId].chara.title : "Unknown") + " " + players[playerId].transform.name + " : " + players[playerId].health + " / " + players[playerId].maxHealth);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    #endregion

    #region properties

    public void ReadFile()
    {
        //string path = "Assets/Resources/maps/";
        StreamReader reader = new StreamReader("properties.txt");
        
        string[] input = reader.ReadToEnd().Split('\n');
        foreach (string s in input) {
            if (s.IndexOf("Respawn Time = ") != -1)
            {
                if (float.TryParse(s.Substring(15), out matchSettings.respawnTime))
                    Debug.Log("Imported Respawn Time from properties file");
                else
                    Debug.Log("Failed to parse Respawn Time from properties file");
            }
            else if (s.IndexOf("Turn Time = ") != -1)
            {
                float x;
                if (float.TryParse(s.Substring(12), out x))
                {
                    matchSettings.speedMult = 6 / x;
                    Debug.Log("Imported Turn Time from properties file");
                }
                else
                    Debug.Log("Failed to parse Turn Time from properties file");
            }
            else if (s.IndexOf("Move Speed = ") != -1)
            {
                if (float.TryParse(s.Substring(13), out matchSettings.moveSpeedMult))
                    Debug.Log("Imported Move Speed from properties file");
                else
                    Debug.Log("Failed to parse Move Speed from properties file");
            }
            else if (s.IndexOf("Scale = ") != -1)
            {
                if (float.TryParse(s.Substring(8), out matchSettings.scaleMult))
                    Debug.Log("Imported Scale from properties file");
                else
                    Debug.Log("Failed to parse Scale from properties file");
            }
            else if (s.IndexOf("Damage = ") != -1)
            {
                if (float.TryParse(s.Substring(9), out matchSettings.damageMult))
                    Debug.Log("Imported Damage from properties file");
                else
                    Debug.Log("Failed to parse Damage from properties file");
            }
            else if (s.IndexOf("Character = ") != -1)
            {
                /*if (int.TryParse(s.Substring(12), out charId))
                    Debug.Log("Imported Character from properties file");
                else
                    Debug.Log("Failed to parse Character from properties file");*/
            }
            else if (s.IndexOf("Volume = ") != -1)
            {
                if (float.TryParse(s.Substring(9), out sound.volume))
                    Debug.Log("Imported Volume from properties file");
                else
                    Debug.Log("Failed to parse Volume from properties file");
            }
            else if (s.IndexOf("Pitch = ") != -1)
            {
                if (float.TryParse(s.Substring(8), out sound.pitch))
                    Debug.Log("Imported Pitch from properties file");
                else
                    Debug.Log("Failed to parse Pitch from properties file");
            }
            else if (s.IndexOf("Attenuation = ") != -1)
            {
                if (float.TryParse(s.Substring(14), out sound.distMult))
                    Debug.Log("Imported Attenuation from properties file");
                else
                    Debug.Log("Failed to parse Attenuation from properties file");
            }
        }
    }

    #endregion
}
