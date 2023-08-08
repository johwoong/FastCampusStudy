using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    Stat stat;
    [SerializeField]
    BoxCollider weapon;
    

    [SerializeField]
    float _speed = 3.0f;

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard;
        Managers.Input.LeftClickAction -= OnLeftClicked;
        Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.LeftClickAction += OnLeftClicked;
        stat = GetComponent<Stat>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("moveSpeed", 0.0f);
    }

    void OnLeftClicked(bool clicked)
    {
        if (clicked)
        {
            animator.SetTrigger("isAttack");
        }
    }

    void OnWeapon()
    {
        weapon.enabled = true;
    }
    void OffWeapon()
    {
        weapon.enabled = false;
    }    

    void OnKeyboard(KeyCode key)
    {
        if (key == KeyCode.W)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }
        if (key == KeyCode.S)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }
        if (key == KeyCode.A)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }
        if (key == KeyCode.D)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }
        animator.SetFloat("moveSpeed", 3.0f);
    }

}
