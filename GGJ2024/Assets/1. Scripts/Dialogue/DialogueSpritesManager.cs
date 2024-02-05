using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework;
using VDFramework.Singleton;

namespace Dialogue
{
    public class DialogueSpritesManager : Singleton<DialogueSpritesManager>
    {
        [SerializeField] public SerializableDictionary<string, Sprite> sprites;
    }
}