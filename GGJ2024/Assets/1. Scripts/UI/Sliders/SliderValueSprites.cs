using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;

public class SliderValueSprites : BetterMonoBehaviour
{
    [SerializeField] private bool useNormalizedValue;

    [SerializeField] private Slider slider;

    [SerializeField] private SerializableDictionary<float, Sprite> sliderValueSprites;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateSprite);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateSprite);
    }

    private void UpdateSprite(float sliderValue)
    {
        if (useNormalizedValue) sliderValue /= slider.maxValue;

        UpdateSprite(GetLastSpriteWithConditionsMet(sliderValue));
    }

    private void UpdateSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("No valid sprite found");
            return;
        }

        image.sprite = sprite;
    }

    private Sprite GetLastSpriteWithConditionsMet(float value)
    {
        Sprite sprite = null;
        var highestValueMet = float.MinValue;

        foreach (var pair in sliderValueSprites)
        {
            var threshold = pair.Key;

            if (highestValueMet < threshold && value >= threshold)
            {
                sprite = pair.Value;
                highestValueMet = threshold;
            }
        }

        return sprite;
    }
}