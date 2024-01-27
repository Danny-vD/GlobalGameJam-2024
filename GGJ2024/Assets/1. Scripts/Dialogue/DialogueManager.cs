using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private bool printing;
        public bool Conversing { get; private set; }

        private Queue<string> _lines;
        private const float WaitTimer = 0.05f;
        private const float SentenceWaitTimer = 0.5f;
        private const float EndWaitTimer = 1;

        private Story currentStory;
        public Animator animator;

        [Header("Regular UI")] [SerializeField]
        public TextMeshProUGUI remark;

        [Header("Dialogue UI")] [SerializeField]
        public GameObject TextPanel;

        [SerializeField] public TextMeshProUGUI nameText;
        [SerializeField] public TextMeshProUGUI dialogueText;
        [SerializeField] public GameObject ChoicesPanel;

        private TextMeshProUGUI[] choicesTextBoxes;

        // Start is called before the first frame update
        private void Start()
        {
            dialogueText.enabled = false;
            printing = false;
            Conversing = false;
            remark.text = "";
            choicesTextBoxes = ChoicesPanel.GetComponentsInChildren<TextMeshProUGUI>();
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
            currentStory = new Story(onEnterDialogueMode.inkFile.text);
            Conversing = true;
            dialogueText.enabled = true;
            TextPanel.SetActive(true);

            DisplayFirstLine();
        }

        private void ContinueDialogue(OnChooseNextDialogueLine nextLine)
        {
            var chosenIndex = nextLine.choiceIndex;
            
            if (currentStory.currentChoices.Count != 0)
            {
                if (chosenIndex > currentStory.currentChoices.Count)
                {
                    chosenIndex %= currentStory.currentChoices.Count;
                }

                currentStory.ChooseChoiceIndex(chosenIndex);
            }
            else
            {
                currentStory.Continue();
            }
        }

        private void DisplayFirstLine()
        {
            if (currentStory.canContinue)
            {
                //Debug.Log(currentStory.Continue());
                StopAllCoroutines();
                StartCoroutine(HandleNextLine(currentStory.Continue()));
                DisplayChoices();
                HandleAuthor();
            }
            else
            {
                Debug.Log("THE TEXT SHOULD BE FINISHED");
                ExitDialogueMode();
            }
        }

        private void DisplayChoices()
        {
            // if (currentStory.currentChoices.Count == 0) return;
            var choices = currentStory.currentChoices;
            var index = 0;
            foreach (var choice in choices)
            {
                choicesTextBoxes[index].text = index + ". " + choice.text;
                index++;
            }

            for (var a = index; a < choicesTextBoxes.Length; a++)
            {
                choicesTextBoxes[a].text = "";
            }
        }

        private void DisplayNextLine(object sender, int choiceIndex)
        {
            if (currentStory.canContinue)
            {
                //Debug.Log(currentStory.Continue());
                StopAllCoroutines();
                StartCoroutine(HandleNextLine(currentStory.Continue()));
                DisplayChoices();
                HandleAuthor();
            }
            else
            {
                ExitDialogueMode();
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
            Conversing = false;
            TextPanel.SetActive(false);
            dialogueText.text = "";
        }

        private IEnumerator HandleNextLine(string line)
        {
            foreach (var letter in line.ToCharArray())
            {
                Debug.Log(dialogueText.text);
                dialogueText.text += letter;
                yield return new WaitForSeconds(WaitTimer);
            }

            yield return new WaitForSeconds(SentenceWaitTimer);

            Debug.Log(line);
        }
        // Start is called before the first frame update

        private void DisplayNextSentence(string dialogueLine)
        {
            Debug.Log(" TYPING LINE: ");
            StopAllCoroutines();
            StartCoroutine(TypeLine(dialogueLine));
        }

        private IEnumerator TypeText(string[] lines)
        {
            foreach (var line in lines)
            {
                Debug.Log("SHOULD BE PRINTING");
                remark.text = "";
                foreach (var letter in line.ToCharArray())
                {
                    remark.text += letter;
                    yield return new WaitForSeconds(WaitTimer);
                }

                yield return new WaitForSeconds(SentenceWaitTimer);
            }

            EndDialogue();
        }

        private IEnumerator TypeLine(string line)
        {
            dialogueText.text = "";

            foreach (char letter in line)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(WaitTimer); // How long it will take for it to the new step
            }
            
            yield return new WaitForSeconds(EndWaitTimer);
            EndDialogue();
        }

        private void EndDialogue()
        {
            // animator.SetBool("IsOpen", false);
            Debug.Log("End of Conversation");
            remark.text = "";
            dialogueText.enabled = false;
            printing = false;
        }
    }
}