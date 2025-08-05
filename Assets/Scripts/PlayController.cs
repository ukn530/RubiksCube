using UnityEngine;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;
    [SerializeField] ButtonSolve _buttonSolvePC;
    [SerializeField] ButtonSolve _buttonSolveSP;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] Texture2D _cursorDefaultTexture;
    [SerializeField] Texture2D _cursorHoverTexture;
    [SerializeField] ViewController _viewController;
    [SerializeField] CameraController _cameraController;

    CubeState _cubeState;
    CubeModel _cubeModel;
    bool _isDisableInteraction = false;
    bool _isPC;

    public bool IsDisableInteraction
    {
        get => _isDisableInteraction;
        set => _isDisableInteraction = value;
    }

    void Start()
    {
        //test commit
        _cubeModel = new CubeModel();
        _cubeState = new CubeState();
        _buttonSolvePC.gameObject.SetActive(false);
        _buttonSolveSP.gameObject.SetActive(false);
        Cursor.SetCursor(_cursorDefaultTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
        CheckScreenRatio();
    }

    void Update()
    {
        CheckScreenRatio();
        if (_isDisableInteraction) return;
        if (_isPC) Pointing();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rotate(0, 0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Rotate(1, 0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Rotate(2, 0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Rotate(3, 0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Rotate(4, 0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Rotate(5, 0, true);
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
        _isDisableInteraction = true;
        var cubeSearch = new CubeSearch(_cubeModel);
        var solution = await cubeSearch.StartSearch(_cubeState, 23, 1f);
        Debug.Log("solution: " + solution);
        _buttonSolvePC.FadeOut();
        _buttonSolveSP.FadeOut();
        RotateSequence(solution);
        _viewController.Rotate(1);
        _cameraController.ZoomIn(2);
    }

    void Pointing()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.gameObject.CompareTag("Grabber")) return;
            GameObject currentHit = hit.collider.gameObject;
            Cursor.SetCursor(_cursorHoverTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);


            Transform faceTransform = hit.transform;

            // ヒットした面の中心位置をスクリーン座標に変換
            Vector3 faceCenterScreenPos = Camera.main.WorldToScreenPoint(faceTransform.position);

            // ヒットした点をスクリーン座標に変換
            Vector3 hitPointScreenPos = Camera.main.WorldToScreenPoint(hit.point);

            int preRotatedIndex = hitPointScreenPos.x < faceCenterScreenPos.x ? -1 : 1;
            int currentPreRotatedIndex = CurrentPreRotatedIndex(currentHit.GetComponent<GrabberController>());
            if (currentPreRotatedIndex != preRotatedIndex)
            {
                OnGrabber(currentHit.GetComponent<GrabberController>(), preRotatedIndex > 0);
            }

            foreach (var gc in _grabberControllers)
            {
                if ((gc.CurrentState == GrabberController.State.PreRotatedL || gc.CurrentState == GrabberController.State.PreRotatedR) && gc.gameObject != currentHit)
                {
                    OffGrabber(gc);
                }
            }
        }
        else
        {
            foreach (var gc in _grabberControllers)
            {
                if (gc.CurrentState == GrabberController.State.PreRotatedL || gc.CurrentState == GrabberController.State.PreRotatedR)
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
        int currentPreRotatedIntex = CurrentPreRotatedIndex(grabberController);
        Rotate(index, 0, currentPreRotatedIntex > 0);
        if (!_buttonSolvePC.gameObject.activeInHierarchy)
        {
            _buttonSolvePC.FadeIn();
        }
        if (!_buttonSolveSP.gameObject.activeInHierarchy)
        {
            _buttonSolveSP.FadeIn();
        }
    }

    public void OnGrabber(GrabberController grabberController, bool isRight)
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
        {
            grabberController.PreRotateFace(isRight);
        }
        PlayAudio(1);
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
        Cursor.SetCursor(_cursorDefaultTexture, Vector2.one * _cursorDefaultTexture.width / 2, CursorMode.ForceSoftware);
    }

    void Rotate(int index, int rotation, bool isRight)
    {
        foreach (var grabberController in _grabberControllers)
        {
            if (grabberController.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }
        PlayAudio(0);
        _grabberControllers[index].RotateFace(rotation, isRight);
        ChangeState(index, rotation, isRight);
    }

    async void RotateSequence(string sequence)
    {
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.PreRotatedL || gc.CurrentState == GrabberController.State.PreRotatedR)
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
            Rotate(grabberIndex, rotation, true);
            await Awaitable.WaitForSecondsAsync(0.2f); // Wait for the rotation to complete
        }
        _isDisableInteraction = false;
    }

    void ChangeState(int index, int rotation, bool isRight)
    {
        if (rotation == 0 && !isRight) rotation = 2; // Adjust rotation for left turns
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
            _audioSource.pitch = 0.75f;
            _audioSource.volume = 1.0f;
        }
        _audioSource.Play();
    }

    int CurrentPreRotatedIndex(GrabberController grabberController)
    {
        if (grabberController.CurrentState == GrabberController.State.PreRotatedR)
        {
            return 1;
        }
        else if (grabberController.CurrentState == GrabberController.State.PreRotatedL)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    void CheckScreenRatio()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio < 1 && _isPC)
        {
            _isPC = false; // Update the state to indicate that we are now in a mobile view
        }
        else if (ratio > 1 && !_isPC)
        {
            _isPC = true; // Update the state to indicate that we are now in a PC view
        }
    }
}
