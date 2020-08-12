using UnityEngine;

public class _ColliderBox : _Collider {

	[SerializeField] private Vector3 boxSize;

    public Vector3 BoxSize { get {
		Vector3 size = boxSize;
		size.Scale(transform.localScale);
		return size;
	} set => boxSize = value; }

    protected override void Awake() {
		base.Awake();
		
		this.setExtent(BoxSize.magnitude / 2);
		this.setType(_ColliderType.Box);
	}

	protected override _Collision DetectCollision(_Collider other) {
		switch (other.Type) {
			case _ColliderType.Box:

				break;
			case _ColliderType.Sphere:
				return _CollisionManager.BoxToSphereCollision(this, (_ColliderSphere) other);
			default:
				Debug.LogWarning("Collider type undetected.");
				break;
		}
		return null;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, BoxSize);
	}
}