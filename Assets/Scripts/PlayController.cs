using UnityEngine;
using UnityEngine.InputSystem;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;

    void Start()
    {
        CubeModel state = new CubeModel();
        string scramble = "L F U2";
        state.ScrambleToState(scramble);
        var solution = state.StartSearch(4);
        Debug.Log("solution: " + solution);
    }

    void Update()
    {
        Pointing();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rotate90(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Rotate90(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Rotate90(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Rotate90(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Rotate90(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Rotate90(5);
        }
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
        foreach (var gc in _grabberControllers)
        {
            if (gc.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }
        if (grabberController != null)
            grabberController.RotateFace();
    }

    public void OnGrabber(GrabberController grabberController)
    {
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

    void Rotate90(int index)
    {
        foreach (var grabberController in _grabberControllers)
        {
            if (grabberController.CurrentState == GrabberController.State.Rotating)
            {
                return;
            }
        }

        _grabberControllers[index].RotateFace();
    }
}
