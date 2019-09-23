using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCModLoader;

using VRCUnlocked;
using VRCMenuUtils;
using VRChat.UI;


using UnityEngine;

namespace VRCMinorUI
{
    [VRCModInfo("VRCMinorUI", "0.1.1", "AtiLion")]
    internal class VRCMinorUI : VRCMod
    {
        #region Config Variables
        private UnlockedConfig<WorldsConfig> _config;
        #endregion
        #region Config Properties
        public WorldsConfig Config
        {
            get
            {
                if (_config == null)
                    _config = new UnlockedConfig<WorldsConfig>("VRCMinorUI.json", "VRCMinorUI");
                return _config?.Config;
            }
        }
        #endregion

        void OnApplicationStart()
        {
            if (Config == null)
                VRCModLogger.LogError("Failed to get config!");
            ModManager.StartCoroutine(Setup());
        }

        #region VRCMinorUI Variables
        private Dictionary<Transform, int> _worldsPositions = new Dictionary<Transform, int>();

        private Transform _worldsList;
        #endregion
        #region VRCMinorUI Coroutines
        private IEnumerator Setup()
        {
            // Wait for load
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Add wait for page shown
            VRCMenuUtilsAPI.OnPageShown += VRCMenuUtilsAPI_OnPageShown;

            // Wait for worlds
            while (_worldsList == null)
                yield return null;

            // Wait for favourite and recent
            while (_worldsList.Find("Playlist1") == null || _worldsList.Find("Recent") == null)
                yield return null;

            // Grab favourites and recent
            Transform[] favourites = new Transform[]
            {
                _worldsList.Find("Playlist1"),
                _worldsList.Find("Playlist2"),
                _worldsList.Find("Playlist3"),
                _worldsList.Find("Playlist4")
            };
            Transform recent = _worldsList.Find("Recent");

            // Grab the positions
            foreach (Transform favourite in favourites)
                _worldsPositions.Add(favourite, favourite.GetSiblingIndex());
            _worldsPositions.Add(recent, recent.GetSiblingIndex());

            // Change positions
            ChangePositions();

            // Set watchers
            _config.WatchForUpdate("FavouriteWorldsAtTop", () =>
                ChangePositions());
            _config.WatchForUpdate("RecentWorldsAtTop", () =>
                ChangePositions());

            VRCMenuUtilsAPI.OnPageShown -= VRCMenuUtilsAPI_OnPageShown;
            VRCModLogger.Log("Worlds UI positions setup!");
        }
        #endregion

        #region UI Event Handlers
        private void VRCMenuUtilsAPI_OnPageShown(VRCUiPage page)
        {
            if (page.GetType() != typeof(VRCUiPageWorlds))
                return;
            if (_worldsPositions.Count > 0) // Already initialized
                return;

            // Grab main transforms
            _worldsList = VRCEUi.WorldsScreen.transform.Find("Vertical Scroll View/Viewport/Content");
        }
        #endregion

        #region WorldsUI Functions
        private void ChangePositions()
        {
            KeyValuePair<Transform, int> element;

            for (int i = 0; i < 4; i++)
            {
                element = _worldsPositions.ElementAt(i);

                if ((bool)Config.FavouriteWorldsAtTop)
                    element.Key.SetSiblingIndex(i);
                else
                    element.Key.SetSiblingIndex(element.Value);
            }

            element = _worldsPositions.ElementAt(4);
            if ((bool)Config.RecentWorldsAtTop)
                element.Key.SetSiblingIndex(0);
            else
                element.Key.SetSiblingIndex(element.Value);
        }
        #endregion
    }
}
