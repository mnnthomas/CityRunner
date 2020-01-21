using UnityEngine;
using System.Collections;



public class ObTrack : MonoBehaviour 
{
	/// <summary>
	/// Raises the became invisible event.
	/// Track rearrange is called whenever a track gets hidden from Camera's view
	/// </summary>

	private float		mOffset =  10; 
    public TrackType    _Type;
		
	void Start()
	{
  	}

	void Update()
	{
        if(((transform.position.z + TrackManager.pInstance._TrackSize/2) < TrackManager.pInstance._PlayerCamera.transform.position.z) && gameObject.activeInHierarchy)
		{
			TrackManager.pInstance.RearrangeTrack(gameObject);
			DestroyImmediate(gameObject);
		}
	}

}
