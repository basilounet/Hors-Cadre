using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOpen : MonoBehaviour
{

    [SerializeField] private InputActionAsset	_InputAction;
    public float open = 100f;
    public float range = 10f;

    public GameObject door;
    public bool isOpening = false;

    public Camera fpsCam;

    private InputAction		_interactAction;

    private void Start()
    {
        _interactAction = _InputAction.FindActionMap("Player").FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (_interactAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        door.GetComponent<Animator>().Play("DoorOpen");
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(5.0f);
        door.GetComponent<Animator>().Play("New State");
        isOpening = false;
    }

}