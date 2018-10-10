#if !NewUI

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRUI;

namespace ProgressCounter
{
    public class ListViewController<T> : ListSettingsController
    {
        public Func<T> GetValue = () => default(T);
        public Action<T> SetValue = (_) => { };
        public Func<T, string> GetTextForValue = (_) => "?";

        public List<T> values;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = values.Count;
            var value = GetValue();

            numberOfElements = values.Count();
            idx = values.FindIndex(v => v.Equals(value));
        }

        protected override void ApplyValue(int idx)
        {
            SetValue(values[idx]);
        }

        protected override string TextForValue(int idx)
        {
            return GetTextForValue(values[idx]);
        }
    }

    public class BoolViewController : SwitchSettingsController
    {
        public Func<bool> GetValue = () => false;
        public Action<bool> SetValue = (a) => { };

        protected override bool GetInitValue()
        {
            return (GetValue());
        }

        protected override void ApplyValue(bool value)
        {
            SetValue(value);
        }

        protected override string TextForValue(bool value)
        {
            return (value) ? "ON" : "OFF";
        }
    }

    public class SubMenu
    {
        public Transform transform;

        public SubMenu(Transform transform)
        {
            this.transform = transform;
        }

        public BoolViewController AddBool(string name)
        {
            return AddToggleSetting<BoolViewController>(name);
        }

        public T AddListSetting<T>(string name) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);

            var volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.name = name;
            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;
            return newSettingsController;
        }


        public T AddToggleSetting<T>(string name) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newToggleSettingsController;
        }
    }

    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance = null;
        private MainMenuViewController _mainMenuViewController = null;
        private SimpleDialogPromptViewController prompt = null;

        private static MainSettingsTableCell tableCell = null;

        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("SettingsUI").AddComponent<SettingsUI>();
        }

        public static bool IsMenuScene(Scene scene)
        {
            return (scene.name == "Menu");
        }

        public static bool IsGameScene(Scene scene)
        {
            return (scene.name == "StandardLevel");
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (IsMenuScene(scene))
            {
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");
            }
        }

        public static void LogComponents(Transform t, string prefix = "=", bool includeScipts = false)
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "-->" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, prefix + "=", includeScipts);
            }
        }

        public static SubMenu CreateSubMenu(string name)
        {
            if (!IsMenuScene(SceneManager.GetActiveScene()))
            {
                Console.WriteLine("Cannot create settings menu when no in the main scene.");
                return null;
            }

            if (tableCell == null)
            {
                tableCell = Resources.FindObjectsOfTypeAll<MainSettingsTableCell>().FirstOrDefault();
                // Get a refence to the Settings Table cell text in case we want to change fint size, etc
                var text = tableCell.GetPrivateField<TextMeshProUGUI>("_settingsSubMenuText");
            }

            var temp = Resources.FindObjectsOfTypeAll<SettingsNavigationController>().FirstOrDefault();

            var others = temp.transform.Find("Others");
            var tweakSettingsObject = Instantiate(others.gameObject, others.transform.parent);
            Transform mainContainer = CleanScreen(tweakSettingsObject.transform);

            var tweaksSubMenu = new SettingsSubMenuInfo();
            tweaksSubMenu.SetPrivateField("_menuName", name);
            tweaksSubMenu.SetPrivateField("_controller", tweakSettingsObject.GetComponent<VRUIViewController>());

            var mainSettingsMenu = Resources.FindObjectsOfTypeAll<MainSettingsMenuViewController>().FirstOrDefault();
            var subMenus = mainSettingsMenu.GetPrivateField<SettingsSubMenuInfo[]>("_settingsSubMenuInfos").ToList();
            subMenus.Add(tweaksSubMenu);
            mainSettingsMenu.SetPrivateField("_settingsSubMenuInfos", subMenus.ToArray());

            SubMenu menu = new SubMenu(mainContainer);
            return menu;
        }

        private static Transform CleanScreen(Transform screen)
        {
            var container = screen.Find("Content").Find("SettingsContainer");
            var tempList = container.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
            return container;
        }
    }
}

#endif