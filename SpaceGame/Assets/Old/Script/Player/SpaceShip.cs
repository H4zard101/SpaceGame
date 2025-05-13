using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShip : MonoBehaviour
{

    [Header("=== Ship Movement Settings ===")]
    [SerializeField] private float yawTorque = 500f;
    [SerializeField] private float pitchTorque = 1000f;
    [SerializeField] private float rollTorque = 1000f;
    [SerializeField] public float thrust = 100f;
    [SerializeField] private float upThrust = 50f;
    [SerializeField] private float strafeThrust = 50f;
    [SerializeField, Range(0.001f, 0.999f)] private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)] private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)] private float leftRightGlideReduction = 0.111f;
    [SerializeField] public float glide = 0f;
    [SerializeField] float verticalGlide = 0f;
    [SerializeField] float horizontalGlide = 0f;

    [Header("=== Ship Boost Settings ===")]
    [SerializeField] private float maxBoostAmount = 20f;
    [SerializeField] private float boostDeprecationRate = 0.25f;
    [SerializeField] private float boostRechargeRate = 0.5f;
    [SerializeField] private float boostMultiplier = 5f;
    [SerializeField] private bool boosting = false;
    [SerializeField] public float currentBoostAmount;
    [SerializeField] public float currentThrust;

    Rigidbody rb;


    // Input Values
    private float thrust1d;
    private float upDown1d;
    private float strafe1d;
    private float roll1d;
    private Vector2 pitchYaw;

    // Thruster Particle system
    [SerializeField] GameObject EngineObject;
    public List<ParticleSystem> thrusters = new List<ParticleSystem>();

    [Header("Size Over Lifetime Curves")]
    public AnimationCurve normalSizeCurve;
    public AnimationCurve boostSizeCurve;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // loop through all the children of the engine object and add their particle systems to the list
        if(EngineObject.transform.childCount>0)
        {
            foreach(Transform child in EngineObject.transform)
            {
                ParticleSystem ps = child.GetComponent<ParticleSystem>();
                if(ps != null)
                {

                    thrusters.Add(ps);

                    // set the default size over lifetime curve
                    var sizeModule = ps.sizeOverLifetime;
                    sizeModule.enabled = true;
                    ApplySizeCurve(ps, normalSizeCurve);
                }

            }    
        }
    }
    // for changing the animation curve for the particle systems size over life time curve
    void ApplySizeCurve(ParticleSystem ps, AnimationCurve curve)
    {
        var sizeModule = ps.sizeOverLifetime;
        sizeModule.size = new ParticleSystem.MinMaxCurve(1f, curve);
    }

    private void FixedUpdate()
    {
        HandelBoosting();
        HandelMovement();
    }

    void HandelBoosting()
    {
        if(boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }
    void HandelMovement()
    {
        // Roll
        rb.AddRelativeTorque(Vector3.back * roll1d * rollTorque * Time.deltaTime);

        // Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);

        // Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        // Thrust
        if(thrust1d > 0.1f || thrust1d < -0.1f)
        {
            
            // play the thruster Particle system here
            foreach(ParticleSystem ps in thrusters)
            {
                ps.Play();
                // keep the animation curve the defualt value for moving normally
                ApplySizeCurve(ps, normalSizeCurve);

            }
            if(boosting)
            {
                currentThrust = thrust * boostMultiplier;
                
                // if we boost we want to increase the curve of the particle system meaning the curve looks diffrent
                foreach (ParticleSystem ps in thrusters)
                {
                    ApplySizeCurve(ps, boostSizeCurve);

                }
            }
            else
            {
                currentThrust = thrust;
            }
            rb.AddRelativeForce(Vector3.forward * thrust1d * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;

            // pause the thruster Particle system
            foreach (ParticleSystem ps in thrusters)
            {
                ps.Stop();

                // Because we are stopping we dont want to have the boosted curve so we switch back to the default one
                ApplySizeCurve(ps, normalSizeCurve);

            }

        }

        // Up Down
        if (upDown1d > 0.1f || upDown1d < -0.1f)
        {
           
            rb.AddRelativeForce(Vector3.up * upDown1d * upThrust * Time.deltaTime);
            verticalGlide = upDown1d * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.deltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        // Strafing 
        if (strafe1d > 0.1f || strafe1d < -0.1f)
        {

            rb.AddRelativeForce(Vector3.right * strafe1d * upThrust * Time.deltaTime);
            horizontalGlide = strafe1d * strafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1d = context.ReadValue<float>();
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1d = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1d = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1d = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
    #endregion
}
