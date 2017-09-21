﻿// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System.Linq;
using UnityEngine;

namespace GameToolkit.Localization
{
	[CreateAssetMenu(fileName = "LocalizationSettings", menuName = "GameToolkit/Localization/Localization Settings", order = 0)]
    public sealed class LocalizationSettings : ScriptableObject
    {
        private const string AssetName = "LocalizationSettings";
        private static LocalizationSettings s_Instance = null;

        [Header("Google Translate")]
        [Tooltip("If you want to use Google Translate, attach the service account or API key file claimed from Google Cloud Console.")]
        public TextAsset GoogleAuthenticationFile;

        /// <summary>
        /// Gets the localization settings instance.
        /// </summary>
        public static LocalizationSettings Instance
        {
            get
            {
                if (!s_Instance)
                {
                    s_Instance = FindByResources();
                }

#if UNITY_EDITOR
                if (!s_Instance)
                {
                    s_Instance = CreateSettingsAndSave();
                }
#endif

                if (!s_Instance)
                {
                    Debug.LogWarning("No instance of " + AssetName + " found, using default values.");
                    s_Instance = ScriptableObject.CreateInstance<LocalizationSettings>();
                }
                return s_Instance;
            }
        }

        private static LocalizationSettings FindByResources()
        {
            return Resources.Load<LocalizationSettings>(AssetName);
        }

#if UNITY_EDITOR
        private static LocalizationSettings CreateSettingsAndSave()
        {
            var localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();

            // Saving during Awake() will crash Unity, delay saving until next editor frame.
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEditor.EditorApplication.delayCall += () => SaveAsset(localizationSettings);
            }
            else
            {
                SaveAsset(localizationSettings);
            }

            return localizationSettings;
        }

        private static void SaveAsset(LocalizationSettings localizationSettings)
        {
            var assetPath = "Assets/Resources/" + AssetName + ".asset";
            var directoryName = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            var uniqueAssetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);
            UnityEditor.AssetDatabase.CreateAsset(localizationSettings, uniqueAssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log(AssetName + " has been created.");
        }
#endif
    }
}