using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TrackType
{
    STRAIGHT,
    STRAIGHT_RIGHT,
    STRAIGHT_LEFT,
    RIGHT_STRAIGHT,
    LEFT_STRAIGHT,
    TRANSITION
};

public enum TrackTheme
{
    CITY,
    FARM
};

[System.Serializable]
public class Themes
{
    public TrackTheme _ThemeName;
    public List<TrackSet> _TrackTypeSets = new List<TrackSet>();
}

[System.Serializable]
public class TrackSet
{
    public TrackType _Type;
    public List<GameObject> _Tracks = new List<GameObject>(); 
}


public class TrackManager : MonoBehaviour {
	///<summary>
	/// This script allows endless track creation for runner based games.
	/// Track creation is forced to happen in single direction.
	/// This class will hold a single static instance for the scene 
	/// </summary>
	
	//Singleton instance
	private static TrackManager			mInstance = null;
	public static TrackManager 			pInstance
	{
		get{return mInstance;}
	}

	public List<Themes>                 _Themes = new List<Themes>();
	public Vector3					    _TrackStartPos = Vector3.zero; // Initial track position
    public float                        _TrackSize; //Track should be a square
	public Camera					    _PlayerCamera;  // The camera that needs to be considered for track creation
	public int					        _ActiveTrackCount;  // Defines the max number of active tracks in the scene at a given time
	public GameObject					_FirstTrack;

	
	//List to hold active and inactive tracks in the scene.
	private List<GameObject>			mActiveTrackList = new List<GameObject>();
	private TrackTheme 					mCurTheme;
    private bool                        isChangingTheme = false;
    private TrackTheme                  mChangingTheme;
	
	void Awake()
	{
		//Creating a single instance
        if(mInstance == null)
		    mInstance = this;
	}
	
	public virtual void Start()
	{
        mCurTheme = TrackTheme.CITY;
		ArrangeTracks();
	}
	
	/// <summary>
	/// Finds a random track from list.
	/// </summary>
	/// <returns>The rand track from list.</returns>
    public T GetNextTrackFromList<T>(List<T> inList, int inIndex = 0)
	{
		return inList[Random.Range(0, inList.Count)];
	}

    public void SetTheme(int inThemeNum)
    {
        SetTheme((TrackTheme)inThemeNum);
    }

	public void SetTheme(TrackTheme inTheme)
	{
        if (!isChangingTheme && mCurTheme != inTheme)
        {
            isChangingTheme = true;
            mChangingTheme = inTheme;
        }
	}
 

