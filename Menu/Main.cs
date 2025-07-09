using MelonLoader;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using WatchTemplate.Classes;
using static WatchTemplate.PluginInfo;

namespace WatchTemplate.Menu
{
    public class Main : MelonMod
    {
        public static GameObject WatchObject = null;
        public static GorillaHuntComputer WatchObjectComponent = null;
        public static GameObject Menu = null;
        public static bool Status = false;
        private bool wasTouching = false;
        private static List<GameObject> buttonObjects = new List<GameObject>();

        public override void OnApplicationStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Menu.Buttons>();
            CreateWatch();
            StartUpItems();
        }

        public static void StartUpItems()
        {
            Materials.SpriteDefault();
            Materials.Standard();
        }

        public override void OnUpdate()
        {
            try
            {
                if (WatchObject == null)
                {
                    CreateWatch();
                }

                if (WatchObjectComponent != null)
                {
                    CleanUpWatchComponents();
                    UpdateWatchText();
                }

                HandleWatchInteraction();
            }
            catch (Exception ex)
            {
                MelonLogger.Msg($"Error in update: {ex.Message}");
            }
        }

        private void HandleWatchInteraction()
        {
            float distance = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, WatchObject.transform.position);
            bool isTouching = distance < 0.1f;

            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Vector3 offset = leftHand.up * -0.32f + leftHand.forward * -0.01f + leftHand.right * -0.005f;

            if (isTouching && !wasTouching && Menu == null) // opem
            {
                CreateMenu(leftHand, offset);
                Status = true;
            }
            else if (!isTouching && wasTouching && Menu != null) //close nw
            {
                UnityEngine.Object.Destroy(Menu);
                Menu = null;
                Status = false;
            }
            else if (Menu != null)
            {
                UpdateMenuPosition(leftHand, offset);
            }

            wasTouching = isTouching;
        }


        private static void CleanUpWatchComponents()
        {
            UnityEngine.Object.Destroy(WatchObjectComponent.material);
            UnityEngine.Object.Destroy(WatchObjectComponent.face);
            UnityEngine.Object.Destroy(WatchObjectComponent.badge);
            UnityEngine.Object.Destroy(WatchObjectComponent.hat);
            WatchObjectComponent.text.text = string.Empty;
        }

        private static void UpdateWatchText()
        {
            string watchText = $"{PluginInfo.MenuName}\n{PluginInfo.Version}\nStatus: {(Status ? "Open" : "Closed")}";
            WatchObjectComponent.text.text = watchText;
        }

        private static void CreateMenu(Transform leftHand, Vector3 offset)
        {
            Menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Menu.transform.position = leftHand.position + offset;
            Menu.transform.rotation = leftHand.rotation;
            Menu.transform.localScale = new Vector3(0.01f, 0.35f, 0.25f);

            var menuRenderer = Menu.GetComponent<Renderer>();
            menuRenderer.material = new Material(Materials.Standard());
            menuRenderer.material.color = MenuSettings.MenuColor;
        }

        private static void UpdateMenuPosition(Transform leftHand, Vector3 offset)
        {
            Menu.transform.position = leftHand.position + offset;
            Menu.transform.rotation = leftHand.rotation;
        }

        public static void CreateWatch()
        {
            if (WatchObject != null) return;

            var huntComputer = GorillaTagger.Instance.offlineVRRig.huntComputer;
            WatchObject = PhotonNetwork.Instantiate("gorillaprefabs/gorillahuntmanager",GorillaTagger.Instance.leftHandTransform.position,GorillaTagger.Instance.leftHandTransform.rotation, 0,null);

            WatchObjectComponent = huntComputer.GetComponent<GorillaHuntComputer>();
            CleanUpWatchComponents();
            UpdateWatchText();
        }
    }
}
