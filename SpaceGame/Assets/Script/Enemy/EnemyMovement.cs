using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject target; // player Ship
    public Rigidbody enemyRB;

    public float rotationDamp = 0.5f; // Same as the glide in the movement script
    public float thrust = 10.0f; // Same as the thrust variable for the player movement

    public float raycastOffset = 2.5f;
    public float detectionDistance = 20.0f;
    public float distance = 10f;
    private void Update()
    {
        Pathfinding();
        Move();
    }

    void Move()
    {
        //transform.position += transform.forward * thrust * Time.deltaTime;

        Vector3 targetLocation = target.transform.position - transform.position;
        distance = targetLocation.magnitude;
        enemyRB.AddRelativeForce(Vector3.forward * Mathf.Clamp((distance - 7)/50f,0f,1f * thrust * Time.deltaTime));
    }

    void Turn()
    {
        Vector3 pos = target.transform.position - transform.position;
        Quaternion roatation = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, roatation, rotationDamp * Time.deltaTime);
    }


    void Pathfinding()
    {
        RaycastHit hit;
        Vector3 _raycastOffset = Vector3.zero;

        Vector3 left = transform.position - transform.right * raycastOffset;
        Vector3 right = transform.position + transform.right * raycastOffset;
        Vector3 up = transform.position + transform.up * raycastOffset;
        Vector3 down = transform.position - transform.up * raycastOffset;


        // Draw out rays
        Debug.DrawRay(left, transform.forward * detectionDistance, Color.red);
        Debug.DrawRay(right, transform.forward * detectionDistance, Color.red);
        Debug.DrawRay(up, transform.forward * detectionDistance, Color.red);
        Debug.DrawRay(down, transform.forward * detectionDistance, Color.red);

        if(Physics.Raycast(left, transform.forward, out hit, detectionDistance))
        {
            _raycastOffset += Vector3.right;
            
        }
        else if (Physics.Raycast(right, transform.forward, out hit, detectionDistance))
        {
            _raycastOffset -= Vector3.right;

        }
        if (Physics.Raycast(up, transform.forward, out hit, detectionDistance))
        {
            _raycastOffset -= Vector3.up;

        }
        else if (Physics.Raycast(down, transform.forward, out hit, detectionDistance))
        {
            _raycastOffset += Vector3.up;

        }

        if(_raycastOffset != Vector3.zero)
        {
            transform.Rotate(_raycastOffset * 5f * Time.deltaTime);
        }
        else
        {
            Turn();
        }
    }
}
