using HMUI;
using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using System.Globalization;
namespace ProgressCounter
{
    class ProgressUI : MonoBehaviour
    {

        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("Progress Counter");

            //Score Counter Enabled bool
            var scoreCounterToggle = subMenu.AddBool("Show Score Counter");

            scoreCounterToggle.GetValue += delegate
            {
                return Plugin.scoreCounterEnabled;
            };

            scoreCounterToggle.SetValue += delegate (bool value)
            {
                Plugin.scoreCounterEnabled = value;
                ModPrefs.SetBool("BeatSaberProgressCounter", "scoreCounterEnabled", Plugin.scoreCounterEnabled);
            };

            //Time Left Bool
            var timeLeft = subMenu.AddBool("Time Left");

            timeLeft.GetValue += delegate
            {
                return Plugin.progressTimeLeft;
            };

            timeLeft.SetValue += delegate (bool value)
            {
                Plugin.progressTimeLeft = value;
                ModPrefs.SetBool("BeatSaberProgressCounter", "progressTimeLeft", Plugin.progressTimeLeft);
            };


            //Decimal Precision
            float[] precisionValues = { 1, 2, 3, 4 };
            var precisionMenu = subMenu.AddList("Decimal Precision", precisionValues);

            precisionMenu.GetValue += delegate
            {
                return Plugin.progressCounterDecimalPrecision;
            };

            precisionMenu.SetValue += delegate (float value)
            {
                Plugin.progressCounterDecimalPrecision = (int)value;

                ModPrefs.SetFloat("BeatSaberProgressCounter", "progressCounterDecimalPrecision", value);
            };

            precisionMenu.FormatValue += delegate (float value)
            {
                if (value == 1)
                {
                    return value + " Place";
                }

                else
                {
                    return value + " Places";
                }

            };

            // Score Counter Position Preset
            float[] scorePresetValues = { 0, 1, 2 };
            var scorePositionPresetMenu = subMenu.AddList("Score Counter Position Preset", scorePresetValues);

            scorePositionPresetMenu.GetValue += delegate
            {
                return Plugin.progressCounterScorePositionPreset;
            };

            scorePositionPresetMenu.SetValue += delegate (float value)
            {
                Plugin.progressCounterScorePositionPreset = (int)value;

                ModPrefs.SetFloat("BeatSaberProgressCounter", "progressCounterScorePositionPreset", value);

                Plugin.setScoreCounterPosition( (int)value);
            };

            scorePositionPresetMenu.FormatValue += delegate (float value)
            {
                switch ((int)value)
                {
                    case 0:
                        return "User";
                    case 1:
                        return "Default";
                    case 2:
                        return "Left";
                    default:
                        return "";

                }

            };

            //Timer Position Preset
            float[] timerPresetValues = { 0, 1, 2, 3, 4, 5, 6 };
            var positionPresetMenu = subMenu.AddList("Timer Position Preset", timerPresetValues);

            positionPresetMenu.GetValue += delegate
            {
                return Plugin.progressCounterPositionPreset;
            };

            positionPresetMenu.SetValue += delegate (float value)
            {
                Plugin.progressCounterPositionPreset = (int)value;

                ModPrefs.SetFloat("BeatSaberProgressCounter", "progressCounterPositionPreset", value);

                Plugin.setProgressCounterPosition( (int)value);
            };

            positionPresetMenu.FormatValue += delegate (float value)
            {
                switch ((int)value)
                {
                    case 0:
                        return "User";
                    case 1:
                        return "Default";
                    case 2:
                        return "Top";
                    case 3:
                        return "Top Left";
                    case 4:
                        return "Top Right";
                    case 5:
                        return "Bottom Left";
                    case 6:
                        return "Bottom Right";
                    default:
                        return "";

                }

            };
            //Score Counter Enabled bool
            var localScoreCounterToggle = subMenu.AddBool("Show local Personal Best %");

            localScoreCounterToggle.GetValue += delegate
            {
                return Plugin.localScoreCounterEnabled;
            };

            localScoreCounterToggle.SetValue += delegate (bool value)
            {
                Plugin.localScoreCounterEnabled = value;
                ModPrefs.SetBool("BeatSaberProgressCounter", "localScoreCounterEnabled", Plugin.localScoreCounterEnabled);
            };





        }
    }
}
