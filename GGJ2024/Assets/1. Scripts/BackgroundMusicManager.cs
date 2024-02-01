using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using VDFramework;

public class BackgroundMusicManager : BetterMonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        AudioPlayer.SetEmitterEvent(EmitterType.BackgroundMusic, AudioEventType.Music_Hell);
        AudioPlayer.PlayEmitter(EmitterType.BackgroundMusic);
    }
}
