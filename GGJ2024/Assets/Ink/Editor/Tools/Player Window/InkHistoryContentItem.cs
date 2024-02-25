using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace Ink.UnityIntegration.Debugging
{
    [Serializable]
    public class InkHistoryContentItem
    {
        public enum ContentType
        {
            PresentedContent,
            ChooseChoice,
            PresentedChoice,
            EvaluateFunction,
            CompleteEvaluateFunction,
            ChoosePathString,
            Warning,
            Error,
            DebugNote
        }

        public string content;
        public List<string> tags;

        public ContentType contentType;

        // Creating a datetime from a long is slightly expensive (it can happen many times in a frame). To fix this we cache the result once converted. 
        [SerializeField] private JsonDateTime _serializableTime;
        [NonSerialized] private DateTime _time;
        [NonSerialized] private bool hasDeserializedTime;

        private InkHistoryContentItem(string text, ContentType contentType)
        {
            content = text;
            this.contentType = contentType;
            time = DateTime.Now;
        }

        private InkHistoryContentItem(string text, List<string> tags, ContentType contentType)
        {
            content = text;
            this.tags = tags;
            this.contentType = contentType;
            time = DateTime.Now;
        }

        public DateTime time
        {
            get
            {
                if (!hasDeserializedTime)
                {
                    _time = _serializableTime;
                    hasDeserializedTime = true;
                }

                return _time;
            }
            private set
            {
                _time = value;
                _serializableTime = value;
            }
        }

        public static InkHistoryContentItem CreateForContent(string choiceText, List<string> tags)
        {
            return new InkHistoryContentItem(choiceText, tags, ContentType.PresentedContent);
        }

        public static InkHistoryContentItem CreateForPresentChoice(Choice choice)
        {
            return new InkHistoryContentItem(choice.text.Trim(), choice.tags, ContentType.PresentedChoice);
        }

        public static InkHistoryContentItem CreateForMakeChoice(Choice choice)
        {
            return new InkHistoryContentItem(choice.text.Trim(), choice.tags, ContentType.ChooseChoice);
        }

        public static InkHistoryContentItem CreateForEvaluateFunction(string functionInfoText)
        {
            return new InkHistoryContentItem(functionInfoText, ContentType.EvaluateFunction);
        }

        public static InkHistoryContentItem CreateForCompleteEvaluateFunction(string functionInfoText)
        {
            return new InkHistoryContentItem(functionInfoText, ContentType.CompleteEvaluateFunction);
        }

        public static InkHistoryContentItem CreateForChoosePathString(string choosePathStringText)
        {
            return new InkHistoryContentItem(choosePathStringText, ContentType.ChoosePathString);
        }

        public static InkHistoryContentItem CreateForWarning(string warningText)
        {
            return new InkHistoryContentItem(warningText, ContentType.Warning);
        }

        public static InkHistoryContentItem CreateForError(string errorText)
        {
            return new InkHistoryContentItem(errorText, ContentType.Error);
        }

        public static InkHistoryContentItem CreateForDebugNote(string noteText)
        {
            return new InkHistoryContentItem(noteText, ContentType.DebugNote);
        }

        private struct JsonDateTime
        {
            public long value;

            public static implicit operator DateTime(JsonDateTime jdt)
            {
                return DateTime.FromFileTime(jdt.value);
            }

            public static implicit operator JsonDateTime(DateTime dt)
            {
                var jdt = new JsonDateTime();
                jdt.value = dt.ToFileTime();
                return jdt;
            }
        }
    }
}