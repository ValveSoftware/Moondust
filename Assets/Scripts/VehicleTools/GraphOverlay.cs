

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

/// <summary>
/// This is a really simple graphing solution for the WheelCollider's friction slips.
/// </summary> 
public class GraphOverlay : MonoBehaviour
{
    [Serializable]
    public class WheelConfig
    {
        public WheelCollider collider;
        public bool visible;

        public List<float> longData = new List<float>();
        public List<float> latData = new List<float>();
    }

	public int thickness = 1;
	public float width = 0.35f;
	public float height = 0.34f;
	public float widthSeconds = 2f; 
	public float heightMeters = 3f;
	public Color32 bgColor = Color.white;
	public Color32 forwardColor = Color.red;
	public Color32 sidewaysColor = Color.green;
	public Color32 guidesColor = Color.blue;
	public Color32 zeroColor = Color.black;
	public Rigidbody vehicleBody;
	[Range(0f, 10f)]
	public float timeTravel;
	public List<WheelConfig> wheelConfigs = new List<WheelConfig>();

	Color32[] m_PixelsBg;
	Color32[] m_Pixels;
	Text m_SpeedText;
	Texture2D m_Texture;	
	int m_WidthPixels;
	int m_HeightPixels;

    const string k_EventSystemName = "EventSystem";
    const string k_GraphCanvasName = "GraphCanvas";
    const string k_GraphImageName = "RawImage";
    const string k_InfoTextName = "InfoText";
    const float k_GUIScreenEdgeOffset = 10f;
    const int k_InfoFontSize = 16;
    const float k_MaxRecordTimeTravel = 0.01f;

	void Start()
	{
		// Add GUI infrastructure.
        var eventSystem = new GameObject(k_EventSystemName);	
		eventSystem.AddComponent<EventSystem>();
		eventSystem.AddComponent<StandaloneInputModule>();

        var canvas = new GameObject(k_GraphCanvasName);
		var canvasScript = canvas.AddComponent<Canvas>();
		canvas.AddComponent<CanvasScaler>();
		canvas.AddComponent<GraphicRaycaster>();

		canvasScript.renderMode = RenderMode.ScreenSpaceOverlay;

		// Add a raw image object.
        var rawImageGO = new GameObject(k_GraphImageName);
		rawImageGO.transform.parent = canvas.transform;
		var imageComponent = rawImageGO.AddComponent<RawImage>();
		var imageXform = rawImageGO.GetComponent<RectTransform>();

		imageXform.anchorMin = Vector2.up;
        imageXform.anchorMax = Vector2.up;
        imageXform.pivot = Vector2.up;
        imageXform.anchoredPosition = new Vector2(k_GUIScreenEdgeOffset, -k_GUIScreenEdgeOffset);

		// Set up our texture.
		m_WidthPixels = (int)(Screen.width * width);
		m_HeightPixels = (int)(Screen.height * height);
		m_Texture = new Texture2D(m_WidthPixels, m_HeightPixels);

		imageComponent.texture = m_Texture;
		imageComponent.SetNativeSize();

		m_Pixels = new Color32[m_WidthPixels * m_HeightPixels];
		m_PixelsBg = new Color32[m_WidthPixels * m_HeightPixels];

	    for (int i = 0; i < m_Pixels.Length; ++i)
	    {
	        m_PixelsBg[i] = bgColor;
	    }

		SetupWheelConfigs();

		// Add speed textbox.
        var infoGo = new GameObject(k_InfoTextName);
		infoGo.transform.parent = canvas.transform;
		m_SpeedText = infoGo.AddComponent<Text>();
		var textXform = infoGo.GetComponent<RectTransform>();

		m_SpeedText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        m_SpeedText.fontSize = k_InfoFontSize;

        textXform.anchorMin = Vector2.up;
        textXform.anchorMax = Vector2.up;
        textXform.pivot = Vector2.up;
        textXform.anchoredPosition = new Vector2(k_GUIScreenEdgeOffset, -m_HeightPixels - k_GUIScreenEdgeOffset);
		var rect = textXform.sizeDelta;
		rect.x = m_WidthPixels;
		textXform.sizeDelta = rect;
	}

	void Reset()
	{
		SetupWheelConfigs ();
	}


