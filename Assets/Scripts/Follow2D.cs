using UnityEngine;
using System.Collections;

public class Follow2D : MonoBehaviour {
	public Transform[] targets;
	Transform target;
	public Vector3 offset;
	public float positionSmooth;
	public bool rotation;
	public float rotationSmooth;
	public bool local;
	int i;

	public float thresholdrot = 0;

	Quaternion rot;
	// Use this for initialization
	void Start () {
	
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.C)) {
			i += 1;
			i = Mathf.RoundToInt(Mathf.Repeat (i, targets.Length));
		}
	}
	// Update is called once per frame
	void LateUpdate () {
		Vector3 sof = offset * Time.timeScale;
		
		target = targets [i];
		if (local) {
			if (positionSmooth != 0) {
				transform.position = Vector3.Lerp (transform.position, target.position + (target.rotation * (sof)), Time.unscaledDeltaTime * positionSmooth);
			} else {
				transform.position = target.position + (target.rotation * (sof));
			}

		} else {
			if (positionSmooth != 0) {
				transform.position = Vector3.Lerp (transform.position, target.position + sof, Time.unscaledDeltaTime * positionSmooth);
			} else {
				transform.position = target.position + sof;
			}

		}
		//print (target.right);

		if (rotation) {
			if (Mathf.Abs(Mathf.DeltaAngle (transform.eulerAngles.y, target.eulerAngles.y)) > thresholdrot) {
				rot = target.rotation;
			}
			if (rotationSmooth != 0) {
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(0,rot.eulerAngles.y,0), Time.unscaledDeltaTime * rotationSmooth);
			} else {
				transform.rotation = target.rotation;

			}
		}


	}
}
