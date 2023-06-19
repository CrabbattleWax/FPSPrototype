using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {
	

	public float timeToDetonate = 2f;
	public float explosionForce;
	public float explostionRadius;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeToDetonate -= Time.deltaTime;
		if(timeToDetonate < 0)
        {
			Explode();
			Destroy(gameObject);
        }
	}
	void Explode()
    {
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explostionRadius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(explosionForce, explosionPos, explostionRadius, 500.0F);
		}
	}
}
