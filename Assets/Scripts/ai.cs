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

	[SerializeField] private Vector3 _offset = new Vector3(0, 5, 0);
	[SerializeField] private Vector3 _textOffset = new Vector3(0, 5.5f, 0);
	[SerializeField] private Vector2 size = new Vector2(60, 10);
	[SerializeField] private List<Transform> _waypoints;
	
	private Camera _camera;
	private struct Increment {
		public float min, max, time, delta, nextStep;
		public Action func;
	};

	[SerializeField] private GameObject chair;
	 private Animator 			_characterAnimator;

	
	private NavMeshAgent _nma;
	private Boolean _chairReached;
	private Boolean _onDesk;
	private Vector3 _lastTargetPos;
	private Vector3 _targetPos;
	private Dictionary<string, Increment> _madnessIncrements = new Dictionary<string, Increment>();
	private Increment _madnessIncr;
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
		_lastTargetPos = chair.transform.position + new Vector3(1, 1, 1);
		_targetPos = chair.transform.position + new Vector3(1, 1, 1);
		_madnessIncrements.AddRange(new Dictionary<string, Increment>
		{
			{ "Normal", new Increment { min = 5.5f, max = 0.75f, time = 0, delta = 1, nextStep = 10, func = NormalBehaviour } },
			{ "UnBothered", new Increment { min = 5.75f, max = 1f, time = 0, delta = 1, nextStep = 20, func = UnBotheredBehaviour } },
			{ "Deranged", new Increment { min = 5f, max = 2f, time = 0, delta = .5f, nextStep = 45, func = DerangedBehaviour } },
			{ "Crazy", new Increment { min = 5.75f, max = 2.75f, time = 0, delta = .5f, nextStep = 85, func = CrazyBehaviour } },
			{ "Insane", new Increment { min = 5f, max = 3.5f, time = 0, delta = .35f, nextStep = 120, func = InsaneBehaviour } }
		});
		_madnessIncr = _madnessIncrements["Normal"];
		_madness = 0;
		_camera = Camera.main;
		var meshRenderer = GetComponentInChildren<MeshRenderer>();
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
		_characterAnimator.SetBool(IsDancing, false);
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
		if (!_chairReached && chair.transform.position != _lastTargetPos)
		{
			_nma.SetDestination(chair.transform.position);
			if (!_onDesk)
			{
				_onDesk = true;
				_characterAnimator.SetTrigger(Sit);
			}
		}
	}

	private void UnBotheredBehaviour()
	{
		_state = "UnBothered";
		_characterAnimator.SetBool(IsDancing, false);
		Work();
		//#stopwork
		//#on desk animations
	}
	private void DerangedBehaviour()
	{
		_state = "Deranged";
		_characterAnimator.SetBool(IsDancing, false);
		_onDesk = false;
		_chairReached = false;
		//# get out of desk
		//# starts walking around
	}
	private void CrazyBehaviour()
	{
		_state = "Crazy";
		_characterAnimator.SetBool(IsDancing, true);
		_characterAnimator.SetInteger("DanceType", UnityEngine.Random.Range(1, 6));
		_onDesk = false;
		_chairReached = false;
	}
	private void InsaneBehaviour()
	{
		_state = "Insane";
		//# starts tweaking
		_characterAnimator.SetBool(IsDancing, false);
		_onDesk = false;
		_chairReached = false;
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
