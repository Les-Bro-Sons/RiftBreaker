using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RB_StatsParser))]
public class RB_CustomEditorStatsParser : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RB_StatsParser statsParser = (RB_StatsParser)target;

        if(GUILayout.Button("Encrypt file"))
        {
            statsParser.EncryptFile(statsParser.ItemsPath, statsParser.EncryptedItemPath);
        }

        if(GUILayout.Button("Decrypt file"))
        {
            statsParser.DecryptFile(statsParser.ItemsPath, statsParser.EncryptedItemPath);
        }
    }
}

public class RB_StatsParser : MonoBehaviour
{
    //File
    Dictionary<string, Dictionary<string, string>> _weaponsStats = new();
    Dictionary<string, Dictionary<string, string>> _playerStats = new();
    public string ItemsPath = Application.dataPath + "/items.txt";
    public string PlayerPath = Application.dataPath + "/player.txt";
    public string EncryptedItemPath = Application.dataPath + "/items.enc";
    public string EncryptedPlayerPath = Application.dataPath + "/player.enc";

    //Instance
    public static RB_StatsParser Instance;

    // Clé de chiffrement et vecteur d'initialisation (IV) fixes (à remplacer par des valeurs sécurisées)
    private static readonly byte[] key = Encoding.UTF8.GetBytes("CleSecrete123456");

    //Awake
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        if(File.Exists(ItemsPath) && !Application.isEditor)
            EncryptFile(ItemsPath, EncryptedItemPath);
        LoadWeaponStatsFromFile();
        LoadPlayerStatsFromFile();
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

    private void LoadWeaponStatsFromFile()
    {
        string[] lines = { };
        if (Application.isEditor)
        {
            if (!File.Exists(ItemsPath)) throw new Exception("File does not exist");
            lines = File.ReadAllLines(ItemsPath);
        }
        else
        {
            if (!File.Exists(EncryptedItemPath)) throw new Exception("File does not exist");
            lines = DecryptFileAndGetLines(EncryptedItemPath);
        }
        if (lines.Length <= 0) throw new NullReferenceException("The file is empty");
        string currentWeaponName = "";
        Dictionary<string, string> currentWeaponStats = new();
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Length <= 0)
            {
                if (currentWeaponStats.Count <= 0) { Debug.LogWarning("Stats is Empty"); continue; }
                _weaponsStats[currentWeaponName] = currentWeaponStats.ToDictionary(entry => entry.Key, entry => entry.Value);
                currentWeaponStats.Clear();
                continue;
            }
            string[] splittedLine = line.Split(' ');
            if (splittedLine.Length <= 0) continue;
            if (splittedLine.Length != 3 && splittedLine.Length != 1) { Debug.LogWarning($"line {i} is not valid"); continue; }
            if (line[0] == '[')
            {
                string subsedLine = splittedLine[0].Substring(1, splittedLine[0].Length - 2);
                currentWeaponName = subsedLine;
                continue;
            }
            if (string.IsNullOrEmpty(currentWeaponName)) { Debug.LogWarning("The weapon has no name"); continue; }
            if (splittedLine.Length != 3 || splittedLine[1] != "=") { Debug.LogWarning($"line {i} is not valid"); continue; }
            currentWeaponStats[(splittedLine[0])] = splittedLine[2];
        }
        print("weapon loaded");
        
    }

    private void LoadPlayerStatsFromFile()
    {
        string[] lines = { };
        if (Application.isEditor)
        {
            if (!File.Exists(PlayerPath)) throw new Exception("File does not exist");
            lines = File.ReadAllLines(PlayerPath);
        }
        else
        {
            if (!File.Exists(EncryptedPlayerPath)) throw new Exception("File does not exist");
            lines = DecryptFileAndGetLines(EncryptedPlayerPath);
        }
        if (lines.Length <= 0) throw new NullReferenceException("The file is empty");
        string currentPlayerStatRegion = "";
        Dictionary<string, string> currentPlayerStat = new();
        for (int i = 0; i < lines.Length+1; i++)
        {
            string line = "";
            if (i < lines.Length)
            {
                line = lines[i];
            }
            if (line.Length <= 0 || i >= line.Length)
            {
                if (currentPlayerStat.Count <= 0) { Debug.LogWarning("Stats is Empty"); continue; }
                print("entered");
                foreach(string stat in currentPlayerStat.Keys)
                {
                    print(stat);
                }
                _playerStats[currentPlayerStatRegion] = currentPlayerStat.ToDictionary(entry => entry.Key, entry => entry.Value);
                currentPlayerStat.Clear();
                continue;
            }
            string[] splittedLine = line.Split(' ');
            if (splittedLine.Length <= 0) continue;
            if (splittedLine.Length != 3 && splittedLine.Length != 1) { Debug.LogWarning($"line {i} is not valid"); continue; }
            if (line[0] == '[')
            {
                string subsedLine = splittedLine[0].Substring(1, splittedLine[0].Length - 2);
                currentPlayerStatRegion = subsedLine;
                continue;
            }
            if (string.IsNullOrEmpty(currentPlayerStatRegion)) { Debug.LogWarning("The weapon has no name"); continue; }
            if (splittedLine.Length != 3 || splittedLine[1] != "=") { Debug.LogWarning($"line {i} is not valid"); continue; }
            currentPlayerStat[(splittedLine[0])] = splittedLine[2];
        }
        print("player loaded");
    }

    #endregion

    public void SetWeaponStat(RB_Items weapon)
    {
        string nameOfWeapon = weapon.GetType().Name.Substring(3);
        if (_weaponsStats.TryGetValue(nameOfWeapon, out Dictionary<string, string> stats))
        {
            foreach (var stat in stats)
            {
                string statName = stat.Key;
                string value = stat.Value;
                Type typeOfWeapon = weapon.GetType();
                FieldInfo field = typeOfWeapon.GetField(statName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Debug.LogWarning($"Property {statName} not found or not part of RB_Items.");
                    continue;
                }

                Type propertyType = field.FieldType;
                object propertyValue = Convert.ChangeType(value, propertyType);
                
                if(propertyValue == null) { Debug.LogWarning($"{propertyValue} is not of type {propertyType} "); }

                try
                {
                    field.SetValue(weapon, propertyValue);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error setting {statName}: {ex.Message}");
                }
            }
        }
        else
        {
            throw new Exception("Weapon is not in database");
        }
    }
}

