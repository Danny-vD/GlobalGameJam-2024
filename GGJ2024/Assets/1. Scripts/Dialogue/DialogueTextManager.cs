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
		[SerializeField]
		private TMP_Text dialogueText;

		[SerializeField]
		private TMP_Text authorText;

		[SerializeField]
		private Image sprite;

		[SerializeField]
		private float waitTimer = 0.05f;

		[SerializeField]
		private float sentenceWaitTimer = 0.5f;

		[SerializeField]
		private SerializableDictionary<string, Sprite> imagesByNames;

		[SerializeField] 
		private SerializableDictionary<string, float> specialCharacterTimes;

		private bool printing;
		private object eventInstance;
		private string nextLine;

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
			nextLine = line;

			printing = true;
			HandlePortrait(author);
			HandleAuthor(author);

			dialogueText.text = "";

			dialogueText.maxVisibleCharacters = 0;
			dialogueText.text                 = line;

			foreach (char letter in line)
			{
				// eventInstance.start();
				
				dialogueText.maxVisibleCharacters++;

				if (specialCharacterTimes.TryGetValue(letter.ToString(), out var specialTime))
				{
					yield return new WaitForSeconds(specialTime);
				}
				else
				{
					yield return new WaitForSeconds(waitTimer);
				}
			}

			yield return new WaitForSeconds(sentenceWaitTimer);

			FinishLine();
		}

		private void FinishLine()
		{
			printing = false;
			nextLine = "";
			EventManager.RaiseEvent(new FinishedLineEvent());
		}

		private void HandlePortrait(string author)
		{
			if (imagesByNames.TryGetValue(author, out var byName))
			{
				sprite.GetComponent<Image>().sprite = byName;
			}
		}

		private void HandleAuthor(string author)
		{
			authorText.text = author;
		}
	}
}