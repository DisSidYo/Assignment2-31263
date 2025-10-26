using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator moveAnimator;

    private Vector3 currentDirection = Vector3.zero;
    private bool isTweening = false;
    private TweenerNormal tweener;
    private float moveDistance = 1f; // Distance to move in each tween
    private float tweenDuration = 0.5f; // Duration of each tween

    private Vector3 movement;
    private float movementSqrMagnitude;

    private bool IsMovementZero => movement == Vector3.zero;
    private float moveSpeed = 5f;

    // [SerializeField] private AsyncLoader asyncLoader;

    // private float ModifiedMoveSpeed => SpeedManager.SpeedModifier * movementSqrMagnitude;
    private int[,] levelMap = new int[,]
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0}
    };   
    void Start()
    {
        moveAnimator.Play("Idle_Right");
        tweener = GetComponent<TweenerNormal>();
    }

    // Update is called once per frame
    void Update () {
        // if(asyncLoader)
        // {
        //     asyncLoader.pos = transform.position;
        // }


        GetMovementInput();
        // CharacterRotation();
        CharacterMovement();
        WalkingAnimation();
	}


    void GetMovementInput()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement = Vector3.ClampMagnitude(movement, 1.0f);
        

       
    }


    // void CharacterRotation() {
    //     if (!IsMovementZero) {
    //         transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
    //     }
    // }
    void CharacterMovement() {
        if (!isTweening)
        {
            if (Input.GetKey(KeyCode.W) && currentDirection != Vector3.up)
            {
                currentDirection = Vector3.up;
                StartNewTween();
                moveAnimator.Play("Pac_Stu_Up");
            }
            else if (Input.GetKey(KeyCode.S) && currentDirection != Vector3.down)
            {
                currentDirection = Vector3.down;
                StartNewTween();
                moveAnimator.Play("Pac_Stu_Down");
            }
            else if (Input.GetKey(KeyCode.A) && currentDirection != Vector3.left)
            {
                currentDirection = Vector3.left;
                StartNewTween();
                moveAnimator.Play("Pac_Stu_Left");
            }
            else if (Input.GetKey(KeyCode.D) && currentDirection != Vector3.right)
            {
                currentDirection = Vector3.right;
                StartNewTween();
                moveAnimator.Play("Pac_Stu_Right");
            }
        }
    }

    private void StartNewTween()
    {
        Vector3 targetPos = transform.position + (currentDirection * moveDistance);
        tweener.AddTween(transform, transform.position, targetPos, tweenDuration);
        isTweening = true;
    }

    void WalkingAnimation()
    {

        // moveAnimator.SetFloat("MoveSpeed", ModifiedMoveSpeed);
        
        
    }
}
