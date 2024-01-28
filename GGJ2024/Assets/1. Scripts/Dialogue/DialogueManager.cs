using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
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

        [SerializeField] public TextMeshProUGUI nameText;
        [SerializeField] public TextMeshProUGUI dialogueText;
        [SerializeField] public GameObject ChoicesPanel;

        private TMP_Text[] choicesTextBoxes;

        private void Start()
        {
            
            
            dialogueText.Disable();
            printing = false;
            Conversing = false;
            choicesTextBoxes = ChoicesPanel.GetComponentsInChildren<TMP_Text>();

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

                    currentStory.ChooseChoiceIndex(index);
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
            var index = 0;
            foreach (var choice in choices)
            {
                
                choicesTextBoxes[index].text = index + ". " + choice.text;
                index++;
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

        private void ExitDialogueMode()
        {
            dialogueCanvas.SetActive(false);
            Conversing = false;
            dialogueText.Disable();
        }

        private IEnumerator HandleNextLine(string line)
        {
            printing = true;
            HandleAuthor();
            dialogueText.text = "";

            foreach (var letter in line.ToCharArray())
            {
                Debug.Log(dialogueText.text);
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