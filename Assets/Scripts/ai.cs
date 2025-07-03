using System;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using static Points;

public class ai : MonoBehaviour
{
	private static readonly int IsWalking = Animator.StringToHash("IsWalking");
	private static readonly int Idle = Animator.StringToHash("Idle");
	private static readonly int Sit = Animator.StringToHash("sit");

	private struct Increment { public float min, max, time, delta; };

	[SerializeField] private GameObject chair;
	 private Animator 			_characterAnimator;

	
	private NavMeshAgent _nma;
	private Boolean _chairReached;
	private Boolean _onDesk;
	private Vector3 _lastTargetPos;
	private Increment _madnessIncr;
	public float _madness;
	public string _state;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_chairReached = false;
		_onDesk = false;
		_nma = GetComponent<NavMeshAgent>();
		_characterAnimator = GetComponent<Animator>();
		_lastTargetPos = chair.transform.position + new Vector3(1, 1, 1);
		_madnessIncr = new Increment { min = 0, max = 0.5f, time = 0, delta = 2 };
		_madness = 0;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		_madness += HandleMadnessIncr();

		if (_madness <= 20)
			NormalBehaviour();
		else if (_madness <= 45)
			UnBotheredBehaviour();
		else
		{
			_onDesk = false;
			_chairReached = false;
			if (_madness <= 70)
				DerangedBehaviour();
			else if (_madness <= 105)
				CrazyBehaviour();
			else if (_madness <= 140)
				InsaneBehaviour();
		}
	}

	private float HandleMadnessIncr()
	{
		_madnessIncr.time += Time.fixedDeltaTime;
		if (_madnessIncr.time >= _madnessIncr.delta)
		{
			_madnessIncr.time = 0;
			return (UnityEngine.Random.Range(_madnessIncr.min, _madnessIncr.max));
		}
		return (0);
	}

	private void NormalBehaviour()
	{
		_state = "Normal";
		_madnessIncr = new Increment { min = 0.2f, max = 0.5f, time = _madnessIncr.time, delta = 2 };
		if (Vector3.Distance(transform.position, chair.transform.position) > 2f) {
			_chairReached = false;
			_characterAnimator.SetBool(IsWalking, true);
		}
		else
		{
			if (!_chairReached)
				_nma.SetDestination(transform.position);
			_chairReached = true;
			_characterAnimator.SetBool(IsWalking, false);
			_characterAnimator.SetTrigger(Idle);
			Points._money += 1 * Time.fixedDeltaTime;
			//# get on desk
			//# startwork
		}
		if (!_chairReached && chair.transform.position != _lastTargetPos)
		{
			_nma.SetDestination(chair.transform.position);
			if (!_onDesk)
			{
				_onDesk = true;
				_characterAnimator.SetTrigger(Sit);
			}
		}
		// Debug.Log(Vector3.Distance(transform.position, chair.transform.position));
		// Debug.Log(_nma.destination);
	}

	private void UnBotheredBehaviour()
	{
		_state = "UnBothered";
		_madnessIncr = new Increment { min = 0.3f, max = 0.75f, time = _madnessIncr.time, delta = 2 };
		//#stopwork
		//#on desk animations
	}
	private void DerangedBehaviour()
	{
		_state = "Deranged";
		_madnessIncr = new Increment { min = 0.5f, max = 1, time = _madnessIncr.time, delta = 1.5f };
		_onDesk = false;
		_chairReached = false;
		//# get out of desk
		//# starts walking around

	}
	private void CrazyBehaviour()
	{
		_state = "Crazy";
		_madnessIncr = new Increment { min = 0.75f, max = 1.25f, time = _madnessIncr.time, delta = 1.5f };
		//# starts running around
		_onDesk = false;
		_chairReached = false;

	}
	private void InsaneBehaviour()
	{
		_state = "Insane";
		_madnessIncr = new Increment { min = 1, max = 1.5f, time = _madnessIncr.time, delta = 1.5f };
		//# starts tweaking
		_onDesk = false;
		_chairReached = false;

	}
}
