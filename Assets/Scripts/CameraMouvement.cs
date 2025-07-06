using UnityEngine;

public class CameraMouvement : MonoBehaviour
{

	[SerializeField] private GameObject LookAt;

	private Vector3 _lookAtPosition;
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_lookAtPosition = LookAt.transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		LookAt.transform.position = _lookAtPosition + transform.forward.normalized;
		// this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - Dist);
	}
}
