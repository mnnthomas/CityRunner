using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public float     _PlayerStartSpeed = 25f;

    private Rigidbody   mRigidbody;
    private Vector3     mRunningDirection = Vector3.forward;
    private float       mPlayerSpeed;

	void Start () 
    {
        mRigidbody = gameObject.GetComponent<Rigidbody>();
        mPlayerSpeed = _PlayerStartSpeed;
	}
	
    void FixedUpdate () 
    {
        transform.Translate(mRunningDirection  * mPlayerSpeed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider inCol)
    {
        if (inCol.gameObject.name == "TurnRight")
        {
            //transform.Rotate(0, 90f, 0);
            StartCoroutine(Turn(new Vector3(0,90f,0)));
            
        }

        if (inCol.gameObject.name == "TurnLeft")
        {
            //transform.Rotate(0, -90f, 0);
            StartCoroutine(Turn(new Vector3(0,-90f,0)));

        }
    }

    IEnumerator Turn(Vector3 inRot)
    {
        float t = 0;
        float step = 0.1f;

        Vector3 StartRot = transform.eulerAngles;
        Vector3 EndRot = StartRot + inRot;

        Debug.Log("Start Rot & End Rot >>" + StartRot + " > " + EndRot);

        while (t <= 1)
        {
            //transform.eulerAngles = Vector3.Lerp(StartRot, EndRot, t);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(StartRot), Quaternion.Euler(EndRot), t);
            t += step;

            yield return new WaitForSeconds(0.01f);
        }

        transform.eulerAngles = EndRot;
    }
}
