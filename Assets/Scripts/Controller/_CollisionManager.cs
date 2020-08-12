using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CollisionManager : MonoBehaviour
{
	// --|-- Variables --|--

	private List<_Collider> colliders = new List<_Collider>();


	// --|-- Metodos --|--

	// ---> Unity
	private void FixedUpdate() {
		foreach (_Collider c in colliders)
		{
			c.LateFixedUpdate();
		}
	}

	// ---> Public

	public void AddCollider(_Collider collider) {
		colliders.Add(collider);
	}
	public void RemoveCollider(_Collider collider) {
		colliders.Remove(collider);
	}

	public _Collider[] getFrontierColliders(_Collider c) {
		List<_Collider> cols = new List<_Collider>();

		foreach (_Collider other in colliders) {
			if (c.Equals(other))
				continue;

			float distance = (c.Transform.position - other.Transform.position).magnitude;

			if (distance < c.Extent + other.Extent) {
				cols.Add(other);
			}
		}

		return cols.ToArray();
	}

	// --|-- Estáticos --|--

	// ---> getInstance

	/// <summary>
	/// La instancia se encarga de administrar todos los colliders existentes en la escena.
	/// </summary>
	/// <returns>La instancia existente en la escena</returns>
	public static _CollisionManager getInstance() {
		GameObject go = GameObject.FindGameObjectWithTag("CollisionManager");
		if (go == null) {
			Debug.LogError("CollisionManager object not found.");
		} else {
			return go.GetComponent<_CollisionManager>();
		}
		return null;
	}

	// ---> Collision Detection

	public static _Collision SphereToSphereCollision(_ColliderSphere me, _ColliderSphere other) {
		Vector3 separation = me.Transform.position - other.Transform.position;
		float collisionDistance = me.Radius + other.Radius;

		if (separation.magnitude <= collisionDistance)
		{
			Vector3 extentPointMe = Vector3.MoveTowards(me.Transform.position, other.Transform.position, me.Extent);
			Vector3 extentPointOther = Vector3.MoveTowards(other.Transform.position, me.Transform.position, other.Extent);

			Vector3 collisionPoint = (extentPointMe + extentPointOther) / 2;
			Vector3 normalUnit = separation.normalized;

			return new _Collision(collisionPoint, (extentPointMe - extentPointOther).magnitude, normalUnit, other);
		}
		else
		{
			return null;
		}
	}
		public static _Collision BoxToSphereCollision(_ColliderBox me, _ColliderSphere other) {
			return BoxToSphereCollision(me, other, true);
		}
		public static _Collision SphereToBoxCollision(_ColliderSphere me, _ColliderBox other) {
			return BoxToSphereCollision(other, me, false);
		}
		private static _Collision BoxToSphereCollision(_ColliderBox box, _ColliderSphere sphere, bool boxMe) {
		/** 
		* Se utiliza Inverse Transform para transformar todas las posiciones y
		* direcciones a un sistema de coordenadas local donde es más sencillo
		* calcular los choques en una dimensión
		*/

		Vector3 bPos = box.Transform.position;
		Vector3 sPos = InverseTransformPointUnscaled(box.Transform, sphere.Transform.position);
		
		// Si no está fuera de un borde con el cual chequear significa que está adentro, por lo que
		// esa coordenada no se utilizará para el calculo.
		Vector3 testPoint = sPos;

		if (sPos.x < -box.BoxSize.x / 2)		testPoint.x = -box.BoxSize.x / 2;
		else if (sPos.x > box.BoxSize.x / 2) testPoint.x = box.BoxSize.x / 2;
		if (sPos.y < -box.BoxSize.y / 2)		testPoint.y = -box.BoxSize.y / 2;
		else if (sPos.y > box.BoxSize.y / 2) testPoint.y = box.BoxSize.y / 2;
		if (sPos.z < -box.BoxSize.z / 2)		testPoint.z = -box.BoxSize.z / 2;
		else if (sPos.z > box.BoxSize.z / 2) testPoint.z = box.BoxSize.z / 2;
		
		float distance = (sPos - testPoint).magnitude;

		if (distance <= sphere.Radius) {
			Vector3 extentPointBox = testPoint;
			Vector3 extentPointSphere = sPos + (testPoint - sPos).normalized * sphere.Extent;

			Vector3 collisionPoint = TransformPointUnscaled(box.Transform, (extentPointBox + extentPointSphere) / 2);
			
			Vector3 normalUnit = box.Transform.TransformDirection(sPos - testPoint).normalized;

			return new _Collision(collisionPoint, (extentPointBox - extentPointSphere).magnitude, normalUnit, boxMe ? (_Collider) sphere : (_Collider) box);
		} else {
			return null;
		}
	}

	public static Vector3 TransformPointUnscaled(Transform transform, Vector3 position)
	{
		var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		return localToWorldMatrix.MultiplyPoint3x4(position);
	}

	public static Vector3 InverseTransformPointUnscaled(Transform transform, Vector3 position)
	{
		var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
		return worldToLocalMatrix.MultiplyPoint3x4(position);
	}
}
