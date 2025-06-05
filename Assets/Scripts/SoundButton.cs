using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] AudioSource _audioSourceUI;
    [SerializeField] AudioSource _audioSourceRubiksCube;
    [SerializeField] Image _image;
    [SerializeField] Button _button;
    [SerializeField] Sprite _spriteSoundOffJP;
    [SerializeField] Sprite _spriteSoundOffHoverJP;
    [SerializeField] Sprite _spriteSoundOffPressedJP;
    [SerializeField] Sprite _spriteSoundOnJP;
    [SerializeField] Sprite _spriteSoundOnHoverJP;
    [SerializeField] Sprite _spriteSoundOnPressedJP;


    void Start()
    {
        PlayerPrefs.SetInt("IsMuted", 0);
        var isMuted = PlayerPrefs.GetInt("IsMuted", 1) == 1;
        SetMute(isMuted);
    }

    void SetMute(bool isMuted)
    {
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        _image.sprite = isMuted ? _spriteSoundOffJP : _spriteSoundOnJP;
        var spriteState = _button.spriteState;
        spriteState.highlightedSprite = isMuted ? _spriteSoundOffHoverJP : _spriteSoundOnHoverJP;
        spriteState.pressedSprite = isMuted ? _spriteSoundOffPressedJP : _spriteSoundOnPressedJP;
        spriteState.disabledSprite = isMuted ? _spriteSoundOffJP : _spriteSoundOnJP;
        spriteState.selectedSprite = isMuted ? _spriteSoundOffJP : _spriteSoundOnJP;
        _button.spriteState = spriteState;
        _audioSourceRubiksCube.mute = isMuted;
        _audioSourceUI.mute = isMuted;
    }

    public void OnPointerClick()
    {
        var isMuted = PlayerPrefs.GetInt("IsMuted", 1) == 1;
        SetMute(!isMuted);
    }
}
