using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody spaceShipRB;

    [SerializeField]float verticalMove;
    [SerializeField] float horizontallMove;
    [SerializeField] float mouseInputX;
    [SerializeField] float mouseInputY;
    [SerializeField] float rollInput;

    [SerializeField] float speedMult = 1;
    [SerializeField] float speedMultAngle = 0.5f;
    [SerializeField] float speedRollMultAngle = 0.05f;

    private void Start()
    {
        // Lock the cursor so you cant see it
        Cursor.lockState = CursorLockMode.Locked;
        spaceShipRB = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        verticalMove = Input.GetAxis("Vertical");
        horizontallMove = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        spaceShipRB.AddForce(spaceShipRB.transform.TransformDirection(Vector3.forward) * verticalMove * speedMult, ForceMode.VelocityChange);
        spaceShipRB.AddForce(spaceShipRB.transform.TransformDirection(Vector3.right) * horizontallMove * speedMult, ForceMode.VelocityChange);

        spaceShipRB.AddTorque(spaceShipRB.transform.right * speedMultAngle * mouseInputY * -1, ForceMode.VelocityChange);
        spaceShipRB.AddTorque(spaceShipRB.transform.up * speedMultAngle * mouseInputX, ForceMode.VelocityChange);

        spaceShipRB.AddTorque(spaceShipRB.transform.forward * speedMultAngle * rollInput, ForceMode.VelocityChange);

    }
}
