using UnityEngine;
using UnityEngine.EventSystems;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;
    [SerializeField] ButtonSolve _buttonSolve;

    // 対象CanvasにアタッチされたGraphicRaycasterをInspectorからセット
    public EventSystem eventSystem;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Texture2D _cursorDefaultTexture;
    [SerializeField] Texture2D _cursorHoverTexture;

    CubeState _cubeState;
    CubeModel _cubeModel;
    bool _isDisableInteraction = false;
    public bool IsDisableInteraction
    {
        get => _isDisableInteraction;
        set => _isDisableInteraction = value;
    }

    bool _isOnGrabber = false;
    bool _isPanelOpen = false;

    void Start()
    {
        _cubeModel = new CubeModel();
        _cubeState = new CubeState();
        _buttonSolve.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_isDisableInteraction) return;
        Pointing();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rotate(0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Rotate(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Rotate(2, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Rotate(3, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Rotate(4, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Rotate(5, 0);
        }
    }

    public void OnClickScrambleButton()
    {
        string scramble = _cubeModel.GenerateRandomScramble(10);
        Debug.Log("scramble: " + scramble);
        RotateSequence(scramble);
    }

    async public void OnClickSolveButton()
    {
        // var token = new CancellationToken();
        // AwaitableCancel(token);
        _isDisableInteraction = true;
        var cubeSearch = new CubeSearch(_cubeModel);
        var solution = await cubeSearch.StartSearch(_cubeState, 23, 1f);
        Debug.Log("solution: " + solution);
        _buttonSolve.FadeOut();
        RotateSequence(solution);
    }

    void Pointing()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     return;
        // }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.gameObject.CompareTag("Grabber")) return;
            GameObject currentHit = hit.collider.gameObject;
            Cursor.SetCursor(_cursorHoverTexture, Vector2.zero, CursorMode.Auto);

            if (currentHit.GetComponent<GrabberController>().CurrentState == GrabberController.State.Base)
            {
                OnGrabber(currentHit.GetComponent<GrabberController>());
            }

            foreach (var gc in _grabberControllers)
            {
                if (gc.CurrentState == GrabberController.State.PreRotated && gc.gameObject != currentHit)
                {
                    OffGrabber(gc);
                }
            }

        }
        else
        {
            foreach (var gc in _grabberControllers)
            {
                if (gc.CurrentState == GrabberController.State.PreRotated)
                {
                    OffGrabber(gc);
                }
            }
        }
    }

    public void ClickedGrabber(GrabberController grabberController)
    {
        if (_isDisableInteraction) return;
        var index = System.Array.IndexOf(_grabberControllers, grabberController);
        Rotate(index, 0);
        if (!_buttonSolve.gameObject.activeInHierarchy)
        {
            _buttonSolve.FadeIn();
        }
    }

    public void OnGrabber(GrabberController grabberController)
    {
        if (_isDisableInteraction) return;
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.Rotating || gc.CurrentState == GrabberController.State.PreRotated)
            {
                return;
            }
        }
        if (grabberController != null)
        {
            grabberController.PreRotateFace();

        }
        if (!_isOnGrabber) PlayAudio(1);
        _isOnGrabber = true;
    }

    public void OffGrabber(GrabberController grabberController)
    {
        if (_isDisableInteraction) return;
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }
        if (grabberController != null)
            grabberController.ResetRotation();
        _isOnGrabber = false;
        Cursor.SetCursor(_cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
    }

    void Rotate(int index, int rotation)
    {
        foreach (var grabberController in _grabberControllers)
        {
            if (grabberController.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }
        PlayAudio(0);
        _grabberControllers[index].RotateFace(rotation);
        ChangeState(index, rotation);
    }

    async void RotateSequence(string sequence)
    {
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.PreRotated)
            {
                gc.ResetRotation();
                _isDisableInteraction = true;
                await Awaitable.WaitForSecondsAsync(0.1f); // Wait for the reset to complete
            }
        }

        if (string.IsNullOrEmpty(sequence)) return;
        _isDisableInteraction = true;
        var moveNames = sequence.Split(' ');
        foreach (var moveName in moveNames)
        {
            int index = _cubeModel.MoveNames.IndexOf(moveName);
            if (index < 0) continue; // Skip if moveName is not valid
            int rotation = index % 3; // Determine rotation based on index
            int grabberIndex = index / 3; // Determine grabber index
            Rotate(grabberIndex, rotation);
            await Awaitable.WaitForSecondsAsync(0.2f); // Wait for the rotation to complete
        }
        _isDisableInteraction = false;
    }

    void ChangeState(int index, int rotation)
    {
        string moveName = _cubeModel.MoveNames[index * 3 + rotation];
        _cubeState = _cubeModel.ScrambleToState(_cubeState, moveName);
    }

    void PlayAudio(int index)
    {
        if (_audioSource == null) return;
        if (index == 1)
        {
            _audioSource.pitch = 0.5f;
            _audioSource.volume = 0.2f;
        }
        else
        {
            _audioSource.pitch = 1.0f;
            _audioSource.volume = 1.0f;
        }
        _audioSource.Play();
    }
}
