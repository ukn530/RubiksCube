using UnityEngine;
using UnityEngine.InputSystem;

public class PlayController : MonoBehaviour
{
    [SerializeField] GrabberController[] _grabberControllers;
    [SerializeField] ViewController _viewController;
    GameObject _lastHit = null;

    bool _isPointerOnCube = false;

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
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            GameObject currentHit = hit.collider.gameObject;

            if (currentHit.GetComponent<GrabberController>().CurrentState == GrabberController.State.Base)
            {
                OnGrabber(currentHit.GetComponent<GrabberController>());
            }
            if (currentHit != _lastHit)
            {
                if (_lastHit != null)
                {
                    OffGrabber(_lastHit.GetComponent<GrabberController>());
                    // _lastHitから離れたときの処理
                }
            }
            _lastHit = currentHit;

            // if (currentHit != _lastHit)
            // {
            //     if (_lastHit != null)
            //     {
            //         _isPointerOnCube = false;
            //         Debug.Log("マウスが " + _lastHit.name + " から離れました。");
            //         OffGrabber(_lastHit.GetComponent<GrabberController>());
            //         // _lastHitから離れたときの処理
            //     }
            //     _isPointerOnCube = true;
            //     Debug.Log("マウスが " + currentHit.name + " に入りました。");
            //     OnGrabber(currentHit.GetComponent<GrabberController>());
            //     // currentHitに入ったときの処理
            //     _lastHit = currentHit;
            // }
            // Debug.Log("マウスが " + currentHit.name + " にいます。");
        }
        else if (_lastHit != null)
        {
            OffGrabber(_lastHit.GetComponent<GrabberController>());
            _lastHit = null;
            // _lastHitから離れたときの処理
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
        Debug.Log("On Grabber:" + grabberController.name);
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
        Debug.Log("Off Grabber:" + grabberController.name);

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
