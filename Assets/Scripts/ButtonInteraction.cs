using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] AudioClip _audioClip;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Texture2D _cursorDefaultTexture;
    [SerializeField] Texture2D _cursorHoverTexture;

    // This method is called when the pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_audioSource != null && _audioClip != null)
        {
            _audioSource.pitch = 1.0f;
            _audioSource.volume = 0.3f;
            _audioSource.PlayOneShot(_audioClip);
        }
        Cursor.SetCursor(_cursorHoverTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_audioSource != null && _audioClip != null)
        {
            _audioSource.pitch = 0.8f;
            _audioSource.volume = 0.5f;
            _audioSource.PlayOneShot(_audioClip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(_cursorDefaultTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
    }
}
