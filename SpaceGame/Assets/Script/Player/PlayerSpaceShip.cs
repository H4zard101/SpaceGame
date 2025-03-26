using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceShip : MonoBehaviour
{
    Rigidbody spaceShipRB;

    // Inputs
    [SerializeField] private float verticalMove;
    [SerializeField] private float horizontalMove;
    [SerializeField] private float mouseInputX;
    [SerializeField] private float mouseInputY;
    [SerializeField] private float rollInput;

    // Speed Mulyipliers
    [SerializeField] float speedMult = 1.0f;
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
        horizontalMove = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
    }
    private void FixedUpdate()
    {
        //Move forward
        spaceShipRB.AddForce(spaceShipRB.transform.TransformDirection(Vector3.forward) * verticalMove * speedMult * Time.deltaTime, ForceMode.VelocityChange);

        // Move Side to Side
        spaceShipRB.AddForce(spaceShipRB.transform.TransformDirection(Vector3.right) * horizontalMove * speedMult * Time.deltaTime, ForceMode.VelocityChange);

        // Rotate
        spaceShipRB.AddTorque(spaceShipRB.transform.right * speedMultAngle * mouseInputY * -1 * Time.deltaTime, ForceMode.VelocityChange);
        spaceShipRB.AddTorque(spaceShipRB.transform.up * speedMultAngle * mouseInputX * Time.deltaTime, ForceMode.VelocityChange);
        spaceShipRB.AddTorque(spaceShipRB.transform.forward * speedMultAngle * rollInput * Time.deltaTime, ForceMode.VelocityChange);

    }
}
