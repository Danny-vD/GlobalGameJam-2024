using System.Collections;
using System.Collections.Generic;
using Dialogue.Events;
using FMOD.Studio;
using Ink.Runtime;
using InputScripts;
using InputScripts.Enums;
using SerializableDictionaryPackage.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework.EventSystem;
using VDFramework.Singleton;
using VDFramework.UnityExtensions;

namespace Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private bool printing;
        private string nextLine;
        public bool Conversing { get; private set; }
        
        private const float WaitTimer = 0.05f;
        private const float SentenceWaitTimer = 0.5f;

        private Story currentStory;

        private TMP_Text[] choicesTextBoxes;
        private EventInstance eventInstance;

        
        [SerializeField] public GameObject dialogueCanvas;
        [SerializeField] private GameObject sprite;
        [SerializeField] private SerializableDictionary<string, Sprite> imagesByNames;
        [SerializeField] public TextMeshProUGUI nameText;
        [SerializeField] public TextMeshProUGUI dialogueText;
        [SerializeField] public GameObject ChoicesPanel;
        [SerializeField] private SerializableDictionary<char, float> characterTimes;
        
        private void Start()
        {
            printing = false;
            Conversing = false;

            InputControlManager.Instance.playerControls.DialogueInteraction.Start.performed += _ =>
            {
                InputControlManager.Instance.ChangeControls(ControlTypes.Menus);
            };

            InputControlManager.Instance.playerControls.DialogueInteraction.Option1.performed +=
                _ => { ContinueDialogue(0); };
            InputControlManager.Instance.playerControls.DialogueInteraction.Option2.performed +=
                _ => { ContinueDialogue(1); };
            InputControlManager.Instance.playerControls.DialogueInteraction.Option3.performed +=
                _ => { ContinueDialogue(2); };

            EventManager.AddListener<ChooseNextDialogueLineEvent>(ContinueDialogue);
           //  eventInstance = AudioPlayer.GetEventInstance(AudioEventType.SFX_UI_Talking);
        }

        private void OnEnable()
        {
            EnterDialogueModeEvent.Listeners += EnterDialogueMode;
        }

        private void OnDisable()
        {
            EnterDialogueModeEvent.Listeners -= EnterDialogueMode;
        }

        private void EnterDialogueMode(EnterDialogueModeEvent enterDialogueModeEvent)
        {
            InputControlManager.Instance.ChangeControls(ControlTypes.Dialogue);
            
            currentStory = new Story(enterDialogueModeEvent.inkFile.text);
            Conversing = true;

            DisplayFirstLine();
        }

        private void ContinueDialogue(int index)
        {

            if (!Conversing) return;

            if (printing)
            {
                SkipDialogue();
            }
            else
            {
                if (currentStory.currentChoices.Count >= index && currentStory.currentChoices.Count != 0) currentStory.ChooseChoiceIndex(index);
                
                if (currentStory.canContinue)
                {
                    StopAllCoroutines();
                    StartCoroutine(HandleNextLine(currentStory.Continue()));
                }
                else
                {
                    ExitDialogueMode();
                }
            }
        }
        
        /// <summary>
        /// Triggers Continue Dialogue Logic through OnChooseNextDialogueLine event
        /// </summary>
        /// <param name="event"></param>
        private void ContinueDialogue(ChooseNextDialogueLineEvent @event)
        {

            if (!Conversing) return;

            if (printing)
            {
                SkipDialogue();
            }
            else
            {   
                if (currentStory.currentChoices.Count >= @event.ChoiceIndex && currentStory.currentChoices.Count != 0) currentStory.ChooseChoiceIndex(@event.ChoiceIndex);
                
                if (currentStory.canContinue)
                {
                    StopAllCoroutines();
                    StartCoroutine(HandleNextLine(currentStory.Continue()));
                }
                else
                {
                    ExitDialogueMode();
                }
            }
        }

        private void DisableAllChoices()
        {
            foreach (var choicesTextBox in choicesTextBoxes)
            {
                choicesTextBox.transform.parent.gameObject.SetActive(false);
            }
        }

        private void DisplayFirstLine()
        {
            if (currentStory.canContinue)
            {
                StopAllCoroutines();
                StartCoroutine(HandleNextLine(currentStory.Continue()));
            }
            else
            {
                ExitDialogueMode();
            }
        }

        private void SkipDialogue()
        {
            dialogueText.text = nextLine;
            StopAllCoroutines();
            FinishLine();
            Debug.Log(nextLine);
            
        }

        private void DisplayChoices()
        {
            var choices = currentStory.currentChoices;
            Debug.Log(choices.Count);
            
            if (choices.Count == 0)
            {
                choicesTextBoxes[0].transform.parent.gameObject.SetActive(true);
                choicesTextBoxes[0].text = "go on...";
            }
            else
            {
                for (int i = 0; i < choices.Count  ; i++)
                {
                    choicesTextBoxes[i].transform.parent.gameObject.SetActive(true);
                    choicesTextBoxes[i].text = choices[i].text; 
                } 
            }
        }

        private void HandleAuthor()
        {
            var tags = currentStory.currentTags;
            Debug.Log(tags.Count);

            if (tags.Count > 0)
            {
                nameText.text = tags[0].Split(':', 2)[1];
            }
        }

        private void HandlePortrait()
        {
            var tags = currentStory.currentTags;
            if (tags.Count <= 0) return;

            var s = tags[0];
            var s1 = s.Split(':', 2)[1].Trim();

            Debug.Log(s1);

            if (imagesByNames.TryGetValue(s1, out var byName))
            {
                sprite.GetComponent<Image>().sprite = byName;
            }
        }

        private void ExitDialogueMode()
        {
            InputControlManager.Instance.ChangeControls(ControlTypes.Overworld);
            dialogueCanvas.SetActive(false);
            Conversing = false;
            dialogueText.Disable();
            DisableAllChoices();
        }

        private IEnumerator HandleNextLine(string line)
        {
            nextLine = line;
            
            printing = true;
            HandleAuthor();
            HandlePortrait();
            dialogueText.text = "";
            
            foreach (char letter in line)
            {
                eventInstance.start();

                // AudioPlayer.PlayOneShot2D(AudioEventType.SFX_UI_Talking);
                dialogueText.text += letter;
                yield return new WaitForSeconds(WaitTimer);
            }

            yield return new WaitForSeconds(SentenceWaitTimer);

            FinishLine();
            Debug.Log(line);
        }

        private void FinishLine()
        {
            printing = false;
            nextLine = "";
            EventManager.RaiseEvent(new NextLineEvent(currentStory.currentChoices));
        }
    }
}