using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace WatchTemplate.NotificationLib
{
    public class Notifications
    {
        public static float Cooldown { get; set; } = 0.5f;
        public static bool Enabled { get; set; } = true;
        public static int MaxLineCount { get; set; } = 110;
        public static int LineGap { get; set; } = 5;
        public static float FadeTime { get; set; } = 1f;
        public static bool ShowTime { get; set; } = false;

        private static float lastNotificationTime;
        private static int lineCount;
        private static Text notificationDisplay;

        private static GameObject container;
        private static GameObject canvasObject;
        private static Camera MainCamera;

        public static void Initialize() // so ahh made in like 10 mins
        {
            MainCamera = Camera.main;
            if (MainCamera == null) return;

            container = new GameObject("DOMSNOTIFICATIONSCONTAINER")
            {
                transform = { position = MainCamera.transform.position }
            };

            canvasObject = new GameObject("DOMSNOTIFICATIONSCANVAS");
            canvasObject.transform.SetParent(container.transform);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = MainCamera;

            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.dynamicPixelsPerUnit = 2f;
            canvasScaler.referencePixelsPerUnit = 2000f;
            canvasScaler.scaleFactor = 1f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var rect = canvasObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(5f, 5f);
            rect.localPosition = new Vector3(0f, 0f, 1.6f);
            rect.localScale = Vector3.one;
            rect.rotation = Quaternion.Euler(0f, -270f, 0f);

            var textObj = new GameObject("NotificationText");
            textObj.transform.SetParent(canvasObject.transform);
            //SHOULD BE BOTTOM LEFT ADJUST IF NEEDED
            notificationDisplay = textObj.AddComponent<Text>();
            notificationDisplay.fontSize = 8; // might need to be bigger is a little small
            notificationDisplay.rectTransform.sizeDelta = new Vector2(260f, 160f);
            notificationDisplay.rectTransform.localScale = new Vector3(0.015f, 0.015f, 1.5f);
            notificationDisplay.rectTransform.localPosition = new Vector3(-5f, -1.5f, 0f);
            notificationDisplay.material = new Material(Shader.Find("GUI/Text Shader"));
            notificationDisplay.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            notificationDisplay.color = Color.white;
            notificationDisplay.alignment = TextAnchor.UpperLeft;
        }

        public static void Update() // uh pretty much update pos and remove lines
        {
            if (notificationDisplay == null || string.IsNullOrEmpty(notificationDisplay.text)) return;
            container.transform.position = MainCamera.transform.position;
            container.transform.rotation = MainCamera.transform.rotation;
            lineCount = notificationDisplay.text.Count(c => c == '\n') + 1;
            if (lineCount > MaxLineCount)
            {
                var lines = notificationDisplay.text
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1);
                notificationDisplay.text = string.Join(Environment.NewLine, lines);
                lineCount = notificationDisplay.text.Count(c => c == '\n') + 1;
            }
            else
            {
                lineCount = 0; 
            }
        }

        public static void SendNotificationTagged(string tagcolor, string tag, string textcolor = "white", string text = "A Empty Notification Was Sent, Menu Owner Do Better!") // send a notification like [TAG] notification
        {
            if (!Enabled || notificationDisplay == null || Time.time - lastNotificationTime < Cooldown) return;
            lastNotificationTime = Time.time;
            string FormattedText = $"<color={tagcolor}>{tag}</color> <color={textcolor}>{text}</color>{Environment.NewLine}";
            notificationDisplay.text += FormattedText;
        }

        public static void SendNotification(string textcolor = "white", string text = "A Empty Notification Was Sent, Menu Owner Do Better!") // send a notification like notification
        {
            if (!Enabled || notificationDisplay == null || Time.time - lastNotificationTime < Cooldown) return;
            lastNotificationTime = Time.time;
            string FormattedText = $"<color={textcolor}>{text}</color>{Environment.NewLine}";
            notificationDisplay.text += FormattedText;
        }

        public static void Clear()
        {
            if (!Enabled || notificationDisplay == null) return;
            notificationDisplay.text = string.Empty;
            lineCount = 0;
        }
    }
}
