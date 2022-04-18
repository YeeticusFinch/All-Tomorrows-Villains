using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    private void Start()
    {

        GetComponent<Player>().Setup();

        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }


    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager.RegisterPlayer(GetComponent<NetworkIdentity>().netId.ToString(), GetComponent<Player>());
    }

    void AssignRemoteLayer()
    {
        //gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        gameObject.layer = 9;
        if (GetComponent<Player>().model == null)
        {
            Debug.Log("1");
            GetComponent<Player>().CmdLoadModel();
        }
        //GetComponent<Player>().model.layer = 9;
    }

    private void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnregisterPlayer(transform.name);
    }
}
