using IL.HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiplayerClient.Canvas
{
    public class CanvasInput
    {
        public GameObject InputObject;
        private readonly GameObject _textObject;
        private readonly GameObject _placeholderObj;
        private readonly InputField _inputField;
        private string _inputName;
        
        private bool _active;

        public CanvasInput(GameObject parent, string name, Texture2D texture, Vector2 position, Vector2 size, Rect bgSubSection, Font font = null, string inputText = "", string placeholderText = "", int fontSize = 13)
        {
            if (size.x == 0 || size.y == 0)
            {
                size = new Vector2(bgSubSection.width, bgSubSection.height);
            }

            _inputName = name;

            InputObject = new GameObject("Canvas Input - " + name);
            InputObject.AddComponent<CanvasRenderer>();
            RectTransform inputTransform = InputObject.AddComponent<RectTransform>();
            inputTransform.sizeDelta = new Vector2(bgSubSection.width, bgSubSection.height);
            Image image = InputObject.AddComponent<Image>();
            image.sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(texture.width / 2, texture.height / 2)
            );
            image.type = Image.Type.Simple;
            _inputField = InputObject.AddComponent<InputField>();

            InputObject.transform.SetParent(parent.transform, false);
            inputTransform.SetScaleX(size.x / bgSubSection.width);
            inputTransform.SetScaleY(size.y / bgSubSection.height);
            
            Vector2 pos = new Vector2((position.x + ((size.x / bgSubSection.width) * bgSubSection.width) / 2f) / 1920f, (1080f - (position.y + ((size.y / bgSubSection.height) * bgSubSection.height) / 2f)) / 1080f);
            inputTransform.anchorMin = pos;
            inputTransform.anchorMax = pos;
            
            Object.DontDestroyOnLoad(InputObject);
            
            _placeholderObj = new GameObject("Canvas Input Placeholder - " + name);
            _placeholderObj.AddComponent<RectTransform>().sizeDelta = new Vector2(bgSubSection.width, bgSubSection.height);
            Text placeholderTxt = _placeholderObj.AddComponent<Text>();
            placeholderTxt.text = inputText;
            placeholderTxt.font = font;
            placeholderTxt.fontSize = fontSize;
            placeholderTxt.alignment = TextAnchor.MiddleCenter;
            Outline placeholderOutline = _placeholderObj.AddComponent<Outline>();
            placeholderOutline.effectColor = Color.black;
            _placeholderObj.transform.SetParent(InputObject.transform, false);
            Object.DontDestroyOnLoad(_placeholderObj);
            
            _textObject = new GameObject("Canvas Input Text - " + name);
            _textObject.AddComponent<RectTransform>().sizeDelta = new Vector2(bgSubSection.width, bgSubSection.height);
            Text textTxt = _textObject.AddComponent<Text>();
            textTxt.text = inputText;
            textTxt.font = font;
            textTxt.fontSize = fontSize;
            textTxt.alignment = TextAnchor.MiddleCenter;
            Outline textOutline = _textObject.AddComponent<Outline>();
            textOutline.effectColor = Color.black;
            _textObject.transform.SetParent(InputObject.transform, false);
            Object.DontDestroyOnLoad(_textObject);

            _inputField.placeholder = placeholderTxt; 
            _inputField.textComponent = textTxt;
            
            _active = true;
        }
        
        public void SetActive(bool active)
        {
            if (InputObject != null)
            {
                InputObject.SetActive(active);
                _active = active;
            }
        }

        public Vector2 GetPosition()
        {
            if (InputObject != null)
            {
                Vector2 anchor = InputObject.GetComponent<RectTransform>().anchorMin;
                Vector2 size = InputObject.GetComponent<RectTransform>().sizeDelta;

                return new Vector2(anchor.x * 1920f - size.x / 2f, 1080f - anchor.y * 1080f - size.y / 2f);
            }

            return Vector2.zero;
        }

        public void SetPosition(Vector2 pos)
        {
            if (InputObject != null)
            {
                Vector2 sz = InputObject.GetComponent<RectTransform>().sizeDelta;
                Vector2 position = new Vector2((pos.x + sz.x / 2f) / 1920f, (1080f - (pos.y + sz.y / 2f)) / 1080f);
                InputObject.GetComponent<RectTransform>().anchorMin = position;
                InputObject.GetComponent<RectTransform>().anchorMax = position;
            }
        }
        
        public string GetText()
        {
            if (InputObject != null)
            {
                return _textObject.GetComponent<Text>().text;
            }

            return null;
        }

        public void Focus()
        {
            _inputField.Select();
            _inputField.ActivateInputField();
        }

        public void ChangePlaceholder(string text)
        {
            _placeholderObj.GetComponent<Text>().text = text;
        }
        
        public void Destroy()
        {
            Object.Destroy(InputObject);
            Object.Destroy(_textObject);
        }

        private void Log(object message) => Modding.Logger.Log("[Canvas Input] " + message);
    }
}