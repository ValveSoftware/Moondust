#if	UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using System.IO;
using System.Collections;

public class BeautyShotExtraFine : MonoBehaviour
{
	public string generateFilename()
	{
		//var frame = Backstage.Director.PlayheadInFrames;
		//var frame = Backstage.frameCount;
		var frame = Time.frameCount;
		return string.Format("/{0}.png", frame + frameOffset );
	}

	private string _folder = "";
	private Texture2D _result = null;

	public LayerMask layerMask;

	public int frameRate = 60;
	public float duration = 10;
	public int	frameOffset = 0;
    public int framesToSkip = 0;
	private float numFrames;

    public int TargetResolutionWidth = 1920*2;
    public int TargetResolutionHeight = 1200*2;

	void Start()
	{
		if( !Application.isPlaying )
			return;

		Application.runInBackground = true;
		Time.captureFramerate = frameRate;
		numFrames = duration * frameRate;

#if UNITY_EDITOR
		var sceneName = System.IO.Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().name);
#else
	    var sceneName = Application.loadedLevelName;
#endif
		var path = "BeautyShots/{0}/{1}";

		//	"640 k ought to be enough for anybody."
		for( var count = 0; count < 640000; count++ )
		{
			_folder = string.Format( path, sceneName, count );
			if( !System.IO.Directory.Exists( _folder ) )
				break;
		}

		System.IO.Directory.CreateDirectory( _folder );
		_result = new Texture2D( TargetResolutionWidth, TargetResolutionHeight, TextureFormat.RGB24, false );

	    StartCoroutine(CaptureAtEndOfFrame());
	}

	private byte[] captureCam( Camera cam, int w, int h )
	{
		var oldcullmask = cam.cullingMask;
		cam.cullingMask = layerMask;
		RenderTexture rt = RenderTexture.GetTemporary( w, h );

		var tmp = cam.targetTexture;
		cam.targetTexture = rt;
		cam.Render();

		RenderTexture.active = rt;
		_result.ReadPixels( new Rect( 0, 0, w, h ), 0, 0, false );
	    _result.Apply(false, false);
	    RenderTexture.active = null;
		cam.targetTexture = tmp;
	    RenderTexture.ReleaseTemporary(rt);
		cam.cullingMask = oldcullmask;
		return _result.EncodeToPNG();
	}

    private IEnumerator CaptureAtEndOfFrame()
    {
        var targetCam = GetComponent<Camera>();
        while (Time.frameCount < numFrames)
        {
            yield return new WaitForEndOfFrame();

            if (framesToSkip > 0)
            {
                --framesToSkip;
                continue;
            }

            var filename = _folder + generateFilename();
            File.WriteAllBytes(filename, captureCam(targetCam, _result.width, _result.height));

            if (Time.frameCount % frameRate == 0)
                Debug.Log(string.Format("{0} second rendered, {1} total frames.", Time.frameCount / Time.captureFramerate, Time.frameCount));
        }

        Application.runInBackground = false;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}