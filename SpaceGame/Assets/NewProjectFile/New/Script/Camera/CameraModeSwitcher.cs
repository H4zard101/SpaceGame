
using UnityEngine;
using Cinemachine;

public class CameraModeSwitcher : MonoBehaviour
{
    [Header("Assign your virtual cameras")]
    public CinemachineVirtualCamera originalVCam;
    public CinemachineVirtualCamera birdsEyeVCam;

    [Header("Priorities (higher wins)")]
    public int activePriority = 20;
    public int inactivePriority = 5;

    [Header("Control scripts to toggle when switching")]
    [Tooltip("E.g., your CameraMovement on the original VCam, and any top-down controls on the birds-eye VCam.")]
    public MonoBehaviour originalControls;  // e.g., CameraMovement on VCam_Original
    public MonoBehaviour birdsEyeControls;  // (optional) a top-down control script on VCam_BirdsEye

    bool isBirdsEye = false;

    void Awake()
    {
        SetMode(false, instant: true); // start in original view
    }

    public void SwitchToBirdsEye() => SetMode(true);
    public void SwitchToOriginal() => SetMode(false);
    public void ToggleMode() => SetMode(!isBirdsEye);

    void SetMode(bool birdsEye, bool instant = false)
    {
        isBirdsEye = birdsEye;

        if (birdsEyeVCam) birdsEyeVCam.Priority = birdsEye ? activePriority : inactivePriority;
        if (originalVCam) originalVCam.Priority = birdsEye ? inactivePriority : activePriority;

        // Enable/disable control scripts so only the active view handles input
        if (originalControls) originalControls.enabled = !birdsEye;
        if (birdsEyeControls) birdsEyeControls.enabled = birdsEye;

        if (instant)
        {
            var brain = Camera.main ? Camera.main.GetComponent<CinemachineBrain>() : null;
            if (brain != null)
            {
                var prev = brain.m_DefaultBlend;
                brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
                // priorities already changed; restore next frame
                StartCoroutine(RestoreBlendNextFrame(brain, prev));
            }
        }
    }

    System.Collections.IEnumerator RestoreBlendNextFrame(CinemachineBrain brain, CinemachineBlendDefinition prev)
    {
        yield return null;
        brain.m_DefaultBlend = prev;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            FindObjectOfType<CameraModeSwitcher>()?.ToggleMode();
    }
}
