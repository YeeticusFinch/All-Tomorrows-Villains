using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Effects : NetworkBehaviour {

    public static Effects instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Effects in scene.");
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    private Material glow;

    [Command]
    public void CmdSparkyColor(Vector3 pos1, Vector3 pos2, string sound, string hitSound, Color color)
    {
        RpcSparkyColor(pos1, pos2, sound, hitSound, color);
    }

    [Command]
    public void CmdSparky(Vector3 pos1, Vector3 pos2, string sound, string hitSound)
    {
        RpcSparky(pos1, pos2, sound, hitSound);
    }

    [ClientRpc]
    public void RpcSparkyColor(Vector3 pos1, Vector3 pos2, string sound, string hitSound, Color color)
    {
        //shoot.Play();
        if (sound != null && sound.Length > 0)
            GameManager.instance.sound.playAt(sound, pos1, 0.5f, 0.1f * Random.Range(7, 13), 100f);
        if (hitSound != null && hitSound.Length > 0)
            GameManager.instance.sound.playAt(hitSound, pos2, 1f, 0.1f * Random.Range(5, 15), 150f);
        StartCoroutine(SparkyIE(pos1, pos2, color));
    }

    [ClientRpc]
    public void RpcSparky(Vector3 pos1, Vector3 pos2, string sound, string hitSound)
    {
        //shoot.Play();
        if (sound != null && sound.Length > 0)
            GameManager.instance.sound.playAt(sound, pos1, 0.5f, 0.1f * Random.Range(7, 13), 100f);
        if (hitSound != null && hitSound.Length > 0)
            GameManager.instance.sound.playAt(hitSound, pos2, 1f, 0.1f * Random.Range(5, 15), 150f);
        StartCoroutine(SparkyIE(pos1, pos2, new Color(0.069420f, 0, 0)));
    }

    public void Sparky(Vector3 pos1, Vector3 pos2, string sound, string hitSound, Color color, float soundVolume = 0.5f, float hitVolume = 1)
    {
        //CmdSparky(pos1, pos2, sound, hitSound);
        if (sound != null && sound.Length > 0)
            GameManager.instance.sound.playAt(sound, pos1, soundVolume, 0.1f * Random.Range(7, 13), 100f);
        if (hitSound != null && hitSound.Length > 0)
            GameManager.instance.sound.playAt(hitSound, pos2, hitVolume, 0.1f * Random.Range(5, 15), 150f);
        StartCoroutine(SparkyIE(pos1, pos2, color));
    }

    public void Sparky(Vector3 pos1, Vector3 pos2, string sound, string hitSound, float soundVolume = 0.5f, float hitVolume = 1)
    {
        //CmdSparky(pos1, pos2, sound, hitSound);
        if (sound != null && sound.Length > 0)
            GameManager.instance.sound.playAt(sound, pos1, soundVolume, 0.1f * Random.Range(7, 13), 100f);
        if (hitSound != null && hitSound.Length > 0)
            GameManager.instance.sound.playAt(hitSound, pos2, hitVolume, 0.1f * Random.Range(5, 15), 150f);
        StartCoroutine(SparkyIE(pos1, pos2, new Color(0.069420f, 0, 0)));
    }

    IEnumerator SparkyIE(Vector3 pos1, Vector3 pos2, Color color, int iterations = 2)
    {
        int c = Random.Range(5, Mathf.Max(7, (int)(2 * Mathf.Abs((pos2 - pos1).magnitude))));

        for (int j = 0; j < iterations; j++) {

            LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
            lineRenderer.material = glow;
            if (Mathf.Abs(color.r - 0.069420f) < 0.0001f && Mathf.Abs(color.g) + Mathf.Abs(color.b) < 0.001)
            {
                lineRenderer.material.color = new Color(Random.Range(0, 100) / 7f, Random.Range(0, 100) / 7f, Random.Range(0, 100) / 7f);

                switch (Random.Range(0, 8))
                {
                    case 0:
                    case 1:
                    case 2:
                        lineRenderer.startColor = Color.magenta;
                        break;
                    case 3:
                        lineRenderer.startColor = Color.yellow;
                        break;
                    case 4:
                    case 5:
                        lineRenderer.startColor = Color.cyan;
                        break;
                    case 6:
                    case 7:
                        lineRenderer.startColor = Color.white;
                        break;
                }
                switch (Random.Range(0, 8))
                {
                    case 0:
                    case 1:
                    case 2:
                        lineRenderer.endColor = Color.magenta;
                        break;
                    case 3:
                        lineRenderer.endColor = Color.yellow;
                        break;
                    case 4:
                    case 5:
                        lineRenderer.endColor = Color.cyan;
                        break;
                    case 6:
                    case 7:
                        lineRenderer.endColor = Color.white;
                        break;
                }
            } else
            {
                lineRenderer.material.color = color*4;
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }

            lineRenderer.SetColors(lineRenderer.startColor, lineRenderer.endColor);
            //lineRenderer.colorGradient = new Gradient(lineRenderer.startColor, lineRenderer.endColor);

            lineRenderer.startWidth = 0.0006f * Random.Range(1, 200);
            lineRenderer.positionCount = c;
            lineRenderer.useWorldSpace = true;

            float stepX = (pos1.x - pos2.x) / c;
            float stepY = (pos1.y - pos2.y) / c;
            float stepZ = (pos1.z - pos2.z) / c;

            lineRenderer.SetPosition(0, new Vector3(pos1.x + Random.Range(-5, 5) / 200f, pos1.y + Random.Range(-5, 5) / 200f, pos1.z + Random.Range(-5, 5) / 200f));
            for (int i = 1; i < c - 1; i++)
                lineRenderer.SetPosition(i, new Vector3(pos1.x - i * stepX + Random.Range(-100, 100) / 200f, pos1.y - i * stepY + Random.Range(-100, 100) / 200f, pos1.z - i * stepZ + Random.Range(-100, 100) / 200f));
            lineRenderer.SetPosition(c - 1, new Vector3(pos2.x, pos2.y, pos2.z));

            yield return new WaitForSeconds(0.075f);

            GameObject.Destroy(lineRenderer.gameObject);
            //yield break;
        }
    }

}
