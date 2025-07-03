using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvment : MonoBehaviour
{
	private static readonly int WalkForward = Animator.StringToHash("WalkForward");

	[SerializeField] private GameObject			Camera;
    [SerializeField] private InputActionAsset	InputAction;
	[SerializeField] private Animator 			_characterAnimator;
	[SerializeField] private GameObject			_MoneyTextObject;

    private InputAction		_moveAction;
    private InputAction		_jumpAction;
    private InputAction		_SprintAction;

    private Vector2     	_moveInput;
    private Rigidbody		_rb;
	private ParticleSystem	_jumpParticles;
	private	bool			_isRunning;
	private TextMeshProUGUI			_MoneyText;

	[SerializeField] private float	_moveSpeed = 40f;
	[SerializeField] private float	_runSpeed = 80f;
	[SerializeField] private float	_movementSmoothing = 10f; // Higher values = more responsive, lower = smoother
	[SerializeField] private bool	_isGrounded = true;
	[SerializeField] private float	_jumpForce = 5f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		InputAction.FindActionMap("Player").Enable();

		_moveAction = InputAction.FindActionMap("Player").FindAction("Move");
		_jumpAction = InputAction.FindActionMap("Player").FindAction("Jump");
		_SprintAction = InputAction.FindActionMap("Player").FindAction("Sprint");

		_rb = GetComponent<Rigidbody>();
		_jumpParticles = GetComponentInChildren<ParticleSystem>();
		_characterAnimator = GetComponent<Animator>();
		_MoneyText = _MoneyTextObject.GetComponent<TextMeshProUGUI>();

		// Lock cursor to center of screen for mouse look
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_isRunning = false;
    }

	// Update is called once per frame
	void Update()
	{
		transform.rotation = new Quaternion(0, Camera.transform.rotation.y, 0, Camera.transform.rotation.w);
		if (_jumpAction.WasPressedThisFrame() && _isGrounded && !_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
			Jump();
    }

	void Jump() {
		_characterAnimator.SetTrigger("JumpTrigger");
		_rb.AddForceAtPosition(new Vector3(0, _jumpForce, 0), Vector3.up, ForceMode.Impulse);
		_isGrounded = false;
	}

	void FixedUpdate()
	{
		_moveInput = _moveAction.ReadValue<Vector2>();
		_MoneyText.text = "Money : " + Math.Round(Points._money).ToString() + " $$$";
		Moving();
	}

	private void Moving()
	{
		Vector3 targetVelocity = default;
		if (_SprintAction.IsPressed() && _moveInput.y >= 0f)
		{
			_isRunning = true;
			targetVelocity = transform.forward * (_moveInput.y * _runSpeed) +
				transform.right * (_moveInput.x * _runSpeed);
			_characterAnimator.SetBool("IsRunning", true);
		}
		else
		{
			_isRunning = false;
			targetVelocity = transform.forward * (_moveInput.y * _moveSpeed) +
				transform.right * (_moveInput.x * _moveSpeed);	
			_characterAnimator.SetBool("IsRunning", false);
			if (_moveInput.y > 0.1f || _moveInput.y < -0.1f)
				_characterAnimator.SetTrigger(WalkForward);
		}
		// Calculate target velocity based on input


		// Smoothly interpolate current velocity towards target velocity
		Vector3 smoothedVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * _movementSmoothing);

		// Bool RunType : 0 - RunForward, 1 - RunForwardLeft, 2 - RunForwardRight, 3 - RunLeft, 4 - RunRight
		_characterAnimator.SetInteger("RunType", 0);
		if (_moveInput.y > 0.1f && _moveInput.x < -0.1f)
			_characterAnimator.SetInteger("RunType", 1);
		else if (_moveInput.y > 0.1f && _moveInput.x > 0.1f)
			_characterAnimator.SetInteger("RunType", 2); 
		else if (_moveInput.y > -0.1f && _moveInput.y < 0.1f && _moveInput.x < -0.1f)
			_characterAnimator.SetInteger("RunType", 3);
		else if (_moveInput.y > -0.1f && _moveInput.y < 0.1f && _moveInput.x > 0.1f)
			_characterAnimator.SetInteger("RunType", 4);

		if (_moveInput.magnitude < 0.1f)
		{
			smoothedVelocity = Vector3.zero; // Stop movement if input is very low
			_characterAnimator.SetTrigger("Idle");
		}
		else if (!_isRunning)
		{
			_characterAnimator.SetTrigger("WalkForward");
		}

		// Apply the smoothed velocity (preserve Y velocity for jumping/gravity)
			_rb.linearVelocity = new Vector3(smoothedVelocity.x, _rb.linearVelocity.y, smoothedVelocity.z);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			_jumpParticles.Play();
			_isGrounded = true;
		}
	}

}
