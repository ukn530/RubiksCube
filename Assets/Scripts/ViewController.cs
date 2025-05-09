using UnityEngine;
using UnityEngine.InputSystem;

public class ViewController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    bool _enabled = true;
    public bool Enabled => _enabled;

    void Update()
    {
        if (!_enabled)
            return;

        if (Mouse.current.leftButton.isPressed)
        {
            var rotationInput = Mouse.current.delta.ReadValue();
            float rotationX = rotationInput.y * rotationSpeed * Time.deltaTime;
            float rotationY = -rotationInput.x * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.right, rotationX, Space.World);
            transform.Rotate(Vector3.up, rotationY, Space.World);
        }
    }
}