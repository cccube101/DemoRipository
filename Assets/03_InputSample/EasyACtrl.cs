using UnityEngine;
using UnityEngine.InputSystem;

public class EasyACtrl : MonoBehaviour
{
    // ---------------------------- Field
    // ���͂��󂯎�邽�߂̕ϐ�
    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;

    // ���͂�ۑ����邽�߂̕ϐ�
    private Vector2 _dir = Vector2.zero;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        // ���̓A�N�V�����̏�����
        _act = new DemoAction();
        _demoAct = _act.Player;
    }


    private void OnEnable()
    {
        // ���̓A�N�V�����̗L����
        _act?.Enable();

        // �C�x���g�o�^
        _demoAct.Move.started += OnMove;
        _demoAct.Move.performed += OnMove;
        _demoAct.Move.canceled += OnMove;

        _demoAct.Fire.started += OnFire;
        _demoAct.Fire.performed += OnFire;
        _demoAct.Fire.canceled += OnFire;

    }

    private void OnDisable()
    {
        // �C�x���g����
        _demoAct.Move.started -= OnMove;
        _demoAct.Move.performed -= OnMove;
        _demoAct.Move.canceled -= OnMove;

        _demoAct.Fire.started -= OnFire;
        _demoAct.Fire.performed -= OnFire;
        _demoAct.Fire.canceled -= OnFire;

        // ���̓A�N�V�����̖�����
        _act?.Disable();
    }




    // ---------------------------- PrivateMethod

    /// <summary>
    /// �ړ��̓��͂��󂯎��
    /// </summary>
    /// <param name="context">�R���e�L�X�g</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        // ���͒l��ۑ�
        _dir = context.ReadValue<Vector2>();
        Debug.Log($"Move Dir:{_dir}");
    }

    /// <summary>
    /// ���΂̓��͂��󂯎��
    /// </summary>
    /// <param name="context">�R���e�L�X�g</param>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Fire!");
        }
    }
}
