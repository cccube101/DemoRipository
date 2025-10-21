# 🎮 Unity Input System 入力方法チュートリアル

## 🧩 準備

 **Package Manager**で
   `Input System` をインポートします。

 シーン内の任意のオブジェクトに
   **PlayerInputコンポーネント**をアタッチします。

 設定変更：

   * **Actions** → `None`
   * **Behavior** → `Invoke C Sharp Events`
     → スクリプトから入力を受け取る準備が完了

 **Create Actions** をクリックし、
   任意のフォルダにアクションアセットを作成します。

   ![PlayerInput](https://github.com/user-attachments/assets/d81fa8a2-78e3-457b-afac-d25d7b31a385)

---

## ⚙️ アクション設定とスクリプト生成

 作成したActionアセットを **Projectタブで選択**。

 `Generate C# Class` にチェックを入れ、`Apply` をクリック。
   → スクリプトから参照可能なクラスが生成されます。

   ![GenerateC#](https://github.com/user-attachments/assets/8c8dc346-3586-4ca6-95ea-661b8d01f3cd)

 下部のタブでアクション内容を確認。
   入力タイプやバインド設定は、他サイトの解説を参考にしてください。

   ![ShowAction](https://github.com/user-attachments/assets/dc23aa12-c8b6-41d9-b851-be9e73eb16e3)

---

## 💡 入力を受け取る簡単なサンプルスクリプト

以下は基本的なイベント登録・解除と入力取得の例です。  
`InputAction.CallbackContext context`を引数に持つメソッドを登録したいアクションに  
`OnEnable`で登録、`OnDissable`で解除します。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class EasyACtrl : MonoBehaviour
{
    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;
    private Vector2 _dir = Vector2.zero;

    private void Awake()
    {
        _act = new DemoAction();
        _demoAct = _act.Player;
    }

    private void OnEnable()
    {
        _act?.Enable();

        _demoAct.Move.started += OnMove;
        _demoAct.Move.performed += OnMove;
        _demoAct.Move.canceled += OnMove;

        _demoAct.Fire.started += OnFire;
        _demoAct.Fire.performed += OnFire;
        _demoAct.Fire.canceled += OnFire;
    }

    private void OnDisable()
    {
        _demoAct.Move.started -= OnMove;
        _demoAct.Move.performed -= OnMove;
        _demoAct.Move.canceled -= OnMove;

        _demoAct.Fire.started -= OnFire;
        _demoAct.Fire.performed -= OnFire;
        _demoAct.Fire.canceled -= OnFire;

        _act?.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _dir = context.ReadValue<Vector2>();
        Debug.Log($"Move Dir:{_dir}");
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Fire!");
        }
    }
}
```

---

## 🧱 実用テンプレート（入力機器変更にも対応）

より実践的なサンプルです。  
`OnEnable`、`OnDissable`で省略できるものを省略しています。  
`OnControlsChanged()`で、キーボード・コントローラーなどの入力デバイス変更も検知します。
スキームの名称を`string`で受取できます。

```csharp
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ACtrl : MonoBehaviour
{
    private enum State { Enable, Disable }

    [SerializeField] private PlayerInput _input;
    [SerializeField] private float _moveSpeed;

    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;
    private Vector2 _dir = Vector2.zero;
    private Transform _tr;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _act = new DemoAction();
        _demoAct = _act.Player;
        _tr = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        _act?.Enable();
        ChangeAct(State.Enable);
    }

    private void OnDisable()
    {
        ChangeAct(State.Disable);
        _act?.Disable();
    }

    private void ChangeAct(State state)
    {
        switch (state)
        {
            case State.Enable:
                _input.onControlsChanged += input => OnControlsChanged();
                break;
            case State.Disable:
                _input.onControlsChanged -= input => OnControlsChanged();
                break;
        }

        Set(_demoAct.Move, OnMove);
        Set(_demoAct.Fire, OnFire);

        void Set(InputAction input, Action<InputAction.CallbackContext> act)
        {
            if (state == State.Enable)
            {
                input.started += act;
                input.performed += act;
                input.canceled += act;
            }
            else
            {
                input.started -= act;
                input.performed -= act;
                input.canceled -= act;
            }
        }
    }

    private void OnControlsChanged()
    {
        Debug.Log($"Control scheme changed: {_input.currentControlScheme}");
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _dir = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector2 move = _dir * _moveSpeed;
        _rb.MovePosition(_rb.position + move);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            Fire();
    }

    private void Fire()
    {
        Debug.Log("Fire!");
    }
}
```

---

## 🔗 最後に

* 作成したスクリプトをシーン内オブジェクトにアタッチします。
* `PlayerInput` の参照をスクリプトの `_input` フィールドに紐づけておきましょう。

![SetScript](https://github.com/user-attachments/assets/bc58a886-cacb-4290-b25b-eb1c77b60cec)

---

### ✅ まとめ

| 項目         | 内容                            |
| ---------- | ----------------------------- |
| 使用パッケージ    | Input System                  |
| コンポーネント    | PlayerInput                   |
| 推奨Behavior | Invoke C Sharp Events         |
| クラス生成      | Action → Generate C# Class    |
| 入力受信       | InputAction.CallbackContext   |
| デバイス検知     | PlayerInput.onControlsChanged |

---
