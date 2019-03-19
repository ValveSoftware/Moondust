using UnityEngine;
using UnityEditor;

class VehicleSkeletonWizard : EditorWindow
{
	int m_AxlesCount = 2;
	float m_Mass = 1000;
	float m_AxleStep = 2;
	float m_AxleWidth = 2;
	float m_AxleShift = -0.5f;

	[MenuItem ("Vehicles/Create skeleton...")]
	public static void  ShowWindow ()
    {
		GetWindow(typeof(VehicleSkeletonWizard));
	}

	void OnGUI ()
    {
		m_AxlesCount = EditorGUILayout.IntSlider ("Axles: ", m_AxlesCount, 2, 10);
		m_Mass = EditorGUILayout.FloatField ("Mass: ", m_Mass);
		m_AxleStep = EditorGUILayout.FloatField ("Axle step: ", m_AxleStep);
		m_AxleWidth = EditorGUILayout.FloatField ("Axle width: ", m_AxleWidth);
		m_AxleShift = EditorGUILayout.FloatField ("Axle shift: ", m_AxleShift);

		if (GUILayout.Button("Generate")) 
        {
			CreateCar ();
		}
	}

	void CreateCar()
	{
		var root = new GameObject ("carRoot");
		var rootBody = root.AddComponent<Rigidbody> ();
		rootBody.mass = m_Mass;

		var body = GameObject.CreatePrimitive (PrimitiveType.Cube);
		body.transform.parent = root.transform;

		float length = (m_AxlesCount - 1) * m_AxleStep;
		float firstOffset = length * 0.5f;

		body.transform.localScale = new Vector3(m_AxleWidth, 1, length);

		for (int i = 0; i < m_AxlesCount; ++i) 
		{
			var leftWheel = new GameObject (string.Format("a{0}l", i));
			var rightWheel = new GameObject (string.Format("a{0}r", i));

			leftWheel.AddComponent<WheelCollider> ();
			rightWheel.AddComponent<WheelCollider> ();

			leftWheel.transform.parent = root.transform;
			rightWheel.transform.parent = root.transform;

            leftWheel.transform.localPosition = new Vector3(-m_AxleWidth * 0.5f, m_AxleShift, firstOffset - m_AxleStep * i);
            rightWheel.transform.localPosition = new Vector3(m_AxleWidth * 0.5f, m_AxleShift, firstOffset - m_AxleStep * i);
		}

		root.AddComponent<EasySuspension>();
		root.AddComponent<Valve.VR.InteractionSystem.Sample.BuggyBuddy>();
	}
}