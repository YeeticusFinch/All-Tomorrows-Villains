
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public MatchSettings matchSettings;
    public SoundLib sound = new SoundLib();

    private void Start()
    {
        ReadFile();
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
