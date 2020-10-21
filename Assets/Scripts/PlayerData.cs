﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerData : PersistentPlayerData 
{
    public bool IsOptionOpen { get; set; } = false;
    public GameObject ActivePanel { get; set; }
    private static PlayerData instance;
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerData();
            }
            return instance;
        }
    }
    public bool IsCreditsOpen { get; set; } = false;
    private readonly string saveFilePath;
    private PersistentPlayerData persistentPlayerData; // warper for all player data to be saved onto disk

    private PlayerData() : base()
    {
        saveFilePath = Application.persistentDataPath + @"\PersistentPlayerData.dat";
    }

    public static PersistentPlayerData SavePersistentPlayerData()
    {
        PersistentPlayerData persistentPlayerData = new PersistentPlayerData(Instance);
        using (FileStream fileStream = File.Create(Instance.saveFilePath))
        {
            try
            {
                new BinaryFormatter().Serialize(fileStream, persistentPlayerData);
            }
            catch (SerializationException)
            {
                //todo: add pop up notification system
            }
        }
        return persistentPlayerData;
    }

    public static PersistentPlayerData LoadPersistentPlayerData()
    {
        using (FileStream fileStream = File.OpenRead(Instance.saveFilePath))
        {
            try
            {
                Instance.persistentPlayerData = (PersistentPlayerData)new BinaryFormatter().Deserialize(fileStream);
            }
            catch (SerializationException)
            {
                Instance.persistentPlayerData = new PersistentPlayerData(Instance);
            }
        }
        ApplyPersistentPlayerData();
        Debug.Log($"Song location = {Instance.BeatmapLocation}");
        return Instance.persistentPlayerData;
    }

    public static void ApplyPersistentPlayerData()
    {
        Instance.BeatmapLocation = Instance.persistentPlayerData.BeatmapLocation;
        Instance.MusicVolume = Instance.persistentPlayerData.MusicVolume;
        Instance.SoundEffectsVolume = Instance.persistentPlayerData.SoundEffectsVolume;
    }
}