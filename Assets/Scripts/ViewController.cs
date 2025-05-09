using UnityEngine;
using UnityEngine.InputSystem;

public class ViewController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    // private Vector2 rotationInput;

    void Update()
    {
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