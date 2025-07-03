using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class ai : MonoBehaviour
{
	private static readonly int IsWalking = Animator.StringToHash("IsWalking");
	private static readonly int Idle = Animator.StringToHash("Idle");

	[SerializeField] private GameObject chair;
	 private Animator 			_characterAnimator;

	
	private NavMeshAgent _nma;
	private Boolean _chairReached;
	private Vector3 _lastChairPos;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_chairReached = false;
		_nma = GetComponent<NavMeshAgent>();
		_lastChairPos = chair.transform.position + new Vector3(1, 1, 1);
		_characterAnimator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(transform.position, chair.transform.position) > 3.5) {
			_chairReached = false;
			_characterAnimator.SetBool(IsWalking, true);
		}
		else
		{
			if (!_chairReached) {
				_nma.SetDestination(transform.position);
			}
			_chairReached = true;
			_characterAnimator.SetBool(IsWalking, false);
			_characterAnimator.SetTrigger(Idle);
		}
		if (!_chairReached && chair.transform.position != _lastChairPos)
		{
			_nma.SetDestination(chair.transform.position);
		}
		// Debug.Log(Vector3.Distance(transform.position, chair.transform.position));
		// Debug.Log(_nma.destination);
	}


	// private void OnTriggerEnter(Collider objectCollided)
	// {
	// 	if (objectCollided.GameObject() == chair)
	// 	{
	// 		_chairReached = true;
	// 	}
	// }
}
