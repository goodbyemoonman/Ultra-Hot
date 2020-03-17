using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> clips;

    public void PlaySE(GameObject who, params AudioClipList[] _clips)
    {
        AudioSource source = GetAudioSource(who);
        source.playOnAwake = false;
        source.clip = clips[(int)_clips[Random.Range(0, _clips.Length)]];
        source.pitch = Time.timeScale * Random.Range(0.9f, 1.1f);
        source.maxDistance = 10f;
        source.spatialBlend = 0.8f;
        source.Play();
    }

    public void PlaySE(GameObject who, float pitch, params AudioClipList[] _clips)
    {
        AudioSource source = GetAudioSource(who);
        source.playOnAwake = false;
        source.clip = clips[(int)_clips[Random.Range(0, _clips.Length)]];
        source.pitch = pitch * Random.Range(0.9f, 1.1f);
        source.maxDistance = 10f;
        source.spatialBlend = 0.8f;
        source.Play();
    }

    AudioSource GetAudioSource(GameObject who)
    {
        AudioSource[] sources = who.GetComponents<AudioSource>();
        AudioSource result;

        result = FindOffSource(sources);

        if(result == null && sources.Length > 3)
        {
            result = FindOldestSource(sources);
        }

        if(result == null)
            result = who.AddComponent<AudioSource>();

        return result;
    }

    AudioSource FindOldestSource(AudioSource[] sources)
    {
        if (sources.Length == 0)
            return null;

        AudioSource result = sources[0];
        float duration = 0f;
        for (int i = 0; i < sources.Length; i++)
        {
            float t = sources[i].time / sources[i].clip.length;
            if (t > duration)
            {
                duration = t;
                result = sources[i];
            }
        }
        return result;
    }

    AudioSource FindOffSource(AudioSource[] sources)
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].isPlaying == false)
                return sources[i];
        }

        return null;
    }

    [ContextMenu("Create Enum List")]
    public void CreateEnumList()
    {
        string path = "Assets/Template/EnumTemplate.txt";
        string enumTemplate = File.ReadAllText(path);
        string SoundList = "";
        string data = "";
        for (int i = 0; i < clips.Count; i++)
        {
            data += ("\t" + clips[i].name + " = " + i.ToString() + ", \n");
        }
        SoundList = enumTemplate.Replace("$ENUMNAME$", "AudioClipList");
        SoundList = SoundList.Replace("$ENUMDATA$", data);

        string savePath = "Assets/Scripts/EnumLists/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        string fullPath = savePath + "AudioClipList.cs";
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        File.WriteAllText(fullPath, SoundList);
    }
}

