using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class ai : MonoBehaviour
{

	[SerializeField] private GameObject chair;

	private NavMeshAgent _nma;
	private Boolean _chairReached;
	private Vector3 _lastChairPos;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_chairReached = false;
		_nma = GetComponent<NavMeshAgent>();
		_lastChairPos = chair.transform.position + new Vector3(1, 1, 1);
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(transform.position, chair.transform.position) > 2)
			_chairReached = false;
		else
		{
			if (!_chairReached)
				_nma.SetDestination(transform.position);
			_chairReached = true;
		}
		if (!_chairReached && chair.transform.position != _lastChairPos)
		{
			_nma.SetDestination(chair.transform.position);
		}
		Debug.Log(Vector3.Distance(transform.position, chair.transform.position));
		Debug.Log(_nma.destination);
	}


	// private void OnTriggerEnter(Collider objectCollided)
	// {
	// 	if (objectCollided.GameObject() == chair)
	// 	{
	// 		_chairReached = true;
	// 	}
	// }
}
