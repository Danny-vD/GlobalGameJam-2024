using FMODUtilityPackage.Core;
using UnityEngine;
using UnityEngine.UI;

public class SetVolumeStart : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Slider>().value = AudioManager.Instance.GetMasterVolume();
    }
}