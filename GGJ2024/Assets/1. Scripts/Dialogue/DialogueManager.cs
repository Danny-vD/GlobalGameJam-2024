using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using Ink.Runtime;
using SerializableDictionaryPackage.SerializableDictionary;
using TMPro;
using UI;
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
        public bool Conversing { get; private set; }

        private Queue<string> _lines;
        private const float WaitTimer = 0.05f;
        private const float SentenceWaitTimer = 0.5f;

        private Story currentStory;

        [SerializeField] public GameObject dialogueCanvas;

        [SerializeField] private GameObject sprite;

        [SerializeField] private SerializableDictionary<string, Sprite> imagesByNames;

        [SerializeField] public TextMeshProUGUI nameText;

        [SerializeField] public TextMeshProUGUI dialogueText;

        [SerializeField] public GameObject ChoicesPanel;

        private TMP_Text[] choicesTextBoxes;

        private EventInstance eventInstance;

        private void Start()
        {
            dialogueText.Disable();
            printing = false;
            Conversing = false;
            choicesTextBoxes = ChoicesPanel.GetComponentsInChildren<TMP_Text>(true);

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

           //  eventInstance = AudioPlayer.GetEventInstance(AudioEventType.SFX_UI_Talking);
        }

        private void OnEnable()
        {
            OnEnterDialogueMode.Listeners += EnterDialogueMode;
            OnChooseNextDialogueLine.Listeners += ContinueDialogue;
        }

        private void OnDisable()
        {
            OnEnterDialogueMode.Listeners -= EnterDialogueMode;
            OnChooseNextDialogueLine.Listeners -= ContinueDialogue;
        }

        private void EnterDialogueMode(OnEnterDialogueMode onEnterDialogueMode)
        {
            Debug.Log("DIALOGUE SHOULD SHOW");

            InputControlManager.Instance.ChangeControls(ControlTypes.Dialogue);

            dialogueCanvas.SetActive(true);
            currentStory = new Story(onEnterDialogueMode.inkFile.text);
            Conversing = true;
            dialogueText.Enable();

            DisplayFirstLine();
        }

        private void ContinueDialogue(int index)
        {
            if (Conversing)
            {
                if (currentStory.currentChoices.Count != 0)
                {
                    if (index > currentStory.currentChoices.Count)
                    {
                        Debug.LogError("OUT OF BOUNDS");
                    }
                    else
                    {
                        currentStory.ChooseChoiceIndex(index);
                    }
                    
                }   

                if (currentStory.canContinue)
                {
                    if (printing) return;

                    StopAllCoroutines();
                    StartCoroutine(HandleNextLine(currentStory.Continue()));
                }
                else
                {
                    ExitDialogueMode();
                }
            }
            else
            {
                Debug.LogError("NOT CURRENTLY CONVERSING");
            }
        }

        private void ContinueDialogue(OnChooseNextDialogueLine nextLine)
        {
            if (Conversing)
            {
                if (currentStory.currentChoices.Count != 0)
                {
                    
                    if (nextLine.choiceIndex > currentStory.currentChoices.Count)
                    {
                        Debug.LogError("OUT OF BOUNDS");
                    }

                    currentStory.ChooseChoiceIndex(nextLine.choiceIndex);
                   
                }

                if (currentStory.canContinue)
                {
                    if (printing) return;

                    StopAllCoroutines();
                    StartCoroutine(HandleNextLine(currentStory.Continue()));
                }
                else
                {
                    ExitDialogueMode();
                }
            }
            else
            {
                Debug.LogError("NOT CURRENTLY CONVERSING");
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
            printing = true;
            HandleAuthor();
            HandlePortrait();
            dialogueText.text = "";

            DisableAllChoices();
            foreach (char letter in line)
            {
                eventInstance.start();

                AudioPlayer.PlayOneShot2D(AudioEventType.SFX_UI_Talking);
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
            DisplayChoices();
        }
    }
}