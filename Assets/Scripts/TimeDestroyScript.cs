using UnityEngine;
using System.Collections;

public class TimeDestroyScript : MonoBehaviour {
	public float waitTime = 5f;
	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}

}
