using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;
    CubeState _cubeState;
    CubeModel _cubeModel;
    bool _isSequenceRunning = false;

    void Start()
    {
        _cubeModel = new CubeModel();
        _cubeState = new CubeState();
    }

    void Update()
    {
        if (_isSequenceRunning) return;
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
        var cubeSearch = new CubeSearch(_cubeModel);
        var solution = await cubeSearch.StartSearch(_cubeState, 23, 1f);
        Debug.Log("solution: " + solution);
        RotateSequence(solution);
    }

    void Pointing()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.gameObject.CompareTag("Grabber")) return;
            GameObject currentHit = hit.collider.gameObject;

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
        if (_isSequenceRunning) return;
        var index = System.Array.IndexOf(_grabberControllers, grabberController);
        Rotate(index, 0);
    }

    public void OnGrabber(GrabberController grabberController)
    {
        if (_isSequenceRunning) return;
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.Rotating || gc.CurrentState == GrabberController.State.PreRotated)
            {
                return;
            }
        }
        if (grabberController != null)
            grabberController.PreRotateFace();
    }

    public void OffGrabber(GrabberController grabberController)
    {
        if (_isSequenceRunning) return;
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }
        if (grabberController != null)
            grabberController.ResetRotation();
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
                _isSequenceRunning = true;
                await Awaitable.WaitForSecondsAsync(0.1f); // Wait for the reset to complete
            }
        }

        if (string.IsNullOrEmpty(sequence)) return;
        _isSequenceRunning = true;
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
        _isSequenceRunning = false;
    }

    void ChangeState(int index, int rotation)
    {
        string moveName = _cubeModel.MoveNames[index * 3 + rotation];
        _cubeState = _cubeModel.ScrambleToState(_cubeState, moveName);
    }
}
