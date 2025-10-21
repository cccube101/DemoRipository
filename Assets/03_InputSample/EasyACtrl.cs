using UnityEngine;
using UnityEngine.InputSystem;

public class EasyACtrl : MonoBehaviour
{
    // ---------------------------- Field
    // 入力を受け取るための変数
    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;

    // 入力を保存するための変数
    private Vector2 _dir = Vector2.zero;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        // 入力アクションの初期化
        _act = new DemoAction();
        _demoAct = _act.Player;
    }


    private void OnEnable()
    {
        // 入力アクションの有効化
        _act?.Enable();

        // イベント登録
        _demoAct.Move.started += OnMove;
        _demoAct.Move.performed += OnMove;
        _demoAct.Move.canceled += OnMove;

        _demoAct.Fire.started += OnFire;
        _demoAct.Fire.performed += OnFire;
        _demoAct.Fire.canceled += OnFire;

    }

    private void OnDisable()
    {
        // イベント解除
        _demoAct.Move.started -= OnMove;
        _demoAct.Move.performed -= OnMove;
        _demoAct.Move.canceled -= OnMove;

        _demoAct.Fire.started -= OnFire;
        _demoAct.Fire.performed -= OnFire;
        _demoAct.Fire.canceled -= OnFire;

        // 入力アクションの無効化
        _act?.Disable();
    }




    // ---------------------------- PrivateMethod

    /// <summary>
    /// 移動の入力を受け取る
    /// </summary>
    /// <param name="context">コンテキスト</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        // 入力値を保存
        _dir = context.ReadValue<Vector2>();
        Debug.Log($"Move Dir:{_dir}");
    }

    /// <summary>
    /// 着火の入力を受け取る
    /// </summary>
    /// <param name="context">コンテキスト</param>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Fire!");
        }
    }
}
