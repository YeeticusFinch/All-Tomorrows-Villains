using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SoundLib {
    public float volume = 1f;
    public float pitch = 1f;
    public float distMult = 1f;
    public AudioClip[] audioClips;
    //public string[] names;
    public Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public void init()
    {
        for (int i = 0; i < audioClips.Length; i++)
        {
            clips.Add(audioClips[i].name, audioClips[i]);
            Debug.Log("Added " + audioClips[i].name);
        }
    }

    public void playAt(string clip, Vector3 pos, float vol = 1f, float pitch = 1f, float maxDistance = 50f)
    {
        //Debug.Log("Searching for " + clip + " in " + clips);
        PlayClipAtPoint(clips[clip], pos, vol * volume, pitch*this.pitch, maxDistance*distMult);
    }

    GameObject PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch, float maxDistance)
    {
        GameObject obj = new GameObject();
        obj.transform.position = position;
        obj.AddComponent<AudioSource>();
        obj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        obj.GetComponent<AudioSource>().minDistance = 1;
        obj.GetComponent<AudioSource>().maxDistance = maxDistance;
        obj.GetComponent<AudioSource>().spatialBlend = 1f;
        obj.GetComponent<AudioSource>().pitch = pitch;
        obj.GetComponent<AudioSource>().PlayOneShot(clip, volume);
        GameObject.Destroy(obj, clip.length / pitch);
        return obj;
    }

}
