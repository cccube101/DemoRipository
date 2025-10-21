using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ACtrl : MonoBehaviour
{
    private enum State
    {
        Enable,
        Disable
    }

    // ---------------------------- SerializeField
    // PlayerInput�R���|�[�l���g�̎Q��
    [SerializeField] private PlayerInput _input;

    // �ړ����x
    [SerializeField] private float _moveSpeed;

    // ---------------------------- Field

    // ���͂��󂯎�邽�߂̕ϐ�
    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;

    // ���͂�ۑ����邽�߂̕ϐ�
    private Vector2 _dir = Vector2.zero;

    // ���̑��A�����ɕK�v�ȕϐ�
    private Transform _tr = null;
    private Rigidbody2D _rb = null;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        // ���̓A�N�V�����̏�����
        _act = new DemoAction();
        _demoAct = _act.Player;

        // �R���|�[�l���g�̎擾
        _tr = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // �ړ������̌Ăяo��
        Move();
    }

    private void OnEnable()
    {
        // ���̓A�N�V�����̗L�����ƃC�x���g�o�^
        _act?.Enable();
        ChangeAct(State.Enable);
    }

    private void OnDisable()
    {
        // ���̓A�N�V�����̖������ƃC�x���g����
        ChangeAct(State.Disable);
        _act?.Disable();
    }

    // ---------------------------- PrivateMethod
    /// <summary>
    /// �A�N�V�����̕ύX�E�o�^�E����
    /// </summary>
    /// <param name="state">�ύX���@</param>
    private void ChangeAct(State state)
    {
        // ���͋@��̕ύX
        switch (state)
        {
            case State.Enable:
                _input.onControlsChanged += input => OnControlsChanged();
                break;

            case State.Disable:
                _input.onControlsChanged -= input => OnControlsChanged();
                break;
        }

        // ���̓A�N�V�����ƃC�x���g�n���h���̕R�t���E����
        Set(_demoAct.Move, OnMove);
        Set(_demoAct.Fire, OnFire);

        // �����ɂ���ď����𕪊�
        void Set(InputAction input, Action<InputAction.CallbackContext> act)
        {
            switch (state)
            {
                case State.Enable:
                    input.started += act;
                    input.performed += act;
                    input.canceled += act;
                    break;

                case State.Disable:
                    input.started -= act;
                    input.performed -= act;
                    input.canceled -= act;
                    break;
            }
        }
    }

    /// <summary>
    /// ���͋@�킪�ύX���ꂽ���̏���
    /// </summary>
    private void OnControlsChanged()
    {
        // ���݂̃R���g���[���X�L�[�������O�ɕ\��
        // �X�L�[������ string �ɕύX���ē��͋@��𔻕ʂ��邱�Ƃ��\
        Debug.Log($"Control scheme changed:{_input.currentControlScheme}");
    }

    /// <summary>
    /// �ړ��̓��͂��󂯎��
    /// </summary>
    /// <param name="context">�R���e�L�X�g</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        // ���͒l��ۑ�
        _dir = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    private void Move()
    {
        // �ۑ����ꂽ���͒l�Ɋ�Â��Ĉړ�����������
        Vector2 move = _dir * _moveSpeed; // �ړ��ʂ��v�Z
        _rb.MovePosition(_rb.position + move); // �C�ӂ̕��@�ňړ�����������
    }

    /// <summary>
    /// ���΂̓��͂��󂯎��
    /// </summary>
    /// <param name="context">�R���e�L�X�g</param>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Fire();
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Fire()
    {
        Debug.Log("Fire!");
    }

}
