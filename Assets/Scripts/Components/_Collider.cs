using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Collider : MonoBehaviour
{
	// --|-- Variables --|--

	// ---> private 	
	private float extent;
	private _Rigidbody rigidBody;

	// ---> protected	
	protected _ColliderType type;

	
	// --|-- Metodos --|--

	// --> Unity
	protected virtual void Awake() {
		rigidBody = GetComponent<_Rigidbody>();

		_CollisionManager.getInstance().AddCollider(this);
	}

	protected virtual void FixedUpdate() {
		_Collider[] list = _CollisionManager.getInstance().getFrontierColliders(this);

		DetectCollisions(list);
	}

	public void LateFixedUpdate() {
		if (rigidBody != null && rigidBody.enabled) {
			rigidBody.LateFixedUpdate();
		}
	}

	// --> Privados
	private void DetectCollisions(_Collider[] list)
	{
		foreach(_Collider other in list) {
			_Collision collision = DetectCollision(other);
			if (collision != null) {
				OnCollision(collision);
			}
		}
	}

	private void OnCollision(_Collision collision) {
		if (rigidBody != null) {
			rigidBody.Collide(collision);
		}
	}

	// --> Protegidos
	protected virtual _Collision DetectCollision(_Collider other)
	{
		Debug.LogWarning("El metodo de detección de colisión no fue sobreescrito.");
		return null;
	}

	//Setters
	protected void setExtent(float extent) {
		this.extent = extent;
	}

	protected void setType(_ColliderType type) {
		this.type = type;
	}
	// --> Publicos

	// Getters
	public float Extent { get => extent; }
	public Transform Transform { get => transform; }
	public _ColliderType Type { get => type; }
}
