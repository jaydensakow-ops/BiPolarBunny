using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D Rigidbody2D;
    public Animator Animator;
    private Vector2 moveDirection;

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    public void FixedUpdate()
    {
        FaceCorrectDirection();
        Animate(); 
        Move();
    }

    private void Move()
    {
        Rigidbody2D.linearVelocity = moveDirection * GameParameters.PlayerSpeed;
    }

    private void Animate()
    {
        Animator.SetFloat("horizontal", Mathf.Abs(moveDirection.x));
        Animator.SetFloat("vertical", Mathf.Abs(moveDirection.y));
    }

    private void FaceCorrectDirection()
    {
        if (IsNotFacingCorrectDirection())
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private bool IsNotFacingCorrectDirection()
    {
        if (moveDirection.x > 0 && transform.localScale.x < 0 || moveDirection.x < 0 && transform.localScale.x > 0)
        {
            return true;
        }
        return false;
    }

}
