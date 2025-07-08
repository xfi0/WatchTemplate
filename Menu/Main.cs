using MelonLoader;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
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
            CreateAllButtons(); // Create all buttons at startup
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

            if (isTouching && !wasTouching && Menu == null)
            {
                CreateMenu(leftHand, offset);
                Status = true;
                SetButtonVisibility(true);
            }
            else if (Menu != null)
            {
                UpdateMenuPosition(leftHand, offset);
            }
            else if (!Status)
            {
                SetButtonVisibility(false);
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

            ParentButtonsToMenu(); // Parent and position buttons here
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
            WatchObject = PhotonNetwork.Instantiate(
                "gorillaprefabs/gorillahuntmanager",
                GorillaTagger.Instance.leftHandTransform.position,
                GorillaTagger.Instance.leftHandTransform.rotation,
                0,
                null
            );

            WatchObjectComponent = huntComputer.GetComponent<GorillaHuntComputer>();
            CleanUpWatchComponents();
            UpdateWatchText();
        }

        public static void CreateAllButtons()
        {
            ClearExistingButtons();

            float buttonOffset = 0.1f;
            float buttonSpacing = 0.1f;

            // Create buttons from the Buttons class configuration
            foreach (var buttonGroup in Buttons.buttons)
            {
                foreach (var buttonInfo in buttonGroup)
                {
                    CreateButton(buttonOffset, buttonInfo);
                    buttonOffset += buttonSpacing;
                }
            }

            // Create disconnect button separately if needed
            CreateDisconnectButton(buttonOffset);

            SetButtonVisibility(false);
        }

        private static void ClearExistingButtons()
        {
            foreach (var button in buttonObjects)
            {
                if (button != null) UnityEngine.Object.Destroy(button);
            }
            buttonObjects.Clear();
        }

        public static void CreateButton(float offset, ButtonInfo buttonInfo)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.layer = 2;
            UnityEngine.Object.Destroy(button.GetComponent<Rigidbody>());
            button.GetComponent<BoxCollider>().isTrigger = true;

            // Parent will be set later, so position is temporary world position now
            // We'll reposition properly on parenting

            // Enable renderer so buttons are visible
            var renderer = button.GetComponent<Renderer>();
            renderer.enabled = true;

            // Scale and position will be set after parenting
            buttonObjects.Add(button);

            var buttonComponent = button.AddComponent<ButtonComponent>();
            buttonComponent.Initialize(buttonInfo);
        }

        public static void CreateDisconnectButton(float offset)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.layer = 2;
            UnityEngine.Object.Destroy(button.GetComponent<Rigidbody>());
            button.GetComponent<BoxCollider>().isTrigger = true;

            var renderer = button.GetComponent<Renderer>();
            renderer.enabled = true;

            buttonObjects.Add(button);

            var buttonInfo = new ButtonInfo
            {
                ButtonText = "Disconnect",
                EnableAction = () => PhotonNetwork.Disconnect(),
                Togglable = false,
                Notification = "Disconnecting from room..."
            };

            var buttonComponent = button.AddComponent<ButtonComponent>();
            buttonComponent.Initialize(buttonInfo);
        }

        private static void ParentButtonsToMenu()
        {
            float buttonOffset = 0.1f;
            float buttonSpacing = 0.1f;
            int i = 0;

            foreach (var button in buttonObjects)
            {
                if (button != null)
                {
                    button.transform.parent = Menu.transform;
                    button.transform.localScale = new Vector3(0.06f, 0.9f, 0.09f);
                    button.transform.localPosition = new Vector3(0.56f, 0f, 0.32f - buttonOffset - i * buttonSpacing);

                    // Ensure renderer enabled (in case something disables it)
                    var renderer = button.GetComponent<Renderer>();
                    renderer.enabled = true;

                    i++;
                }
            }
        }

        private static void SetButtonVisibility(bool visible)
        {
            foreach (var button in buttonObjects)
            {
                if (button != null)
                {
                    button.SetActive(visible);
                }
            }
        }
    }

    public class ButtonComponent : MonoBehaviour
    {
        public ButtonInfo ButtonInfo { get; private set; }
        public string RelatedText { get; set; }

        public void Initialize(ButtonInfo buttonInfo)
        {
            ButtonInfo = buttonInfo;
            RelatedText = buttonInfo.ButtonText;
        }

        void OnTriggerEnter(Collider other)
        {
            if (ButtonInfo != null)
            {
                ButtonInfo.EnableAction?.Invoke();
                if (!string.IsNullOrEmpty(ButtonInfo.Notification))
                {
                    NotificationLib.Notifications.SendNotificationTagged("green", "TOOLTIP", "white", ButtonInfo.Notification);
                }
            }
        }
    }
}
