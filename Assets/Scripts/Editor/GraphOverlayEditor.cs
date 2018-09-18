using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphOverlay))]
public class GraphOverlayEditor : Editor {

    public override void OnInspectorGUI()
    {
        GraphOverlay myTarget = (GraphOverlay)target;

        myTarget.vehicleBody = (Rigidbody)EditorGUILayout.ObjectField("Vehicle", myTarget.vehicleBody, typeof(Rigidbody), true);

        if (!myTarget.vehicleBody)
            return;

        myTarget.timeTravel = EditorGUILayout.FloatField("Time travel", myTarget.timeTravel);
        
        myTarget.width = EditorGUILayout.Slider("Width", myTarget.width, 0, 1);
		myTarget.height = EditorGUILayout.Slider("Height", myTarget.height, 0, 1);

        myTarget.widthSeconds = EditorGUILayout.FloatField("Width seconds", myTarget.widthSeconds);
        myTarget.heightMeters = EditorGUILayout.FloatField("Height meters", myTarget.heightMeters);

        myTarget.bgColor = EditorGUILayout.ColorField("Bg color", myTarget.bgColor);
        myTarget.forwardColor = EditorGUILayout.ColorField("Forward color", myTarget.forwardColor);
        myTarget.sidewaysColor = EditorGUILayout.ColorField("Sideways color", myTarget.sidewaysColor);
        myTarget.zeroColor = EditorGUILayout.ColorField("Zero color", myTarget.zeroColor);

        if (myTarget.vehicleBody)
        {
	        foreach (var wheelConfig in myTarget.wheelConfigs)
	        {
                EditorGUILayout.LabelField(wheelConfig.collider.name);
                wheelConfig.visible = EditorGUILayout.Toggle("Enabled", wheelConfig.visible);
	        }
        }
    }
}