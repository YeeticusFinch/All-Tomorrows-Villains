
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
    public static Player localPlayer;
    public bool thirdPerson = false;
    public bool freeCam = false;

    public bool isServer = false;

    private void Start()
    {
        //ReadFile();
        importShit();
        sound.init();
        readInputs();

        if (sceneCamera != null)
        {
            playables = sceneCamera.GetComponent<SceneCameraRotate>().playables;
            charId = sceneCamera.GetComponent<SceneCameraRotate>().charId;
        }
    }

    private Dictionary<string, string> controls = new Dictionary<string, string>();

    public string getControlMapping(string ctrl)
    {
        if (controls.ContainsKey(ctrl))
            return controls[ctrl];
        return "Missing keybind you fucking idiot";
    }

    public void readInputs()
    {
        /*
        var inputManager = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(inputManager);
        UnityEditor.SerializedProperty axisArray = obj.FindProperty("m_Axes");
        if (axisArray.arraySize == 0)
            Debug.Log("No Axes, is Input Manager empty?");

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            var axis = axisArray.GetArrayElementAtIndex(i);
            var name = axis.displayName;  //axis.displayName  "Horizontal"	string
            axis.Next(true);        //axis.displayName	    "Name"	string
            axis.Next(false);   //axis.displayName	"Descriptive Name"	string
            axis.Next(false);   //axis.displayName	"Descriptive Negative Name"	string
            axis.Next(false);   //axis.displayName	"Negative Button"	string
            axis.Next(false);   //axis.displayName	"Positive Button"	string
            var value = axis.stringValue;  //"right"

            //Debug.Log(name + " | " + value);
            if (controls.ContainsKey(name.ToString()) && (controls[name.ToString()] == null || controls[name.ToString()].Length < 1))
                controls[name.ToString()] = value.ToString();
            else if (!controls.ContainsKey(name.ToString()))
                controls.Add(name.ToString(), value.ToString());
        }*/
    }

    public void importShit()
    {
        if (sceneCamera)
        {
            matchSettings.respawnTime = sceneCamera.GetComponent<SceneCameraRotate>().shit[1];
            matchSettings.speedMult = 6 / sceneCamera.GetComponent<SceneCameraRotate>().shit[2];
            matchSettings.moveSpeedMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[3];
            matchSettings.scaleMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[4];
            matchSettings.damageMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[5];
            sound.volume = sceneCamera.GetComponent<SceneCameraRotate>().shit[6];
            sound.pitch = sceneCamera.GetComponent<SceneCameraRotate>().shit[7];
            sound.distMult = sceneCamera.GetComponent<SceneCameraRotate>().shit[8]; playables = sceneCamera.GetComponent<SceneCameraRotate>().playables;
            charId = sceneCamera.GetComponent<SceneCameraRotate>().charId;
            Debug.Log("Successfully completed important shit!");
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        } else {
            instance = this;
        }
        if (sceneCamera != null && sceneCamera.GetComponent<SceneCameraRotate>() != null)
            charId = sceneCamera.GetComponent<SceneCameraRotate>().charId;
    }

    public void otherHitStuff(RaycastHit hit, float dmg, string dmgType, int num, string save, GameObject sender)
    {
        if(hit.collider.gameObject.tag == "Lever")
        {
            ShipFloor floor = ShipFloor.floorMap[ShipFloor.leverMap[hit.collider.gameObject]].GetComponent<ShipFloor>();
            if (floor != null)
            {
                floor.toggleSteps();
            }
        } else if (hit.collider.gameObject.tag == "Elevator")
        {
            sender.GetComponent<Player>().chara.creature.elevatorSelect(hit.collider.gameObject.GetComponent<Elevator>().floorNum);
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

    public static void SetLocalPlayer(Player player)
    {
        localPlayer = player;
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
        if (isServer)
        {
            foreach (string playerId in players.Keys)
            {
                GUILayout.Label((players[playerId] != null && players[playerId].chara != null ? players[playerId].chara.title : "Unknown") + " " + players[playerId].transform.name/* + " : " + players[playerId].health + " / " + players[playerId].maxHealth*/);
            }
        }
        if (localPlayer != null)
            GUILayout.Label(Mathf.Round(localPlayer.GetSpeed()) + " feet / turn");

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
