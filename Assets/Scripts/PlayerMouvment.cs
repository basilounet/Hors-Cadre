using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvment : MonoBehaviour
{

    [SerializeField] private GameObject			Camera;
    [SerializeField] private InputActionAsset	InputAction;
	[SerializeField] private Animator 			_characterAnimator;

    private InputAction		_moveAction;
    private InputAction		_jumpAction;

    private Vector2     	_moveInput;
    private Rigidbody		_rb;
	private ParticleSystem	_jumpParticles;

	[SerializeField] private float	_moveSpeed = 5f;
	[SerializeField] private float	_movementSmoothing = 10f; // Higher values = more responsive, lower = smoother
	[SerializeField] private bool	_isGrounded = true;
	[SerializeField] private float	_jumpForce = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputAction.FindActionMap("Player").Enable();

		_moveAction = InputAction.FindActionMap("Player").FindAction("Move");
		_jumpAction = InputAction.FindActionMap("Player").FindAction("Jump");

		_rb = GetComponent<Rigidbody>();
		_jumpParticles = GetComponentInChildren<ParticleSystem>();
		_characterAnimator = GetComponent<Animator>();
		
		// Lock cursor to center of screen for mouse look
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

	// Update is called once per frame
	void Update()
	{
		// transform.rotation = Camera.transform.rotation;
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
		Walking();
	}

	private void Walking()
	{
		// Calculate target velocity based on input
		Vector3 targetVelocity = transform.forward * _moveInput.y * _moveSpeed + 
								 transform.right * _moveInput.x * _moveSpeed;
		
		// Smoothly interpolate current velocity towards target velocity
		Vector3 smoothedVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * _movementSmoothing);

		if (_moveInput.magnitude < 0.1f) {
			smoothedVelocity = Vector3.zero; // Stop movement if input is very low
			_characterAnimator.SetTrigger("Idle");
		}
		else {
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
