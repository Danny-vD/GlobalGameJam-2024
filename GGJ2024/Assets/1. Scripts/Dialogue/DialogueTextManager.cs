using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private SerializableDictionary<string, float> specialCharacterTimes;

        [SerializeField] private List<string> tags;
        private object eventInstance;
        private string nextLine;

        private bool printing;

        private void OnEnable()
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
            var searchString = "<wave>";

            if (line.Contains(searchString))
            {
                // finds index of the first part of the searchString
                var startIndex = line.IndexOf(searchString);


                // creates the second part of the searchString
                searchString = "</" + searchString.Substring(1);

                // finds the index of the starting section of the endIndex
                var endIndex = line.IndexOf(searchString);

                // constructs a substring from the startingIndex 
                var substring = line.Substring(startIndex, endIndex + searchString.Length - startIndex);

                Debug.Log(substring);
                line = line.Remove(line.IndexOf("<wave>"), searchString.Length - 1);

                Debug.Log(line);
                line = line.Remove(line.IndexOf("</wave>"), searchString.Length);
                Debug.Log(line);
                // line = ParseMarkup(line, substring);
            }


            printing = true;
            dialogueText.text = "";
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = line;
            nextLine = line;
            HandlePortrait(author);
            HandleAuthor(author);


            foreach (var letter in line)
            {
                dialogueText.maxVisibleCharacters++;

                if (specialCharacterTimes.TryGetValue(letter.ToString(), out var specialTime))
                    yield return new WaitForSeconds(specialTime);
                else
                    yield return new WaitForSeconds(waitTimer);
            }

            yield return new WaitForSeconds(sentenceWaitTimer);

            FinishLine();
        }

        private string ParseMarkup(string line, string markup)
        {
            return line;
        }

        private void FinishLine()
        {
            printing = false;
            nextLine = "";
            EventManager.RaiseEvent(new FinishedLineEvent());
        }

        private void HandlePortrait(string author)
        {
            if (imagesByNames.TryGetValue(author, out var byName)) sprite.GetComponent<Image>().sprite = byName;
        }

        private void HandleAuthor(string author)
        {
            authorText.text = author;
        }
    }
}