using System;
using System.Collections;
using Dialogue.Events;
using SerializableDictionaryPackage.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class DialogueTextManager : BetterMonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text authorText;
        [SerializeField] private Image sprite;

        [SerializeField] private float waitTimer = 0.05f;
        [SerializeField] private float sentenceWaitTimer = 0.5f;

        [SerializeField] private SerializableDictionary<string, Sprite> imagesByNames;
        [SerializeField] private SerializableDictionary<char, float> characterTimes;

        private bool printing;
        private object eventInstance;
        private string nextLine;

        private void Start()
        {
            EventManager.AddListener<NextLineEvent>(OnLineChosen);
            EventManager.AddListener<ExitDialogueModeEvent>(OnExitDialogueModeEventHandler);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<NextLineEvent>(OnLineChosen);
            EventManager.RemoveListener<ExitDialogueModeEvent>(OnExitDialogueModeEventHandler);
        }
        private void OnExitDialogueModeEventHandler(ExitDialogueModeEvent @event)
        {
            gameObject.SetActive(false);
        }

        private void OnLineChosen(NextLineEvent @event)
        {
            StopAllCoroutines();
            StartCoroutine(HandleNextLine(@event.Line, @event.Author));
        }

        private IEnumerator HandleNextLine(string line, string author)
        {
            nextLine = line;

            printing = true;
            HandlePortrait(author);
            dialogueText.text = "";

            foreach (char letter in line)
            {
                // eventInstance.start();

                dialogueText.text += letter;
                yield return new WaitForSeconds(waitTimer);
            }

            yield return new WaitForSeconds(sentenceWaitTimer);

            FinishLine();
            Debug.Log(line);
        }

        private void FinishLine()
        {
            printing = false;
            nextLine = "";
        }

        private void HandlePortrait(string author)
        {
            if (imagesByNames.TryGetValue(author, out var byName))
            {
                sprite.GetComponent<Image>().sprite = byName;
            }
        }

        private void HandleAuthor()
        {
            throw new NotImplementedException();
        }
    }
}