	public GameObject GetNextTrackFromTheme(TrackTheme inTheme)
    {
		foreach (Themes themes in _Themes) 
		{
			if (themes._ThemeName == inTheme) 
			{
				TrackType lasttracktype = mActiveTrackList [mActiveTrackList.Count - 1].GetComponent<ObTrack> ()._Type;
				List<GameObject> tempList = new List<GameObject>();

				switch (lasttracktype)
				{
				case TrackType.STRAIGHT:
				case TrackType.LEFT_STRAIGHT:
				case TrackType.RIGHT_STRAIGHT:
                case TrackType.TRANSITION:
					for (int i = 0; i < themes._TrackTypeSets.Count; i++) 
					{
                            if (!isChangingTheme)
                            {
                                if (themes._TrackTypeSets[i]._Type == TrackType.STRAIGHT || themes._TrackTypeSets[i]._Type == TrackType.STRAIGHT_RIGHT || themes._TrackTypeSets[i]._Type == TrackType.STRAIGHT_LEFT)
                                {
                                    for (int j = 0; j < themes._TrackTypeSets[i]._Tracks.Count; j++)
                                    {
                                        tempList.Add(themes._TrackTypeSets[i]._Tracks[j]);
                                    }
                                }
                            }
                            else
                            {
                                if (themes._TrackTypeSets[i]._Type == TrackType.TRANSITION)
                                {
                                    for (int j = 0; j < themes._TrackTypeSets[i]._Tracks.Count; j++)
                                    {                                      
                                        tempList.Add(themes._TrackTypeSets[i]._Tracks[j]);
                                    }

                                    isChangingTheme = false;
                                    mCurTheme = mChangingTheme;
                                }

                            }
					}
					break;

				case TrackType.STRAIGHT_RIGHT:
					for (int i = 0; i < themes._TrackTypeSets.Count; i++) 
					{
						if (themes._TrackTypeSets [i]._Type == TrackType.LEFT_STRAIGHT) 
						{
							for (int j = 0; j < themes._TrackTypeSets [i]._Tracks.Count; j++) 
							{
								tempList.Add (themes._TrackTypeSets [i]._Tracks [j]);
							}
						}
					}
					break;

				case TrackType.STRAIGHT_LEFT:
					for (int i = 0; i < themes._TrackTypeSets.Count; i++) 
					{
						if (themes._TrackTypeSets [i]._Type == TrackType.RIGHT_STRAIGHT) 
						{
							for (int j = 0; j < themes._TrackTypeSets [i]._Tracks.Count; j++) 
							{
								tempList.Add (themes._TrackTypeSets [i]._Tracks [j]);
							}
						}
					}
					break;
				}
				return tempList [Random.Range (0, tempList.Count)];
			}
		}
		return null;
    }
 
	
	/// <summary>
	/// Finds the next track position.
	/// </summary>
	/// <returns>The next track's Vector3 position.</returns>   
	public Vector3 FindNextTrackPosition(int inTrackCount)
	{
		if(inTrackCount != 0)
		{
            if (mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.STRAIGHT || mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.RIGHT_STRAIGHT ||
                mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.LEFT_STRAIGHT ||  mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.TRANSITION)
                return mActiveTrackList[inTrackCount - 1].transform.position + (Vector3.forward * _TrackSize);
            else if (mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.STRAIGHT_RIGHT)
                return mActiveTrackList[inTrackCount - 1].transform.position + (Vector3.right * _TrackSize);
            else if (mActiveTrackList[inTrackCount - 1].GetComponent<ObTrack>()._Type == TrackType.STRAIGHT_LEFT)
                return mActiveTrackList[inTrackCount - 1].transform.position + (Vector3.left * _TrackSize);
            else  
                return Vector3.zero;
        
		}
		else
			return _TrackStartPos;
	}
	
	/// <summary>
	/// Initial track arrangement function
	/// This is called on the start of the class.
	/// </summary>
	public virtual void ArrangeTracks()
	{
		for(int i = 0; i < _ActiveTrackCount; i++)
		{  
			if(mActiveTrackList.Count == 0)
			{
				GameObject newGo = GameObject.Instantiate(_FirstTrack) as GameObject;
				mActiveTrackList.Add(newGo);
				mActiveTrackList[i].SetActive(true);
				mActiveTrackList[i].transform.position = _TrackStartPos;
			}
			else
			{
                GameObject newGo = GameObject.Instantiate(GetNextTrackFromTheme(mCurTheme)) as GameObject;
				mActiveTrackList.Add(newGo);
				mActiveTrackList[i].SetActive(true);
				mActiveTrackList[i].transform.position = FindNextTrackPosition(i);
			}
		}
	}

	/// <summary>
	/// Rearranges track whenever needed and brings a new track to the front 
	/// </summary>
	public virtual void RearrangeTrack(GameObject inTrack)
	{
		//if(_Tracks.Count > 0 )
		//{
			GameObject tempTrack = inTrack;
			mActiveTrackList.Remove (tempTrack);

            GameObject newGo = GameObject.Instantiate(GetNextTrackFromTheme(mCurTheme)) as GameObject;
			mActiveTrackList.Add(newGo);
			mActiveTrackList[mActiveTrackList.Count -1].transform.position = FindNextTrackPosition(mActiveTrackList.Count -1);
			mActiveTrackList[mActiveTrackList.Count -1].SetActive(true);
		//}
		//else
		//{
		//	mActiveTrackList[0].transform.position = FindNextTrackPosition(mActiveTrackList.Count -1);
		//	mActiveTrackList.Add(mActiveTrackList[0]);
		//	mActiveTrackList.RemoveAt(0);
		//}
		
	}
	
	void Destroy()
	{
		mInstance = null;
	}
	
}
