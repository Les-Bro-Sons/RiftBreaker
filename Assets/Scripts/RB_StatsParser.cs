using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Net;





#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(RB_StatsParser))]
public class RB_CustomEditorStatsParser : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RB_StatsParser statsParser = (RB_StatsParser)target;

        if(GUILayout.Button("Encrypt file"))
        {
            statsParser.EncryptFile(statsParser.StatsPath, statsParser.EncryptedIStatsPath);
        }

        if(GUILayout.Button("Decrypt file"))
        {
            statsParser.DecryptFile(statsParser.StatsPath, statsParser.EncryptedIStatsPath);
        }
    }
}
#endif
public class RB_StatsParser : MonoBehaviour
{

    //File
    Dictionary<STATSCONTAINER, Dictionary<STATSREGION, Dictionary<DIFFICULTY, Dictionary<STATS, string>>>> _stats = new();
    public string StatsPath;
    public string EncryptedIStatsPath;

    //Instance
    public static RB_StatsParser Instance;

    // Clé de chiffrement et vecteur d'initialisation (IV) fixes (à remplacer par des valeurs sécurisées)
    private static readonly byte[] key = Encoding.UTF8.GetBytes("CleSecrete123456");

    //Awake
    private void Awake()
    {
        StatsPath = Application.dataPath + "/stats.txt";
        EncryptedIStatsPath = Application.dataPath + "/stats.enc";
        if (Instance == null)
            Instance = this;
        if(File.Exists(StatsPath) && !Application.isEditor)
            EncryptFile(StatsPath, EncryptedIStatsPath);
        LoadStats();
    }
    private void SetField(object objectToSet, string fieldName, object value)
    {
        FieldInfo field = objectToSet.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            throw new Exception($"Property {fieldName} not found or not part of RB_Items.");
        }

