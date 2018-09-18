using UnityEngine;
using System;
using System.Collections;


public class BuggyBuddy : MonoBehaviour
{
    public Transform turret;

    [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;

    [Tooltip("Maximum Turning torque")]
    public float maxTurnTorque = 30f;

    [Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;

	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;

	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject[] wheelRenders;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

    public AudioSource au_motor;
    public AudioSource au_skid;

    public WheelDust skidsample;

    public Vector3 localGravity;
    
    public bool stranded;

    public BuggyReset resetter;

    public ParticleSystem turboParticles;
    public float turboSpeed = 20;

    [HideInInspector]
    public float mvol;

    [HideInInspector]
    public Vector2 steer;

    [HideInInspector]
    public float throttle;

    [HideInInspector]
    public float handBrake;

    [HideInInspector]
    public Transform controllerReference;

    [HideInInspector]
    public float speed;

    [HideInInspector]
    public Rigidbody body;


    private float turretRot;
    private bool[] wasStranded;
    private bool turbo;
    private float shootTimer;
    private float skidSpeed = 3;
    private float svol;
    private WheelCollider[] m_Wheels;

    private void Start()
	{
        wasStranded = new bool[30];
        
        body = GetComponent<Rigidbody>();
		m_Wheels = GetComponentsInChildren<WheelCollider>();

        StartCoroutine(DoCheckStranded());
	}

    /*
    private void TurretInput()
    {
        Vector2 tIn = TurretControl.joystick();

        Vector3 tur = new Vector3(tIn.x, 0, tIn.y);
        tur = TurretControl.transform.TransformDirection(tur);
        tur = transform.InverseTransformDirection(tur);
        tur = Vector3.ProjectOnPlane(tur, Vector3.up);
        turretRot = VectorMath.FindAngle(Vector3.forward, tur, Vector3.up) * Mathf.Rad2Deg;
        Vector3 turup = Vector3.forward;
        turret.localRotation = Quaternion.Euler(turup * turretRot);


        if (rapidfireTime == 0)
        {
            if (TurretControl.GetPressDown(KnucklesButton.Trigger))
            {
                Fire();
            }
        }else
        {
            if (shootTimer > rapidfireTime&& TurretControl.GetPress(KnucklesButton.Trigger))
            {
                Fire();
                shootTimer = 0;
            }
            shootTimer += Time.deltaTime;
        }
    }
    */

    private void Update()
    {
        m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

        if (!resetter.resetting && stranded)
        {
            Reset();
        }

        //TurretInput();

        float forward = maxTorque * ((turbo ? 1 : throttle) * 2);
        if (steer.y < -0.5f)
            forward *= -1;

        float angle = maxAngle * steer.x;

        speed = transform.InverseTransformVector(body.velocity).z;

        float forw = Mathf.Abs(speed);


        angle /= 1 + forw / 20;

        float fVol = Mathf.Abs(forward);
        mvol = Mathf.Lerp(mvol, Mathf.Pow((fVol / maxTorque), 0.8f) * Mathf.Lerp(0.4f, 1.0f, (Mathf.Abs(m_Wheels[2].rpm) / 200)) * Mathf.Lerp(1.0f, 0.5f, handBrake), Time.deltaTime * 9);

        au_motor.volume = Mathf.Clamp01(mvol);
        float motorPitch = Mathf.Lerp(0.8f, 1.0f, mvol);
        au_motor.pitch = Mathf.Clamp01(motorPitch);

        svol = Mathf.Lerp(svol, skidsample.amt / skidSpeed, Time.deltaTime * 9);

        au_skid.volume = Mathf.Clamp01(svol);
        float skidPitch = Mathf.Lerp(0.9f, 1.0f, svol);
        au_skid.pitch = Mathf.Clamp01(skidPitch);

        for (int i = 0; i < wheelRenders.Length; i++)
        {
            WheelCollider wheel = m_Wheels[i];

            if (wheel.transform.localPosition.z > 0)
            {
                // front wheels
                wheel.steerAngle = angle;

                //4wd?
                wheel.motorTorque = forward;
            }

            if (wheel.transform.localPosition.z < 0) // back wheels
            {

            }

            wheel.brakeTorque = turbo ? 0 : Mathf.Lerp(Mathf.Abs(forward) < 0.1f ? 1000 : 0, brakeTorque, handBrake);

            wheel.motorTorque = forward;

            if (wheel.transform.localPosition.x < 0) // left wheels
            {
            }

            if (wheel.transform.localPosition.x >= 0) // right wheels
            {
            }

            // Update visual wheels if they exist, and the colliders are enabled
            if (wheelRenders[i] != null && m_Wheels[0].enabled)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                Transform shapeTransform = wheelRenders[i].transform;
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }

        steer = Vector2.Lerp(steer, Vector2.zero, Time.deltaTime * 4);
    }

    private IEnumerator DoCheckStranded()
    {
        while (true)
        {
            bool stationary = body.velocity.sqrMagnitude < 0.7f;
            bool flipped = Vector3.Dot(transform.up, Vector3.up) < 0.3f;
            bool isStrand = stationary && flipped;

            for (int i = 0; i < 29; i++)
            {
                wasStranded[i] = wasStranded[i + 1];
            }
            wasStranded[29] = isStrand;


            int strandTime = 0;
            for (int i = 29; i >= 0; i--)
            {
                if (wasStranded[i])
                {
                    strandTime++;
                }
                else
                {
                    i = 0;
                }
            }


            if (isStrand && strandTime > 25)
            {
                stranded = true;
            }
            else
            {
                stranded = false;
            }


            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator turboGo(float time)
    {
        if (turbo == false) // make sure I'm not already turboing
        {
            turbo = true;
            turboParticles.Play();
            yield return new WaitForSeconds(time);
            turboParticles.Stop();
            turbo = false;
        }
    }

    public void Reset()
    {
        if (!resetter.resetting)
        {
            resetter.ResetNow();
        }
    }

    public void ResetStranded()
    {
        wasStranded = new bool[30];
    }


    private void FixedUpdate()
    {
        body.AddForce(localGravity, ForceMode.Acceleration);

        if (turbo)
        {
            body.AddForce(transform.forward * turboSpeed, ForceMode.Acceleration);
        }
    }

    public class VectorMath
    {
        public static float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
        {
            // If the vector the angle is being calculated to is 0...
            if (toVector == Vector3.zero)
                // ... the angle between them is 0.
                return 0f;

            // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
            float angle = Vector3.Angle(fromVector, toVector);

            // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
            Vector3 normal = Vector3.Cross(fromVector, toVector);

            // The dot product of the normal with the upVector will be positive if they point in the same direction.
            angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

            // We need to convert the angle we've found from degrees to radians.
            angle *= Mathf.Deg2Rad;

            return angle;
        }
    }
}