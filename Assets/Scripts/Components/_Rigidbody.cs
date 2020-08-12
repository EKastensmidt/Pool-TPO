using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Rigidbody : MonoBehaviour
{
	[SerializeField] private float mass = 1;
	[SerializeField] private float inertiaMoment; //2/5 M(R)^2

	[SerializeField] private float cfInelasticidad = 1;
	[SerializeField] private float cfRozamiento = 1;

	private List<Force> forces = new List<Force>();
	private class Force {
		public Vector3 force;
		public Vector3 point;

		public Force(Vector3 force, Vector3 point) {
			this.force = force;
			this.point = point;
		}
	}

	private Vector3 acceleration;
	private Vector3 velocity;
	
	private List<Vector3> torques;
	private Vector3 angularAcceleration;
	private Vector3 angularVelocity;

	private bool lateUpdate;
	private Vector3 latePosition;

	private void FixedUpdate()
	{	
		float rozamiento = cfRozamiento * mass * Physics.gravity.magnitude;
		AddForce(-velocity.normalized * rozamiento, transform.position);

		UpdateAcceleration();
		UpdatePosition();
		UpdateSpeed();

		forces.Clear();
	}

	public void LateFixedUpdate() {
		if (lateUpdate) {
			transform.position = latePosition;
		}
		lateUpdate = false;
	}

	// a = F / m
	void UpdateAcceleration() {
		Vector3 F = Vector3.zero;
		Vector3 T = Vector3.zero;

		foreach (Force f in forces) {
			F += f.force;
			
			Vector3 AB = (f.point + f.force) - f.point;
			Vector3 AC = transform.position - f.point;

			Vector3 proyection = f.point + Vector3.Project(AC, AB);
			Vector3 r = proyection - transform.position;

			Vector3 t = Vector3.Cross(r, f.force);
			T += t;
		}
		
		acceleration = F / mass;
		angularAcceleration = T / inertiaMoment;
	}

	// v(t) = vi + a * dt
	void UpdateSpeed() {
		velocity += acceleration * Time.fixedDeltaTime;
		if (velocity.magnitude < 0.05f) {
			velocity = Vector3.zero;
		}
		if (GetComponent<_ColliderSphere>() != null) {
			//Para esferas
			angularVelocity.x = velocity.z / GetComponent<_ColliderSphere>().Radius * Mathf.Rad2Deg;
			angularVelocity.y = 0;
			angularVelocity.z = -velocity.x / GetComponent<_ColliderSphere>().Radius * Mathf.Rad2Deg;
			// print(angularVelocity);
		} else {
			//Para el resto
			angularVelocity += angularAcceleration * Time.fixedDeltaTime;
			if (angularVelocity.magnitude < 0.05f) {
				angularVelocity = Vector3.zero;
			}
			if (angularAcceleration == Vector3.zero) {
				angularVelocity *= 0.9f;
			}
		}
	}

	// p(t) = pi + v * dt
	void UpdatePosition() {
		transform.position += velocity * Time.fixedDeltaTime + .5f*acceleration*Mathf.Pow(Time.fixedDeltaTime,2);
//		print(angularVelocity);
		if (angularVelocity.magnitude > 0)
			transform.Rotate(angularVelocity * Time.fixedDeltaTime  + .5f*angularAcceleration*Mathf.Pow(Time.fixedDeltaTime,2), Space.World);
	}


	// ---> Public

	public void AddForce(Vector3 force, Vector3 point) {
		
		//Alternativamente usemos Vector3.project
		// Vector3 AP = transform.position - point;

		// Vector3 B = point + force;
		// Vector3 AB = B - point;

		// Vector3 proyection = point + Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB) * AB;
		// Vector3 r = proyection - transform.position;

		// //t = r x F
		// Vector3 torque = Vector3.Cross(r, force);
		
		Force f = new Force(force, point);
		forces.Add(f);
	}

	public void Collide(_Collision collision) {
		Vector3 nUnit = collision.NormalUnit;
		Vector3 tUnit1 = Vector3.Cross(nUnit, Vector3.up);
		Vector3 tUnit2 = Vector3.Cross(nUnit, tUnit1);

		float vn = Vector3.Dot(nUnit, velocity);
		float vt1 = Vector3.Dot(tUnit1, velocity);
		float vt2 = Vector3.Dot(tUnit2, velocity);
		
		float v2n;
		float m2;
		if (collision.Rigidbody != null) {
			v2n = Vector3.Dot(nUnit, collision.Rigidbody.velocity);
			m2 = collision.Rigidbody.Mass;
		} else {
			v2n = -vn;
			m2 = mass;
		}

		float vFn = (vn * (mass - m2) + 2 * m2 * v2n) / (mass + m2);

		Vector3 variation;
		if (collision.Rigidbody != null) {
			variation = (transform.position - collision.CollisionPoint).normalized * (collision.CollisionRange + .01f) *
						.5f; //vn / (vn+v2n);
		} else {
			variation = (transform.position - collision.CollisionPoint).normalized *
						(collision.CollisionRange + .01f);
		}

		latePosition = transform.position + variation;
		latePosition.y = 0;
		Vector3 lateVelocity = nUnit * vFn + tUnit1 * vt1 + tUnit2 * vt2;
		lateVelocity.y = 0;

		Vector3 velDif = (lateVelocity - velocity);
		Vector3 acc = velDif / Time.fixedDeltaTime;
		Vector3 force = acc * Mass;
		AddForce(force * cfInelasticidad, collision.CollisionPoint);

		lateUpdate = true;

		//Todo esto es para llamar las funciones de sonido (solo las bochas usan)
		if (gameObject.tag != "Bocha") return;

		if (collision.GameObject.tag == "Bocha") {
			SendMessage("BochaCollision", force.magnitude);
		} else if (collision.GameObject.tag == "Pared") {
			SendMessage("MesaCollision", force.magnitude);
		}
	}

	//Getters
	public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float Mass { get => mass; set => mass = value; }
}
