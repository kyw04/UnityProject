using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 10.0f;

    private Vector3 moveDirection;
    
    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
    
    void OnMove(InputValue value)
    {
        Vector2 inputVec2 = value.Get<Vector2>();

        if(inputVec2 != null) {
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
        }
    }
}
