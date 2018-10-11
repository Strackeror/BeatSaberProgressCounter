using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProgressCounter
{
    internal class ProgressUI : MonoBehaviour
    {
        //Necessary because monobehaviours can't be generics
        public class DecimalSettingsViewController : ListViewController<int> { }
        public class PositionSettingsViewController : ListViewController<Tuple<Vector3, string>> { }

        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("Progress Counter");

            //Time Left Bool
            {
                var timeLeft = subMenu.AddBool("Time Left");

                timeLeft.GetValue = () => Plugin.progressTimeLeft;

                timeLeft.SetValue = (bool value) =>
                {
                    Plugin.progressTimeLeft = value;
                    ModPrefs.SetBool("BeatSaberProgressCounter", "progressTimeLeft", Plugin.progressTimeLeft);
                };
            }


            //Decimal Precision
            {
                int[] precisionValues = { 1, 2, 3, 4 };
                var precisionMenu = subMenu.AddListSetting<DecimalSettingsViewController>("Decimal Precision");
                precisionMenu.values = precisionValues.ToList();

                precisionMenu.GetValue = () => Plugin.progressCounterDecimalPrecision;

                precisionMenu.SetValue = (int value) =>
                {
                    Plugin.progressCounterDecimalPrecision = (int)value;

                    ModPrefs.SetFloat("BeatSaberProgressCounter", "progressCounterDecimalPrecision", value);
                };

                precisionMenu.GetTextForValue = (int value) => value + " Place" + ((value == 1) ? "s" : "");


            }

<<<<<<< HEAD

            //Timer Position Preset
            {
                var timerPositions = new List<Tuple<Vector3, string>>
                {
                    {Plugin.progressCounterPosition, "Current"},
                    {new Vector3(.25f, -2.0f, 7.5f), "Default"},
                    {new Vector3(.25f, 3.4f, 7.5f), "Top"},
                    {new Vector3(-3.0f, 3.4f, 7f), "Top Left"},
                    {new Vector3(3.5f, 3.4f, 7f), "Top Right"},
                    {new Vector3(-3.0f, -1.75f, 7f), "Bottom Left"},
                    {new Vector3(3.5f, -1.6f, 7f), "Bottom Right"},
                };

                var timerPositionMenu = subMenu.AddListSetting<PositionSettingsViewController>("Timer Position");
                timerPositionMenu.values = timerPositions;

                timerPositionMenu.GetValue = () => timerPositions[0];
                timerPositionMenu.GetTextForValue = (value) => value.Item2;
                timerPositionMenu.SetValue = (v) =>
                {
                    Plugin.progressCounterPosition = v.Item1;
                    ModPrefs.SetString("BeatSaberProgressCounter", "progressPosition",
                                        FormattableString.Invariant(
                                       $"{Plugin.progressCounterPosition.x:0.00},{Plugin.progressCounterPosition.y:0.00},{Plugin.progressCounterPosition.z:0.00}"));
                };
            }


=======
>>>>>>> upstream/master
            // Score Counter Position Preset
            {
                var scorePositions = new List<Tuple<Vector3, string>>
                {
                    {Plugin.scoreCounterPosition, "Current"},
                    {new Vector3(3.25f, 0.5f, 7.0f), "Default" },
                    {new Vector3(-3.25f, -0.3f, 7.0f), "Left"  },
                };

                var scorePositionPresetMenu = subMenu.AddListSetting<PositionSettingsViewController>("Score Counter Position");
                scorePositionPresetMenu.values = scorePositions;

                scorePositionPresetMenu.GetValue = () => scorePositions[0];
                scorePositionPresetMenu.GetTextForValue = (value) => value.Item2;
                scorePositionPresetMenu.SetValue = (v) =>
                {
                    Plugin.scoreCounterPosition = v.Item1;
                    ModPrefs.SetString("BeatSaberProgressCounter", "scorePosition",
                                        FormattableString.Invariant(
                                       $"{Plugin.scoreCounterPosition.x:0.00},{Plugin.scoreCounterPosition.y:0.00},{Plugin.scoreCounterPosition.z:0.00}"));
                };
            }

<<<<<<< HEAD

            //Score Counter Toggle
            {
                var scoreCounterToggle = subMenu.AddBool("Enable Score Counter");

                scoreCounterToggle.GetValue = () => Plugin.scoreCounterEnabled;

                scoreCounterToggle.SetValue = (bool value) =>
                {
                    Plugin.scoreCounterEnabled = value;
                    ModPrefs.SetBool("BeatSaberProgressCounter", "scoreCounterEnabled", Plugin.scoreCounterEnabled);
                };
            }


            //Personal Best Tracker Toggle
            {
                var pbTrackerToggle = subMenu.AddBool("Enable Personal Best Tracker");

                pbTrackerToggle.GetValue = () => Plugin.pbTrackerEnabled;

                pbTrackerToggle.SetValue = (bool value) =>
                {
                    Plugin.pbTrackerEnabled = value;
                    ModPrefs.SetBool("BeatSaberProgressCounter", "scoreCounterEnabled", Plugin.pbTrackerEnabled);
                };
            }
        }
    }
}
        
  
=======
            //Timer Position Preset
            {
                var timerPositions = new List<Tuple<Vector3, string>>
                {
                    {Plugin.progressCounterPosition, "Current"},
                    {new Vector3(.25f, -2.0f, 7.5f), "Default"},
                    {new Vector3(.25f, 3.4f, 7.5f), "Top"},
                    {new Vector3(-3.0f, 3.4f, 7f), "Top Left"},
                    {new Vector3(3.5f, 3.4f, 7f), "Top Right"},
                    {new Vector3(-3.0f, -1.75f, 7f), "Bottom Left"},
                    {new Vector3(3.5f, -1.6f, 7f), "Bottom Right"},
                };

                var timerPositionMenu = subMenu.AddListSetting<PositionSettingsViewController>("Timer Position");
                timerPositionMenu.values = timerPositions;

                timerPositionMenu.GetValue = () => timerPositions[0];
                timerPositionMenu.GetTextForValue = (value) => value.Item2;
                timerPositionMenu.SetValue = (v) =>
                {
                    Plugin.progressCounterPosition = v.Item1;
                    ModPrefs.SetString("BeatSaberProgressCounter", "progressPosition",
                                        FormattableString.Invariant(
                                       $"{Plugin.progressCounterPosition.x:0.00},{Plugin.progressCounterPosition.y:0.00},{Plugin.progressCounterPosition.z:0.00}"));
                };
            }

        }
    }
}
>>>>>>> upstream/master
