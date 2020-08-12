using UnityEngine;

public class _ColliderSphere : _Collider {

	[SerializeField] private float radius;

    public float Radius { get => radius; set => radius = value; }

    protected override void Awake() {
		base.Awake();

		this.setExtent(radius);
		this.setType(_ColliderType.Sphere);
	}

	protected override _Collision DetectCollision(_Collider other) {
		switch (other.Type) {
			case _ColliderType.Box:
				return _CollisionManager.SphereToBoxCollision(this, (_ColliderBox) other);
			case _ColliderType.Sphere:
				return _CollisionManager.SphereToSphereCollision(this, (_ColliderSphere) other);
			default:
				Debug.LogWarning("Collider type undetected.");
				break;
		}
		return null;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, Radius);
	}
}