        Type propertyType = field.FieldType;
        value = Convert.ChangeType(value, propertyType);
        if (value == null) { Debug.LogWarning($"{value} is not of type {propertyType} "); }
        try
        {
            field.SetValue(objectToSet, value);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error setting {fieldName}: {ex.Message}");
        }
    }

    #region Encrypting file

    public void EncryptFile(string filePath, string encryptedFilePath)
    {
        try
        {
            byte[] bytesToEncrypt = File.ReadAllBytes(filePath);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV(); // Génère un IV aléatoire de la bonne taille
                byte[] iv = aesAlg.IV; // Récupère l'IV généré

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    // Écrit le IV en tête du fichier chiffré
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    byte[] encryptedBytesWithIV = new byte[iv.Length + encryptedBytes.Length];
                    Array.Copy(iv, 0, encryptedBytesWithIV, 0, iv.Length);
                    Array.Copy(encryptedBytes, 0, encryptedBytesWithIV, iv.Length, encryptedBytes.Length);

                    File.WriteAllBytes(encryptedFilePath, encryptedBytesWithIV);
                }
            }

            File.Delete(filePath);
            Debug.Log("Encryption successful. Encrypted file saved at: " + encryptedFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Encryption error: " + ex.Message);
        }
    }

    // Déchiffrer le fichier texte
    public void DecryptFile(string filePath, string encryptedFilePath)
    {
        try
        {
            byte[] encryptedBytesWithIV = File.ReadAllBytes(encryptedFilePath);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Récupère l'IV depuis le début du fichier chiffré
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(encryptedBytesWithIV, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedBytesWithIV, iv.Length, encryptedBytesWithIV.Length - iv.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    byte[] decryptedBytes = msDecrypt.ToArray();
                    File.WriteAllBytes(filePath, decryptedBytes);
                }
            }
            File.Delete(encryptedFilePath);
            Debug.Log("Decryption successful. Decrypted file saved at: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Decryption error: " + ex.Message);
        }
    }

    public string[] DecryptFileAndGetLines(string encryptedFilePath)
    {
        try
        {
            byte[] encryptedBytesWithIV = File.ReadAllBytes(encryptedFilePath);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Récupère l'IV depuis le début du fichier chiffré
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(encryptedBytesWithIV, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytesWithIV, iv.Length, encryptedBytesWithIV.Length - iv.Length))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(csDecrypt))
                {
                    // Lire toutes les lignes dans un tableau de string
                    string decryptedText = sr.ReadToEnd();
                    string[] decryptedLines = decryptedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    return decryptedLines;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Decryption error: " + ex.Message);
            return new string[0]; // Retourne un tableau vide en cas d'erreur
        }
    }

    #endregion

    #region Load from file


    private void LoadStats()
    {
        string[] lines = { };
        if (Application.isEditor)
        {
            if (!File.Exists(StatsPath)) throw new Exception($"File : {StatsPath} does not exist");
            lines = File.ReadAllLines(StatsPath);
        }
        else
        {
            if (!File.Exists(EncryptedIStatsPath)) throw new Exception("File does not exist");
            lines = DecryptFileAndGetLines(EncryptedIStatsPath);
        }
        if (lines.Length <= 0) throw new NullReferenceException("The file is empty");
        STATSREGION currentRegionName = new();
        STATSCONTAINER currentContainerName = new();
        Dictionary<DIFFICULTY, Dictionary<STATS, string>> currentStat = new();
        for (int i = 0; i < lines.Length + 1; i++)
        {
            string line = "";
            if (i < lines.Length)
            {
                line = lines[i];
            }
            if (line.Length <= 0 || i >= lines.Length)
            {
                if (currentStat.Count <= 0) { Debug.LogWarning($"Stats at line {i} is Empty "); continue; }

                _stats[currentContainerName][currentRegionName] = currentStat.ToDictionary(entry => entry.Key, entry => entry.Value);
                currentStat.Clear();
                continue;
            }
            string[] splittedLine = line.Split(' ');
            if (splittedLine.Length <= 0) continue;
            if (splittedLine.Length != 3 && splittedLine.Length != 1) { Debug.LogWarning($"line {i} is not valid"); continue; }
            string subsedLine = splittedLine[0].Substring(1, splittedLine[0].Length - 2);
            if (line[0] == '(')
            {
                currentContainerName = (STATSCONTAINER)Enum.Parse(typeof(STATSCONTAINER), subsedLine);
                if(!_stats.ContainsKey(currentContainerName))
                    _stats[currentContainerName] = new();
                continue;
            }
            if (line[0] == '[')
            {
                currentRegionName = (STATSREGION)Enum.Parse(typeof(STATSREGION), subsedLine);
                continue;
            }
            string lastOfSplittedLine = splittedLine[splittedLine.Length - 1];
            if (splittedLine.Length != 3 || splittedLine[1] != "=") { Debug.LogWarning($"line {i} is not valid"); continue; }
            string[] statsSplittedByDifficulty = lastOfSplittedLine.Split('|');
            List<DIFFICULTY> difficulties = new();
            difficulties.AddRange(Enum.GetValues(typeof(DIFFICULTY)));
            int statsSplittedByDifficultyIndex = 0;
            for (int j = 0; j < difficulties.Count; j++)
            {
                if (!currentStat.ContainsKey(difficulties[j]))
                    currentStat[difficulties[j]] = new();
                if (statsSplittedByDifficulty.Length > j)
                {
                    statsSplittedByDifficultyIndex = j;
                }
                if (Enum.TryParse(typeof(STATS), splittedLine[0].ToString(), out var stat))
                {
                    currentStat[difficulties[j]][(STATS)stat] = statsSplittedByDifficulty[statsSplittedByDifficultyIndex].ToString();
                }
            }
        }
        foreach(var container in _stats.Keys)
        {
            //print(container);
        }

        foreach (var statsValue in _stats.Values)
        {
            foreach(var region in  statsValue.Keys)
            {
                //print(region);
            }
        }
    }

    #endregion

    #region SetStats
    public void SetStats(object script, STATSCONTAINER statsContainer, STATSREGION statsRegion, DIFFICULTY difficulty)
    {
        string nameOfObject = script.GetType().Name.Substring(3);
        Dictionary<STATS, string> statsToGet = new();
        statsToGet = _stats[statsContainer][statsRegion][difficulty];

        foreach (var stat in statsToGet)
        {
            string statName = stat.Key.ToString();
            string value = stat.Value;

            SetField(script, statName, value);
        }
    }

    #endregion
}

