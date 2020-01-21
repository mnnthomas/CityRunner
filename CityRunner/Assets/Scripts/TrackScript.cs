using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour {

public GameObject[]	_CoinPatterns;
private	GameObject	mCoin;

	void Start ()
	{		
	}

	void OnEnable ()
	{
		if (_CoinPatterns.Length > 0) {
			int rand = Random.Range (0, _CoinPatterns.Length);
			mCoin = GameObject.Instantiate (_CoinPatterns [rand]) as GameObject;
			mCoin.transform.localPosition = gameObject.transform.localPosition - new Vector3 (0, 0, 25f);
			mCoin.transform.parent = gameObject.transform;
		}
	}

	void OnDisable()
	{
		GameObject.Destroy(mCoin);
	}
	
	void Update () 
	{
	
	}
}
