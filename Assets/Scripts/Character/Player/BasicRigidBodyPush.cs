using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	public LayerMask pushLayers;
	
	public bool canPush;
	[Range(0.5f, 5f)] 
	public float strength = 1.1f;

	/// <summary>
	/// 다른 오브젝트와 부딪쳤을 때 
	/// </summary>
	/// <param name="hit"></param>
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (canPush == true)
		{
			PushRigidBodies(hit);
		}
	}

	private void PushRigidBodies(ControllerColliderHit hit)
	{
		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
		{
			return;
		}

		// pushLayers 일때만
		int bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0)		
		{
			return;
		}

		//아래의 있는 오브젝트는 제외
		if (hit.moveDirection.y < -0.3f)
		{
			return;
		}

		//밀리는 방향
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
		body.AddForce(pushDir * strength, ForceMode.Impulse);
	}
}