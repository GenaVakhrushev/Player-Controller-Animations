using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    bool toFinish = false;
    Transform finishingTarget;

    Vector3 rot;
    Vector3 moveVec = Vector3.zero;
    Vector3 camForward => Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
    Vector3 camRight => Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;

    bool anyInput => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

    public float MoveSpeed;
    public float BodyRotationSpeed;
    public float TagretBoteRotationSpeed;

    public Transform Model;
    public Transform TargetBone;

    public float FinishingDistance;
    public float FinishingRadius;
    public GameObject FinishingPanel;

    public GameObject Gun;
    public GameObject Sword;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(finishingTarget && Input.GetKeyUp(KeyCode.Space))
        {
            toFinish = true;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void LateUpdate()
    {
        SetMoveAnimation();

        RotateModel();

        LookToMouse();
    }

    void Move()
    {
        if (toFinish)
        {
            if (anyInput)
            {
                toFinish = false;
                return;
            }
            MoveToFinish();
        }
        else
        {
            MoveKeyboard();
        }

        transform.Translate(moveVec * MoveSpeed * Time.deltaTime, Space.World);
        transform.LookAt(-camForward + camRight + transform.position);
    }

    void MoveKeyboard()
    {
        if (!anyInput)
        {
            moveVec = Vector3.zero;
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveVec += camForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveVec -= camRight;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVec -= camForward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveVec += camRight;
        }
        moveVec.Normalize();
    }

    void MoveToFinish()
    {
        float distToTagrget = Vector3.Distance(finishingTarget.position, transform.position);
        float bodyAngleToTarget = Vector3.Angle(Model.forward, finishingTarget.transform.position - transform.position);

        if (distToTagrget <= FinishingDistance && bodyAngleToTarget < 10)
        {
            animator.SetTrigger("Finishing");
            toFinish = false;
            //ResetTarget();
            return;
        }

        if (distToTagrget > FinishingDistance)
            moveVec = (finishingTarget.position - transform.position).normalized * 2;
        else
            moveVec = Vector3.zero;
    }

    void LookToMouse()
    {
        if (toFinish)
        {
            TargetBoneLookAt(finishingTarget.position, 2);
            return;
        }

        Plane plane = new Plane(transform.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;

        plane.Raycast(ray, out dist);

        Vector3 mousePos = ray.GetPoint(dist);
        TargetBoneLookAt(mousePos, 1);
    }

    void SetMoveAnimation()
    {
        float angleToMoveVec = Vector3.Angle(transform.forward, moveVec);
        bool movingRight = Vector3.Angle(transform.right, moveVec) < 90;

        if (moveVec == Vector3.zero)
        {
            animator.SetInteger("Direction", -1);
            return;
        }

        if (angleToMoveVec < 60)
        {
            animator.SetInteger("Direction", 0);
        }
        else if(angleToMoveVec < 120 && !movingRight)
        {
            animator.SetInteger("Direction", 1);
        }
        else if (angleToMoveVec < 120 && movingRight)
        {
            animator.SetInteger("Direction", 3);
        }
        else
        {
            animator.SetInteger("Direction", 2);
        }
    }

    void RotateModel()
    {
        if (toFinish)
        {
            BodyLookAt(finishingTarget.position, 2);
            return;
        }

        if (moveVec == Vector3.zero)
        {
            BodyLookAt(-camForward + camRight + transform.position, 1);
            return;
        }
        
        float angleToMoveVec = Vector3.Angle(transform.forward, moveVec);
        bool movingRight = Vector3.Angle(transform.right, moveVec) < 90;

        if (angleToMoveVec > 30 && angleToMoveVec < 60)
        {
            if (movingRight)
                BodyLookAt(-camForward + transform.position, 1);
            else
                BodyLookAt(camRight + transform.position, 1);
        }
        else if (angleToMoveVec > 120 && angleToMoveVec < 150)
        {
            if (movingRight)
                BodyLookAt(camRight + transform.position, 1);
            else
                BodyLookAt(-camForward + transform.position, 1);
        }
        else
        {
            BodyLookAt(-camForward + camRight + transform.position, 1);
        }
    }

    void BodyLookAt(Vector3 pos, float speedMultiplyer)
    {
        Vector3 direction = pos - Model.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Model.rotation = Quaternion.Lerp(Model.rotation, rotation, Time.deltaTime * BodyRotationSpeed * speedMultiplyer);
    }

    void TargetBoneLookAt(Vector3 pos, float speedMultiplyer)
    {
        Vector3 toPos = pos - transform.position;
        bool rotateÑlockwise = Vector3.Angle(Model.right, toPos) < 90;
        float angleToPos = Vector3.Angle(Model.forward, toPos);

        rot.x = Mathf.LerpAngle(rot.x, angleToPos * (rotateÑlockwise ? -1 : 1), TagretBoteRotationSpeed * speedMultiplyer * Time.deltaTime);

        TargetBone.rotation *= Quaternion.Euler(rot);
    }

    void ResetTarget()
    {
        finishingTarget = null;
        FinishingPanel.SetActive(false);
    }

    public void KillEnemy()
    {
        finishingTarget.GetComponent<Enemy>().Die();
        ResetTarget();
    }

    public void EnableSword()
    {
        Gun.SetActive(false);
        Sword.SetActive(true);
    }

    public void EnableGun()
    {
        Gun.SetActive(true);
        Sword.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        GetComponent<SphereCollider>().radius = FinishingRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Enemy>())
            return;

        finishingTarget = other.gameObject.transform;
        FinishingPanel.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        ResetTarget();
    }
}