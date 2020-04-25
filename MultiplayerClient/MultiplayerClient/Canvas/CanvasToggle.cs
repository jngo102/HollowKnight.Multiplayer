using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerClient.Canvas
{
    public class CanvasToggle
    {
        public GameObject ToggleObject;
        private readonly GameObject _background;
        private readonly GameObject _checkmark;
        private readonly GameObject _label;
        private readonly Toggle _toggle;
        private string _toggleName;
        
        private bool _active;

        public CanvasToggle(GameObject parent, string name, Texture2D bgTexture, Texture2D checkTexture, Vector2 position, Vector2 size, Vector2 bgPos, Rect labelRect, Font font = null, string labelText = "", int fontSize = 13)
        {
            _toggleName = name;

            ToggleObject = new GameObject("Canvas Toggle - " + name);
            RectTransform toggleTransform = ToggleObject.AddComponent<RectTransform>();
            toggleTransform.sizeDelta = new Vector2(size.x, size.y);
            _toggle = ToggleObject.AddComponent<Toggle>();
            _toggle.transition = Selectable.Transition.ColorTint;

            ToggleObject.transform.SetParent(parent.transform, false);
            toggleTransform.SetScaleX(1.0f);
            toggleTransform.SetScaleY(1.0f);
            
            Vector2 pos = new Vector2((position.x + size.x / 2f) / Screen.width, (Screen.height - (position.y + size.y / 2f)) / Screen.height);
            toggleTransform.anchorMin = pos;
            toggleTransform.anchorMax = pos;
            Object.DontDestroyOnLoad(ToggleObject);

            _background = new GameObject("Canvas Toggle Background - " + name);
            RectTransform bgTransform = _background.AddComponent<RectTransform>();
            bgTransform.sizeDelta = new Vector2(bgTexture.width, bgTexture.height);
            bgTransform.position = bgPos;
            _background.AddComponent<CanvasRenderer>();
            Image bgImg = _background.AddComponent<Image>();
            _background.transform.SetParent(ToggleObject.transform, false);
            _toggle.targetGraphic = bgImg;
            Object.DontDestroyOnLoad(_background);

            _checkmark = new GameObject("Canvas Toggle Checkmark - " + name);
            RectTransform checkTransform = _checkmark.AddComponent<RectTransform>();
            checkTransform.sizeDelta = new Vector2(checkTexture.width, checkTexture.height);
            _checkmark.AddComponent<CanvasRenderer>();
            Image checkImg = _checkmark.AddComponent<Image>();
            checkImg.sprite = Sprite.Create(checkTexture, new Rect(0, 0, checkTexture.width, checkTexture.height), new Vector2(checkTexture.width / 2.0f, checkTexture.height / 2.0f));
            _toggle.graphic = checkImg;
            _checkmark.transform.SetParent(_background.transform, false);
            Object.DontDestroyOnLoad(_checkmark);
            
            _label = new GameObject("Canvas Toggle Label - " + name);
            RectTransform labelTransform = _label.AddComponent<RectTransform>();
            labelTransform.position = new Vector2(labelRect.position.x, labelRect.position.y);
            labelTransform.sizeDelta = new Vector2(labelRect.width, labelRect.height);
            _label.AddComponent<CanvasRenderer>();
            Text label = _label.AddComponent<Text>();
            label.text = labelText;
            label.alignment = TextAnchor.MiddleCenter;
            label.font = font;
            label.fontSize = fontSize;
            Outline labelOutline = _label.AddComponent<Outline>();
            labelOutline.effectColor = Color.black;
            _label.transform.SetParent(ToggleObject.transform, false);
            Object.DontDestroyOnLoad(_label);

            _active = true;
        }
        
        public void SetActive(bool active)
        {
            if (ToggleObject != null)
            {
                ToggleObject.SetActive(active);
                _active = active;
            }
        }

        public Vector2 GetPosition()
        {
            if (ToggleObject != null)
            {
                Vector2 anchor = ToggleObject.GetComponent<RectTransform>().anchorMin;
                Vector2 size = ToggleObject.GetComponent<RectTransform>().sizeDelta;

                return new Vector2(anchor.x * Screen.width - size.x / 2f, Screen.height - anchor.y * Screen.height - size.y / 2f);
            }

            return Vector2.zero;
        }

        public void SetPosition(Vector2 pos)
        {
            if (ToggleObject != null)
            {
                Vector2 sz = ToggleObject.GetComponent<RectTransform>().sizeDelta;
                Vector2 position = new Vector2((pos.x + sz.x / 2f) / Screen.width, (Screen.height - (pos.y + sz.y / 2f)) / Screen.height);
                ToggleObject.GetComponent<RectTransform>().anchorMin = position;
                ToggleObject.GetComponent<RectTransform>().anchorMax = position;
            }
        }
        
        public bool GetValue()
        {
            if (ToggleObject != null)
            {
                return true;
            }

            return false;
        }
        
        public void Destroy()
        {
            Object.Destroy(ToggleObject);
            Object.Destroy(_background);
            Object.Destroy(_label);
            Object.Destroy(_checkmark);
        }

        private void Log(object message) => Modding.Logger.Log("[Canvas Input] " + message);
    }
}