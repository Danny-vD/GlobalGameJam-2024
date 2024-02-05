using System;
using System.Threading;
using Dialogue.Events;
using Ink.Parsed;
using TMPro;
using VDFramework;
using VDFramework.EventSystem;
using Button = UnityEngine.UI.Button;

namespace Dialogue
{
    public class ChoiceTextHandler : BetterMonoBehaviour
    {
        public int index;
        public string text;
        private Button button;
        private TMP_Text asset;

        private void Awake()
        {
            button = GetComponent<Button>();
            asset = GetComponentInChildren<TMP_Text>();
        }

        public void SetValues(string text, int index)
        {
            asset.text = text;
            this.index = index;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public void SetIndex(int index)
        {
            this.index = index;
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

        private void OnClicked()
        {
            EventManager.RaiseEvent<OnChooseNextDialogueLineEvent>(new OnChooseNextDialogueLineEvent(index));
        }
    }
}