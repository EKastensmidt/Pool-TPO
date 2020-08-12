using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BochaBlanca : MonoBehaviour
{
	public float maxForce = 100;
	public float sensitivity = 100;
	public Material lineMaterial;

	private Vector3 click;
	private float cooldown;

	private LineRenderer lineRenderer;
	private _ColliderSphere colliderSphere;
	private new _Rigidbody rigidbody;

	public AudioSource collision;
	public AudioSource collisionMesa;
	public AudioSource palo;
	public AudioSource hole;
	private bool check = true;

    void Start()
    {
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		colliderSphere = GetComponent<_ColliderSphere>();
		rigidbody = GetComponent<_Rigidbody>();

		lineRenderer.startWidth = 0;
		lineRenderer.endWidth = 0.2f;
		lineRenderer.enabled = false;
		lineRenderer.material = lineMaterial;
    }

    void Update()
    {
		cooldown -= Time.deltaTime;
        Plane plane = new Plane(Vector3.up, 0);
		
		float dist;
		Vector3 mPos = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (plane.Raycast(ray, out dist))
		{
			mPos = ray.GetPoint(dist);
		}
		
		if (cooldown < 0 && Input.GetKeyDown(KeyCode.Mouse0)) {
				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.positionCount = 1;
				lineRenderer.enabled = true;
				click = mPos;
		}
		else if (Input.GetKey(KeyCode.Mouse0)) {
				lineRenderer.positionCount = 2;
				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.SetPosition(1, transform.position + (click - mPos).normalized *
										Mathf.Min((click - mPos).magnitude, maxForce/sensitivity));
		}
		else if (Input.GetKeyUp(KeyCode.Mouse0)) {
			float force = Mathf.Min((click - mPos).magnitude  * sensitivity, maxForce);
			Vector3 direction = (click - mPos).normalized;
			direction.y = 0;
			rigidbody.AddForce(direction * force, transform.position);
			
			palo.volume = Mathf.Min(force / 800, 1);
			palo.Play();
			
			lineRenderer.enabled = false;
			cooldown = .2f;
		}

		CheckAgujeros();
	}
	
	private void CheckAgujeros() {
		if (!check) return;
		
		Vector2 pos = new Vector2(transform.position.x, transform.position.z);
		if (Vector2.Distance(pos, new Vector2(-13.3f, 5.8f)) < .4f) {
			StartCoroutine(destroy());
		}
		if (Vector2.Distance(pos, new Vector2(-13.3f, -5.8f)) < .4f) {
			StartCoroutine(destroy());
		}
		if (Vector2.Distance(pos, new Vector2(13.1f, 5.8f)) < .4f) {
			StartCoroutine(destroy());
		}
		if (Vector2.Distance(pos, new Vector2(13.1f, -5.8f)) < .4f) {
			StartCoroutine(destroy());
		}
		if (Vector2.Distance(pos, new Vector2(-.1f, 6)) < .4f) {
			StartCoroutine(destroy());
		}
		if (Vector2.Distance(pos, new Vector2(-.1f, -6)) < .4f) {
			StartCoroutine(destroy());
		}
	}

	private IEnumerator destroy() {
		check = false;
		GetComponent<Renderer>().enabled = false;
		_CollisionManager.getInstance().RemoveCollider(GetComponent<_Collider>());
		transform.position = getPlacePosition(new Vector3(-5, 0.4f, 0), GameObject.FindObjectsOfType<Bocha>());
		rigidbody.Velocity = Vector3.zero;

		hole.Play();

		yield return new WaitForSeconds(1);

		GetComponent<Renderer>().enabled = true;	
		_CollisionManager.getInstance().AddCollider(GetComponent<_Collider>());
		check = true;
	}

	private Vector3 getPlacePosition(Vector3 pos, Bocha[] bochas) {
		bool touching = false;
		foreach (Bocha b in bochas) {
			if (Vector3.Distance(b.transform.position, pos) < .51f) {
				touching = true;
				break;
			}
		}
		if (touching) {
			return getPlacePosition(pos + Vector3.forward*.1f, bochas);
		} else {
			return pos;
		}
	}

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Exit() {
		SceneManager.LoadSceneAsync(0);
	}

	public void BochaCollision(float force) {
		collision.volume = Mathf.Min(force / 1200f, 1);
		collision.Play();
	}
	public void MesaCollision(float force) {
		collisionMesa.volume = Mathf.Min(force / 1600f, 1);
		collisionMesa.Play();
	}
}
