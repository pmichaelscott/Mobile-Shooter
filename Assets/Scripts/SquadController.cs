using UnityEngine;
using UnityEngine.InputSystem;

public class SquadController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX =  7f;

    private InputAction horizontalInputAction;

    void OnEnable()
    {
        horizontalInputAction = InputSystem.actions.FindAction("Move");
        horizontalInputAction.Enable();
    }


    void OnDisable()
    {      
        horizontalInputAction.Disable();
    }

    void Update()
    {
        float input = horizontalInputAction.ReadValue<Vector2>().x;
        Vector3 pos = transform.position;
        pos.x += input * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}