    public void SetupWheelConfigs ()
    {
        wheelConfigs.Clear();

        // Locate all the wheels.
        if (vehicleBody)
        {
            foreach (var wheel in vehicleBody.GetComponentsInChildren<WheelCollider>())
            {
                var wheelConfig = new WheelConfig();
                wheelConfig.visible = true;
                wheelConfig.collider = wheel;

                wheelConfigs.Add(wheelConfig);
            }
        }
    }

	void FixedUpdate()
	{
		foreach (var wheelConfig in wheelConfigs)
		{
			WheelHit hit;
			if (!wheelConfig.collider.GetGroundHit(out hit))
			{
                // No hit!
				continue;
			}

            if (Mathf.Abs(timeTravel) < k_MaxRecordTimeTravel)// TODO: solve this mystery
			{
				wheelConfig.longData.Add(hit.forwardSlip);
				wheelConfig.latData.Add(hit.sidewaysSlip);
			}
		}
	}

	void Update()
	{
		// Clear.
		Array.Copy(m_PixelsBg, m_Pixels, m_Pixels.Length);

		// Draw guides.
        DrawLine(new Vector2(0f, m_HeightPixels * 0.5f), new Vector2(m_WidthPixels, m_HeightPixels * 0.5f), zeroColor);

		float guide = 1f / heightMeters * m_HeightPixels;
        float upperGuide = m_HeightPixels * 0.5f - guide;
        float lowerGuide = m_HeightPixels * 0.5f + guide;
		DrawLine(new Vector2(0f, upperGuide), new Vector2(m_WidthPixels, upperGuide), guidesColor);
		DrawLine(new Vector2(0f, lowerGuide), new Vector2(m_WidthPixels, lowerGuide), guidesColor);

		// Draw graphs.
		int samplesOnScreen = (int)(widthSeconds / Time.fixedDeltaTime);
		int stepsBack = (int)(timeTravel / Time.fixedDeltaTime);

		foreach (var wheelConfig in wheelConfigs)
		{
			if (!wheelConfig.visible)
				continue;
				
			int cursor = Mathf.Max(wheelConfig.longData.Count - samplesOnScreen - stepsBack, 0);

			// Forward slip.
			for (int i = cursor; i < wheelConfig.longData.Count - 1 - stepsBack; ++i)
			{
				DrawLine( PlotSpace(cursor, i, wheelConfig.longData[i]), PlotSpace(cursor, i + 1, wheelConfig.longData[i + 1]), forwardColor );
			}

			// Sideways slip.
			for (int i = cursor; i < wheelConfig.latData.Count - 1 - stepsBack; ++i)
			{
				DrawLine( PlotSpace(cursor, i, wheelConfig.latData[i]), PlotSpace(cursor, i + 1, wheelConfig.latData[i + 1]), sidewaysColor );
			}
		}

		m_Texture.SetPixels32(m_Pixels);
		m_Texture.Apply();

		if (vehicleBody)
			m_SpeedText.text = string.Format("Speed: {0:0.00} m/s", vehicleBody.velocity.magnitude);
	}

	// Convert time-value to the pixel plot space.
	Vector2 PlotSpace(int cursor, int sample, float value)
	{
		float x = (sample - cursor) * Time.fixedDeltaTime / widthSeconds * m_WidthPixels;

		float v = value + heightMeters / 2;
		float y = v / heightMeters * m_HeightPixels;

		if (y < 0)
			y = 0;

		if (y >= m_HeightPixels)
			y = m_HeightPixels - 1;

		return new Vector2(x, y);
	}

	void DrawLine(Vector2 from, Vector2 to, Color32 color)
	{
		int i;
		int j;

		if (Mathf.Abs(to.x - from.x) > Mathf.Abs(to.y - from.y))
		{
			// Horizontal line.
			i = 0;
			j = 1;
		}
		else
        {
			// Vertical line.
			i = 1;
			j = 0;
		}

		int x = (int)from[i];
		int delta = (int)Mathf.Sign(to[i] - from[i]);
		while (x != (int)to[i])
		{
			int y = (int)Mathf.Round(from[j] + (x - from[i]) * (to[j] - from[j]) / (to[i] - from[i]));

		    int index;
		    if (i == 0)
		        index = y * m_WidthPixels + x;
		    else
		        index = x * m_WidthPixels + y;

            index = Mathf.Clamp(index, 0, m_Pixels.Length - 1);
            m_Pixels[index] = color;

			x += delta;
		}
	}
}