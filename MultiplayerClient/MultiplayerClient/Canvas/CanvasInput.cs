using UnityEngine;
using UnityEngine.Events;
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
            
            Vector2 pos = new Vector2((position.x + ((size.x / bgSubSection.width) * bgSubSection.width) / 2f) / Screen.width, (Screen.height - (position.y + ((size.y / bgSubSection.height) * bgSubSection.height) / 2f)) / Screen.height);
            inputTransform.anchorMin = pos;
            inputTransform.anchorMax = pos;
            Object.DontDestroyOnLoad(InputObject);
            
            _placeholderObj = new GameObject("Canvas Input Placeholder - " + name);
            _placeholderObj.AddComponent<RectTransform>().sizeDelta = new Vector2(bgSubSection.width, bgSubSection.height);
            Text placeholderTxt = _placeholderObj.AddComponent<Text>();
            placeholderTxt.text = placeholderText;
            placeholderTxt.font = font;
            placeholderTxt.color = new Color(0, 0, 0, 0.5f);
            placeholderTxt.fontSize = fontSize;
            placeholderTxt.alignment = TextAnchor.MiddleCenter;
            _placeholderObj.transform.SetParent(InputObject.transform, false);
            Object.DontDestroyOnLoad(_placeholderObj);
            
            _textObject = new GameObject("Canvas Input Text - " + name);
            _textObject.AddComponent<RectTransform>().sizeDelta = new Vector2(bgSubSection.width, bgSubSection.height);
            Text textTxt = _textObject.AddComponent<Text>();
            textTxt.text = inputText;
            textTxt.font = GUIController.Instance.perpetua;
            textTxt.fontSize = fontSize;
            textTxt.color = Color.black;
            textTxt.alignment = TextAnchor.MiddleCenter;
            _textObject.transform.SetParent(InputObject.transform, false);
            Object.DontDestroyOnLoad(_textObject);

            _inputField.targetGraphic = image;
            _inputField.placeholder = placeholderTxt;
            _inputField.textComponent = textTxt;
            _inputField.text = inputText;
            
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

                return new Vector2(anchor.x * Screen.width - size.x / 2f, Screen.height - anchor.y * Screen.height - size.y / 2f);
            }

            return Vector2.zero;
        }

        public void SetPosition(Vector2 pos)
        {
            if (InputObject != null)
            {
                Vector2 sz = InputObject.GetComponent<RectTransform>().sizeDelta;
                Vector2 position = new Vector2((pos.x + sz.x / 2f) / Screen.width, (Screen.height - (pos.y + sz.y / 2f)) / Screen.height);
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

        public void SetActive()
        {
            _inputField.Select();
            _inputField.ActivateInputField();
        }

        public void AddClickEvent(UnityAction<string> action)
        {
            if (_textObject != null)
            {
                _textObject.GetComponent<Button>().onClick.AddListener(SetActive);
            }
        }

        public void ChangePlaceholder(string text)
        {
            _placeholderObj.GetComponent<Text>().text = text;
        }
        
        public void Destroy()
        {
            Object.Destroy(InputObject);
            Object.Destroy(_placeholderObj);
            Object.Destroy(_textObject);
        }

        private void Log(object message) => Modding.Logger.Log("[Canvas Input] " + message);
    }
}