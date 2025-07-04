using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Splines;
using static Points;

public class ai : MonoBehaviour
{
	private static readonly int IsWalking = Animator.StringToHash("IsWalking");
	private static readonly int Idle = Animator.StringToHash("Idle");
	private static readonly int Sit = Animator.StringToHash("Sit");
	private static readonly int IsDancing = Animator.StringToHash("IsDancing");
	private static readonly int IsRunning = Animator.StringToHash("IsRunning");

	[SerializeField] private Vector3 _offset = new Vector3(0, 5, 0);
	[SerializeField] private Vector3 _textOffset = new Vector3(0, 5.5f, 0);
	[SerializeField] private Vector2 size = new Vector2(60, 10);
	[SerializeField] private List<Transform> _waypoints;
	[SerializeField] private float _speed = 5f;
	[SerializeField] private float _runSpeed = 10f;
	
	private Camera _camera;
	private struct Increment {
		public float min, max, time, delta, nextStep;
		public Action func;
	};

	[SerializeField] private GameObject chair;
	[SerializeField] private GameObject table;
	 private Animator 			_characterAnimator;

	[SerializeField] private Texture moneyUp;
	[SerializeField] private Texture moneyDown;
	[SerializeField] private Texture youtube;
	private NavMeshAgent _nma;
	private Boolean _chairReached;
	private Boolean _onDesk;
	private Vector3 _targetPos;
	private Dictionary<string, Increment> _madnessIncrements = new Dictionary<string, Increment>();
	private Increment _madnessIncr;
	private MeshRenderer _tableMesh;
	public float _madness;
	public string _state;

	public float GetMaxMadness()
	{
		return _madnessIncrements["Insane"].nextStep;
	}
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_chairReached = false;
		_onDesk = false;
		_nma = GetComponent<NavMeshAgent>();
		_characterAnimator = GetComponent<Animator>();
		_targetPos = chair.transform.position + new Vector3(1, 1, 1);
		_madnessIncrements.AddRange(new Dictionary<string, Increment>
		{
			{ "Normal", new Increment { min = 0.5f, max = 1f, time = 0, delta = 1, nextStep = 10, func = NormalBehaviour } },
			{ "UnBothered", new Increment { min = 0.75f, max = 1.5f, time = 0, delta = 1, nextStep = 20, func = UnBotheredBehaviour } },
			{ "Deranged", new Increment { min = 1.5f, max = 3f, time = 0, delta = .75f, nextStep = 45, func = DerangedBehaviour } },
			{ "Crazy", new Increment { min = 1.25f, max = 2.5f, time = 0, delta = .5f, nextStep = 100, func = CrazyBehaviour } },
			{ "Insane", new Increment { min = 2f, max = 3.5f, time = 0, delta = .35f, nextStep = 140, func = InsaneBehaviour } }
		});
		_madnessIncr = _madnessIncrements["Normal"];
		_madness = 0;
		_camera = Camera.main;
		_tableMesh = table.transform.Find("computer/screen/cuboid_1")?.GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		_madness += HandleMadnessIncr();
		_madness = Mathf.Clamp(_madness, 0, _madnessIncrements["Insane"].nextStep);

