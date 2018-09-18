using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Camera cam;

    [HideInInspector]
    public bool tpc;

    public float speed = 1;
    public float lookSpeed = 1;

    public bool startTP;

    private Rigidbody camRigidbody;

    private Vector3 camVelocity;
    private Vector2 camRotation;

    private Transform remote;
    private float initialScale;


    private void Start()
    {
        initialScale = transform.lossyScale.x;
        camRigidbody = cam.GetComponent<Rigidbody>();
        camRigidbody.mass *= initialScale;
        remote = new GameObject().transform;
        remote.parent = transform;
        cam.nearClipPlane *= initialScale;
        //cam.farClipPlane *= scl;
        camRotation = new Vector2(cam.transform.localEulerAngles.y, -cam.transform.localEulerAngles.x);
        transform.parent = null;
        if (startTP)
        {
            tpc = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public UnityEngine.UI.Toggle ui_tpc;
    private void Update()
    {
        ui_tpc.isOn = tpc;

        cam.gameObject.SetActive(tpc);

        if (tpc)
        {
            float s = speed;
            s *= Mathf.Lerp(1, 3f, Input.GetAxis("Sprint"));
            camVelocity = Vector3.zero;
            camVelocity += cam.transform.forward * s * Input.GetAxis("Forward");
            camVelocity += transform.up * s * Input.GetAxis("Vertical");
            camVelocity += cam.transform.right * s * Input.GetAxis("Horizontal");

            // if(Input.mouseScrollDelta)

            camVelocity *= initialScale; // adjust for player scale

            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            camRotation += mouse * lookSpeed;
            remote.localRotation = Quaternion.Euler(-camRotation.y, camRotation.x, -mouse.x / 2);
        }
    }

    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ToggleTPC(bool val)
    {
        tpc = val;
        if (tpc) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    public void ThrowingPlus()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ThrowingPlus");
    }

    private void FixedUpdate()
    {
        camRigidbody.velocity = Vector3.Lerp(camRigidbody.velocity, camVelocity, Time.fixedDeltaTime * 5);

        // Rotations stack right to left,
        // so first we undo our rotation, then apply the target.
        var delta = remote.rotation * Quaternion.Inverse(camRigidbody.rotation);

        float angle; Vector3 axis;
        delta.ToAngleAxis(out angle, out axis);

        // We get an infinite axis in the event that our rotation is already aligned.
        if (float.IsInfinity(axis.x))
            return;

        if (angle > 180f)
            angle -= 360f;

        // Here I drop down to 0.9f times the desired movement,
        // since we'd rather undershoot and ease into the correct angle
        // than overshoot and oscillate around it in the event of errors.
        Vector3 angular = (0.1f * Mathf.Deg2Rad * angle / Time.fixedDeltaTime) * axis.normalized;

        camRigidbody.angularVelocity = angular;
    }
}