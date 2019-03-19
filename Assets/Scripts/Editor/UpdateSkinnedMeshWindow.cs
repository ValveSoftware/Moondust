//Copyright(c) 2016 Tim McDaniel (TrickyHandz on forum.unity3d.com)
// Adapted from code provided by Alima Studios on forum.unity.com
// http://forum.unity3d.com/threads/prefab-breaks-on-mesh-update.282184/#post-2661445

// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to
// the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,  
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using UnityEngine;
using UnityEditor;

public class UpdateSkinnedMeshWindow : EditorWindow
{
    [MenuItem("Window/Update Skinned Mesh Bones")]
    public static void OpenWindow()
    {
        var window = GetWindow<UpdateSkinnedMeshWindow>();
        window.titleContent = new GUIContent("Skin Updater");
    }

    private SkinnedMeshRenderer targetSkin;
    private SkinnedMeshRenderer originalSkin;

    private void OnGUI()
    {
        targetSkin = EditorGUILayout.ObjectField("Target", targetSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
        originalSkin = EditorGUILayout.ObjectField("Original", originalSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;

        GUI.enabled = (targetSkin != null && originalSkin != null);

        if (GUILayout.Button("Update Skinned Mesh Renderer"))
        {

            Transform[] newBones = new Transform[targetSkin.bones.Length];

            int a = 0;
            //for (int i = 0; i < originalSkin.bones.Length; i++;)
            foreach (var originalBone in originalSkin.bones)
                {
                    int b = 0;
                    foreach (var newBone in targetSkin.bones)
                    {
                        if (newBone.name == originalBone.name)
                        {
                            newBones[a] = targetSkin.bones[b];
                            continue;
                        }
                        b++;
                    }
                    a++;
                }
            targetSkin.sharedMesh = originalSkin.sharedMesh;
            targetSkin.bones = newBones;
        }
    }
}