		for (var i = 0; i < _madnessIncrements.Count; i++) {
			var madnessIncr = _madnessIncrements.ElementAt(i);
			if (_madness < madnessIncr.Value.nextStep || i == _madnessIncrements.Count - 1) {
				if (i >= 2) {
					_onDesk = false;
					_chairReached = false;
				}
				if (madnessIncr.Value.func != _madnessIncr.func)
					_madnessIncr = madnessIncr.Value;
				_madnessIncr.func.Invoke();
				break ;
			}
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

	private void Work() {
		_chairReached = true;
		_characterAnimator.SetBool(IsWalking, false);
		_characterAnimator.SetTrigger(Idle);
		Points._money += 1 * Time.fixedDeltaTime;
	}
	
	private void NormalBehaviour()
	{
		_state = "Normal";
		_tableMesh.material.SetTexture("_BaseMap", moneyUp);
		_characterAnimator.SetBool(IsDancing, false);
		_characterAnimator.SetBool(IsRunning, false);
		_nma.speed = _speed;
		_nma.acceleration = _speed * 2;
		if (Vector3.Distance(transform.position, chair.transform.position) > 2f) {
			_chairReached = false;
			_characterAnimator.SetBool(IsWalking, true);
		}
		else
		{
			if (!_chairReached)
				_nma.SetDestination(transform.position);
			Work();
			//# get on desk
			//# startwork
		}
		if (!_chairReached && chair.transform.position != _nma.destination)
		{
			_nma.SetDestination(chair.transform.position);
		}
	}

	private void UnBotheredBehaviour()
	{
		_tableMesh.material.SetTexture("_BaseMap", youtube);
		_state = "UnBothered";
		_characterAnimator.SetBool(IsDancing, false);
		_characterAnimator.SetBool(IsRunning, false);
		_nma.speed = _speed;
		_nma.acceleration = _speed * 2;
		//#stopwork
		//#on desk animations
	}
	private void DerangedBehaviour()
	{
		_tableMesh.material.SetTexture("_BaseMap", moneyDown);
		_state = "Deranged";
		_characterAnimator.SetBool(IsDancing, false);
		_characterAnimator.SetBool(IsRunning, false);
		_nma.speed = _speed;
		_nma.acceleration = _speed * 2;
		_onDesk = false;
		_chairReached = false;
		//# get out of desk
		//# starts walking around
		if (Vector3.Distance(_nma.destination, transform.position) < 2)
		{
			_characterAnimator.SetBool(IsWalking, true);
			Vector3 newPos = _waypoints[UnityEngine.Random.Range(0, _waypoints.Count() - 1)].position;
			_nma.SetDestination(newPos);
		}
	}
	private void CrazyBehaviour()
	{
		_state = "Crazy";
		_characterAnimator.SetBool(IsDancing, true);
		_characterAnimator.SetBool(IsRunning, false);
		_nma.speed = _speed;
		_nma.acceleration = _speed * 2;
		_characterAnimator.SetInteger("DanceType", UnityEngine.Random.Range(1, 6));
		_onDesk = false;
		_chairReached = false;
		if (Vector3.Distance(_nma.destination, transform.position) < 2)
		{
			_characterAnimator.SetBool(IsWalking, true);
			Vector3 newPos = _waypoints[UnityEngine.Random.Range(0, _waypoints.Count() - 1)].position;
			_nma.SetDestination(newPos);
		}
	}
	private void InsaneBehaviour()
	{
		_state = "Insane";
		//# starts tweaking
		_characterAnimator.SetBool(IsDancing, false);
		_characterAnimator.SetBool(IsRunning, true);
			_nma.speed = _runSpeed;
			_nma.acceleration = _runSpeed * 2;
		_onDesk = false;
		_chairReached = false;
		if (Vector3.Distance(_nma.destination, transform.position) < 2)
		{
			_characterAnimator.SetBool(IsWalking, true);
			Vector3 newPos = _waypoints[UnityEngine.Random.Range(0, _waypoints.Count() - 1)].position;
			_nma.SetDestination(newPos);
		}
	}
	
	void OnGUI() {
		Vector3 worldPos = transform.position + _offset;
		Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);

		if (screenPos.z > 0)
		{
			float max = _madnessIncrements["Insane"].nextStep;
			float fill =  Mathf.Clamp(_madness, 0, max) / max;
			Rect bgRect = new Rect(screenPos.x - size.x / 2, Screen.height - screenPos.y, size.x, size.y);
			GUI.color = Color.black;
			GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
			GUI.color = Color.Lerp(Color.green, Color.red, fill);
			GUI.DrawTexture(new Rect(bgRect.x + 1, bgRect.y + 1, (size.x - 2) * fill, size.y - 2), Texture2D.whiteTexture);
			GUI.color = Color.white;
		}
        
		// AI state text
		Vector3 textWorldPos = transform.position + _textOffset;
		Vector3 textScreenPos = _camera.WorldToScreenPoint(textWorldPos);
		if (textScreenPos.z > 0)
		{
			Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(_state));
			Rect textRect = new Rect(textScreenPos.x - textSize.x / 2, Screen.height - textScreenPos.y, textSize.x, textSize.y);
			GUI.color = Color.white;
			// float fill = Mathf.Clamp(_aiScript._madness, 0, 150) / 150f;
			// GUI.color = Color.Lerp(Color.green, Color.red, fill);
			GUI.Label(textRect, _state);
		}
	}
}
