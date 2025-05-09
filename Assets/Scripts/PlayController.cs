using UnityEngine;
using UnityEngine.InputSystem;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;
    [SerializeField] ViewController _viewController;

    void OnEnable()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            keyboard.onTextInput += OnTextInput;
        }
    }

    void OnDisable()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            keyboard.onTextInput -= OnTextInput;
        }
    }

    void OnTextInput(char ch)
    {
        int index;
        switch (ch)
        {
            case '1':
                index = 0;
                break;
            case '2':
                index = 1;
                break;
            case '3':
                index = 2;
                break;
            case '4':
                index = 3;
                break;
            case '5':
                index = 4;
                break;
            case '6':
                index = 5;
                break;
            case '7':
                index = 6;
                break;
            case '8':
                index = 7;
                break;
            case '9':
                index = 8;
                break;
            default:
                Debug.LogError("Invalid input: " + ch);
                return;
        }
        foreach (var grabberController in _grabberControllers)
        {
            if (grabberController.IsRotating)
            {
                return;
            }
        }
        _grabberControllers[index].RotateFace();
    }
}
