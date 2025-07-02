using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvment : MonoBehaviour
{

    [SerializeField] private GameObject Camera;

    [SerializeField] private InputActionAsset InputAction;

    private InputAction	_moveAction;
    private InputAction	_jumpAction;

    private Vector2     _moveInput;
    private Vector2     _lookInput;
    private Rigidbody	_rb;

	[SerializeField] private float	_moveSpeed = 5f;
	[SerializeField] private bool	_isGrounded = true;
	[SerializeField] private float	_jumpForce = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputAction.FindActionMap("Player").Enable();

		_moveAction = InputAction.FindActionMap("Player").FindAction("Move");
		_jumpAction = InputAction.FindActionMap("Player").FindAction("Jump");

		_rb = GetComponent<Rigidbody>();
		
		// Lock cursor to center of screen for mouse look
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
		// transform.rotation = Camera.transform.rotation;
		transform.rotation = new Quaternion(0, Camera.transform.rotation.y, 0, Camera.transform.rotation.w);
        if (_jumpAction.WasPressedThisFrame() && _isGrounded)
			Jump();
    }

	void Jump() {
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
		_rb.MovePosition(_rb.position + transform.forward * _moveInput.y * _moveSpeed * Time.fixedDeltaTime + 
						 transform.right * _moveInput.x * _moveSpeed * Time.fixedDeltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			_isGrounded = true;
		}
	}

}
