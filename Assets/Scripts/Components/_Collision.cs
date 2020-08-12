using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Collision
{
	private Vector3 collisionPoint;
	private float collisionRange;
	private Vector3 normalUnit;

	private GameObject gameObject;
	private Transform transform;
	private _Rigidbody rigidbody;
	private _Collider collider;

	public _Collision(Vector3 collisionPoint, float collisionRange, Vector3 normalUnit, _Collider other) {
		this.collisionPoint = collisionPoint;
		this.collisionRange = collisionRange;
		this.normalUnit = normalUnit;

		collider = other;
		gameObject = other.gameObject;
		transform = other.transform;
		rigidbody = other.GetComponent<_Rigidbody>();
		if (rigidbody != null && !rigidbody.enabled) rigidbody = null;
	}

    public Vector3 CollisionPoint { get => collisionPoint; }
    public Vector3 NormalUnit { get => normalUnit; }
    public GameObject GameObject { get => gameObject;}
    public Transform Transform { get => transform;}
    public _Rigidbody Rigidbody { get => rigidbody;}
    public _Collider Collider { get => collider;}
	public float CollisionRange { get => collisionRange; }
}
