#if	UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using System.IO;
using System.Collections;

public class BeautyShot : MonoBehaviour
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
	private float numFrames;
	
	public bool captureUsingScreenshot = false;
	
	public enum Supersample
	{
		None = 1,
		Two = 2,
		Four = 4,
		Eight = 8,
		Sixteen = 16,
		Wtf = 32
	};
	
	public Supersample supersampleScreenshot = Supersample.None;

	private void Start()
	{
		if( !Application.isPlaying )
			return;

#if	UNITY_EDITOR
		Application.runInBackground = true;
		Time.captureFramerate = frameRate;
		numFrames = duration * frameRate;

		var sceneName = System.IO.Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().name);
		var path = "BeautyShots/{0}/{1}";

		//	"640 k ought to be enough for anybody."
		for( var count = 0; count < 640000; count++ )
		{
			_folder = string.Format( path, sceneName, count );
			if( !System.IO.Directory.Exists( _folder ) )
				break;
		}

		System.IO.Directory.CreateDirectory( _folder );
		_result = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
#endif
	}

#if	UNITY_EDITOR
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
		cam.targetTexture = tmp;
		cam.cullingMask = oldcullmask;
		return _result.EncodeToPNG();
	}
#endif
	
#if	UNITY_EDITOR
	private void OnPostRender()
	{
		var filename = _folder + generateFilename();
		if( captureUsingScreenshot == false )
		{
			#if !UNITY_WEBPLAYER
			var cam = Camera.current;
			if( cam != null )
			{
				System.IO.File.WriteAllBytes( filename, captureCam( cam, Screen.width, Screen.height ) );
				Debug.Log( "File written" );
			}
			else
				Debug.LogError( "Cam is null?" );
#endif
		}
		else
			ScreenCapture.CaptureScreenshot( filename, (int)supersampleScreenshot );

		if( Time.frameCount % frameRate == 0 )
			Debug.Log( string.Format( "{0} second rendered, {1} total frames.", Time.frameCount / Time.captureFramerate, Time.frameCount ) );
		
		if( Time.frameCount > numFrames )
		{
			Debug.Log( string.Format( "Capture done, {0} second rendered, {1} total frames.", Time.frameCount / Time.captureFramerate, Time.frameCount ) );
			Application.runInBackground = false;
			EditorApplication.isPlaying = false; 		}
	}
#endif
}