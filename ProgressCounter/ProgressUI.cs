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
namespace ProgressCounter
{
    class ProgressUI : MonoBehaviour
    {

        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("Progress Counter");

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
                Plugin.progressCounterDecimalPrecision = value;

                ModPrefs.SetFloat("BeatSaberProgressCounter", "progressCounterDecimalPrecision", value);
            };

            precisionMenu.FormatValue += delegate (float value)
            {
                if (value == 1)
                {
                    return value + " Decimal Place";
                }

                else
                {
                    return value + " Decimal Places";
                }

            };


        }


    }
}
