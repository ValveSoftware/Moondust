/*

This script is based on TubeRenderer.cs, created by Ray Nothnagel of Last Bastion Games. It is 
free for use and available on the Unify Wiki.

For other components, see:
http://lastbastiongames.com/middleware/

(C) 2008 Last Bastion Games
*/


using System;
using UnityEngine;
 
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RacetrackRenderer : MonoBehaviour
{


    [Serializable]
    public class TubeVertex
    {
        public Vector3 point = Vector3.zero;
        public float radius = 1.0f;
        public Color color = Color.white;

        public TubeVertex(Vector3 pt, float r, Color c)
        {
            point = pt;
            radius = r;
            color = c;
        }
    }
    public BezierSpline spline;

    public float radius = 1;

    public float endFadeLength = 5f;

    private MeshRenderer mr;
    private MeshFilter mf;


    public float pointDensity = 1;
    private float lastPointDensity;

    [HideInInspector]
    public TubeVertex[] vertices;

    public Material material;

    public int crossSegments = 3;
    private Vector3[] crossPoints;
    private int lastCrossSegments;
    public float flatAtDistance = -1;

    private Vector3 lastCameraPosition1;
    private Vector3 lastCameraPosition2;
    public int movePixelsForRebuild = 6;
    public float maxRebuildTime = 0.1f;
    private float lastRebuildTime = 0.00f;

    void Reset()
    {

        vertices = new TubeVertex[]
        {
            new TubeVertex(Vector3.zero, 1.0f, Color.white),
            new TubeVertex(new Vector3(1,0,0), 1.0f, Color.white),
        };
    }
    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();
        mr.sharedMaterial = material;
    }

    private void OnEnable()
    {
        Start();
    }

    [ContextMenu("Rebuild Vertices")]
    public void Respline()
    {
        int ptCount = (int)(spline.EstimateLength() * pointDensity);
        vertices = new TubeVertex[ptCount+1];
        for (int i = 0; i <= ptCount; i++)
        {
            float t = (float)i / (float)ptCount;
            Vector3 pos = spline.GetPoint(t);
            float fade = Mathf.Clamp01(Vector3.Distance(pos, spline.GetPoint(0)) / endFadeLength); // fade tube at beginning
            fade *= Mathf.Clamp01(Vector3.Distance(pos, spline.GetPoint(1)) / endFadeLength); // and at end
            vertices[i] = new TubeVertex(transform.InverseTransformPoint(pos), 1f, Color.Lerp(Color.clear, Color.white, fade));
        }
    }

    [ContextMenu("Rebuild Mesh")]
    public void Rebuild()
    {
        //draw tube

        if (crossSegments != lastCrossSegments)
        {
            crossPoints = new Vector3[crossSegments];
            float theta = 2.0f * Mathf.PI / crossSegments;
            for (int c = 0; c < crossSegments; c++)
            {
                crossPoints[c] = new Vector3(Mathf.Cos(theta * c), Mathf.Sin(theta * c), 0);
            }
            lastCrossSegments = crossSegments;
        }

        Vector3[] meshVertices = new Vector3[vertices.Length * crossSegments];
        Vector2[] uvs = new Vector2[vertices.Length * crossSegments];
        Color[] colors = new Color[vertices.Length * crossSegments];
        int[] tris = new int[vertices.Length * crossSegments * 6];
        int[] lastVertices = new int[crossSegments];
        int[] theseVertices = new int[crossSegments];
        Quaternion rotation = Quaternion.identity;

        for (int p = 0; p < vertices.Length; p++)
        {
            if (p < vertices.Length - 1) rotation = Quaternion.FromToRotation(Vector3.forward, vertices[p + 1].point - vertices[p].point);

            for (int c = 0; c < crossSegments; c++)
            {
                int vertexIndex = p * crossSegments + c;
                meshVertices[vertexIndex] = vertices[p].point + rotation * crossPoints[c] * vertices[p].radius * radius;
                uvs[vertexIndex] = new Vector2((0.0f + c) / crossSegments, (0.0f + p) / vertices.Length);
                colors[vertexIndex] = vertices[p].color;

                //				print(c+" - vertex index "+(p*crossSegments+c) + " is " + meshVertices[p*crossSegments+c]);
                lastVertices[c] = theseVertices[c];
                theseVertices[c] = p * crossSegments + c;
            }
            //make triangles
            if (p > 0)
            {
                for (int c = 0; c < crossSegments; c++)
                {
                    int start = (p * crossSegments + c) * 6;
                    tris[start] = lastVertices[c];
                    tris[start + 1] = lastVertices[(c + 1) % crossSegments];
                    tris[start + 2] = theseVertices[c];
                    tris[start + 3] = tris[start + 2];
                    tris[start + 4] = tris[start + 1];
                    tris[start + 5] = theseVertices[(c + 1) % crossSegments];
                    //					print("Triangle: indexes("+tris[start]+", "+tris[start+1]+", "+tris[start+2]+"), ("+tris[start+3]+", "+tris[start+4]+", "+tris[start+5]+")");
                }
            }
        }

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
        }
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.uv = uvs;
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void LateUpdate()
    {
        if (null == vertices ||
            vertices.Length <= 1 || spline == null)
        {
            mr.enabled = false;
            return;
        }
        mr.enabled = true;

        //rebuild the spline?
        bool respline = false;
        if (pointDensity <= 0) pointDensity = 1;
        if (pointDensity != lastPointDensity)
        {
            respline = true;
            lastPointDensity = pointDensity;
        }

        if (respline)
        {
            Respline();
        }

        //rebuild the mesh?
        bool re = false;
        float distFromMainCam;
        if (vertices.Length > 1)
        {
            Vector3 cur1 = Camera.main.WorldToScreenPoint(vertices[0].point);
            distFromMainCam = lastCameraPosition1.z;
            lastCameraPosition1.z = 0;
            Vector3 cur2 = Camera.main.WorldToScreenPoint(vertices[vertices.Length - 1].point);
            lastCameraPosition2.z = 0;

            float distance = (lastCameraPosition1 - cur1).magnitude;
            distance += (lastCameraPosition2 - cur2).magnitude;

            if (distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime)
            {
                re = true;
                lastCameraPosition1 = cur1;
                lastCameraPosition2 = cur2;
            }
        }


        if (re)
        {
            Rebuild();
        }
    }

    //sets all the points to points of a Vector3 array, as well as capping the ends.
    void SetPoints(Vector3[] points, float radius, Color col)
    {
        if (points.Length < 2) return;
        vertices = new TubeVertex[points.Length + 2];

        Vector3 v0offset = (points[0] - points[1]) * 0.01f;
        vertices[0] = new TubeVertex(v0offset + points[0], 0.0f, col);
        Vector3 v1offset = (points[points.Length - 1] - points[points.Length - 2]) * 0.01f;
        vertices[vertices.Length - 1] = new TubeVertex(v1offset + points[points.Length - 1], 0.0f, col);

        for (int p = 0; p < points.Length; p++)
        {
            vertices[p + 1] = new TubeVertex(points[p], radius, col);
        }
    }
}