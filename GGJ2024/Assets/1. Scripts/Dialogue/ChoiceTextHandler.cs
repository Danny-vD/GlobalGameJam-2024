using Dialogue.Events;
using TMPro;
using UnityEngine.UI;
using VDFramework;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class ChoiceTextHandler : BetterMonoBehaviour
    {
        public int index;
        public string text;
        private TMP_Text asset;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            asset = GetComponentInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClicked);
            GetComponentInChildren<TMP_Text>().text = text;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClicked);
        }

        public void SetValues(string text, int index)
        {
            asset.text = text;
            this.index = index;
        }

        private void OnClicked()
        {
            EventManager.RaiseEvent(new ChoiceSelectedEvent(index, false));
        }
    }
}