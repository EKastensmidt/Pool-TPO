using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bocha : MonoBehaviour
{
    void FixedUpdate()
    {
        CheckAgujeros();
    }

	public GameObject ballImage;
	public AudioSource collision;
	public AudioSource collisionMesa;
	public AudioSource hole;
	private bool check = true;

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
		ballImage.SetActive(true);
		GetComponent<Renderer>().enabled = false;
		_CollisionManager.getInstance().RemoveCollider(GetComponent<_Collider>());
		hole.Play();

		yield return new WaitForSeconds(1);
		Destroy(gameObject);
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
