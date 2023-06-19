using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class GunVariables{
	public string name;
	public float gunReloadTime;
	public float gunRecoil;
	public int gunAmmoCount;
	public int gunMaxAmmo;
}
public class GunAimScript : MonoBehaviour {
	public Transform gun;
	public Transform gunSocket;
	public float smoothTime = 5f;
	public float reloadTime = 0.5f;
	public GameObject bulletHole;
	public float recoil = 0.2f;
	public int ammoCount = 5; 
	public int maxAmmoCount = 20;
	public GunVariables[] gunVariables;
	public TextMeshProUGUI gunNameUI;

	public float throwForce;
	public GameObject grenade;

	private Vector3 startPos;
	private Vector3 endPos;
	private float newPos = 0;
	private float coolDown = 0;
	private float timeLeft;
	private LineRenderer lRend;
	private Transform cam;
	private float randX;
	private float randY;
	private float maxRecoilX = -5f;
	private float maxRecoilY = 5f;
	private float recoilSpeed = 10f;
	private string tempGunName = "Pistol";
	private int gunNum = 0;
	private float tempRecoil;

	
	// Use this for initialization
	void Start () {
		startPos = gun.localPosition;
		endPos = new Vector3 (0, -0.2f, 1f);
		lRend = gameObject.GetComponentInChildren<LineRenderer> ();
		lRend.enabled = false;
		timeLeft = reloadTime;
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		GunType ();
		Ray rayCamera = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2 + randX,
			Screen.height/2 + randY,0));
		RaycastHit hitCamera;
		if (Input.GetButton ("Fire2")) {
			randX = Random.Range (12.9f, -12.9f);
			randY = Random.Range (12.1f, -12.1f);
			maxRecoilY = Random.Range (-1, 1);
			newPos = Time.deltaTime * smoothTime;
			gun.transform.localPosition = Vector3.Lerp (gun.localPosition, endPos, newPos);
		} else {
			randX = Random.Range (32.9f, -32.9f);
			randY = Random.Range (32.1f, -32.1f);
			maxRecoilY = Random.Range (-2, 2);
			newPos = Time.deltaTime * smoothTime;
			gun.transform.localPosition = Vector3.Lerp (gun.localPosition, startPos, newPos);
		}
		Recoiling ();
		coolDown -= Time.deltaTime;
		timeLeft -= Time.deltaTime;
		if (Input.GetButton ("Fire1") && timeLeft < 0 && ammoCount > 0) {
			//ammoCount--;
			gunVariables[gunNum].gunAmmoCount--;
			recoil = tempRecoil;
			lRend.enabled = true;
			RaycastHit hit;
			Ray ray = new Ray (gun.position, gun.TransformDirection (Vector3.forward));
			if (Physics.Raycast (ray, out hit, 100f) && Physics.Raycast (rayCamera, out hitCamera, 100f)) {
				lRend.SetPosition (0, gunSocket.position);
				lRend.SetPosition (1, hitCamera.point);
				if (hitCamera.collider.tag == "Enemy") {
					Destroy (hitCamera.collider.gameObject);
				} else {
					Quaternion startRot = Quaternion.LookRotation (hitCamera.normal);
					Instantiate (bulletHole, hitCamera.point, startRot);
				}
			} else {
				lRend.SetPosition (0, gunSocket.position);
				lRend.SetPosition (1, ray.GetPoint(100f));
			}
			timeLeft = reloadTime;
		}
		if (timeLeft <= reloadTime - 0.02f) {
			lRend.enabled = false;
		}
		gunNameUI.text = gunVariables[gunNum].name + " " + ammoCount.ToString() + " / " + maxAmmoCount.ToString();

		if (Input.GetKeyDown(KeyCode.G))
		{
			GameObject newGrenade = Instantiate(grenade, gunSocket.position,
				gunSocket.rotation) as GameObject;
			newGrenade.GetComponent<Rigidbody>().AddForce(transform.forward *throwForce);
		}
	}
	void Recoiling(){
		if (recoil > 0f) {
			Quaternion maxRecoil = Quaternion.Euler (maxRecoilX, maxRecoilY, 0f);
			gun.transform.localRotation = Quaternion.Slerp (gun.transform.localRotation, maxRecoil,
				Time.deltaTime * recoilSpeed);
			recoil -= Time.deltaTime;
		} else {
			recoil = 0f;
			gun.transform.localRotation = Quaternion.Slerp (gun.localRotation, Quaternion.identity,
				Time.deltaTime * recoilSpeed / 2);
		}
	}

	void GunType(){
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			gunNum = 0;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			gunNum = 1;
		}

		reloadTime = gunVariables [gunNum].gunReloadTime;
		tempRecoil = gunVariables [gunNum].gunRecoil;
		ammoCount = gunVariables [gunNum].gunAmmoCount;
		maxAmmoCount = gunVariables [gunNum].gunMaxAmmo;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Ammo") {
			
			if (gunVariables [gunNum].gunAmmoCount < gunVariables [gunNum].gunMaxAmmo) {
				gunVariables [gunNum].gunAmmoCount += 10;
				Destroy (other.gameObject);
				if (gunVariables [gunNum].gunAmmoCount > gunVariables [gunNum].gunMaxAmmo) {
					gunVariables [gunNum].gunAmmoCount = gunVariables [gunNum].gunMaxAmmo;
				}
			} 
		}
	}
}
