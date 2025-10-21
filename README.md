# 🎮 Unity Input System 入力方法チュートリアル

## 🧩 準備

 **Package Manager**で
   `Input System` をインポートします。

 シーン内の任意のオブジェクトに
   **PlayerInputコンポーネント**をアタッチします。

 設定変更：

   * **Actions** → `None`  
      選択を`None`に変更
   * **Behavior** → `Invoke C Sharp Events`  
      スクリプトから入力を受け取る準備が完了

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
    private enum State
    {
        Enable,
        Disable
    }

    // ---------------------------- SerializeField
    // PlayerInputコンポーネントの参照
    [SerializeField] private PlayerInput _input;

    // 移動速度
    [SerializeField] private float _moveSpeed;

    // ---------------------------- Field

    // 入力を受け取るための変数
    private DemoAction _act;
    private DemoAction.PlayerActions _demoAct;

    // 入力を保存するための変数
    private Vector2 _dir = Vector2.zero;

    // その他、処理に必要な変数
    private Transform _tr = null;
    private Rigidbody2D _rb = null;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        // 入力アクションの初期化
        _act = new DemoAction();
        _demoAct = _act.Player;

        // コンポーネントの取得
        _tr = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 移動処理の呼び出し
        Move();
    }

    private void OnEnable()
    {
        // 入力アクションの有効化とイベント登録
        _act?.Enable();
        ChangeAct(State.Enable);
    }

    private void OnDisable()
    {
        // 入力アクションの無効化とイベント解除
        ChangeAct(State.Disable);
        _act?.Disable();
    }

    // ---------------------------- PrivateMethod
    /// <summary>
    /// アクションの変更・登録・解除
    /// </summary>
    /// <param name="state">変更方法</param>
    private void ChangeAct(State state)
    {
        // 入力機器の変更
        switch (state)
        {
            case State.Enable:
                _input.onControlsChanged += input => OnControlsChanged();
                break;

            case State.Disable:
                _input.onControlsChanged -= input => OnControlsChanged();
                break;
        }

        // 入力アクションとイベントハンドラの紐付け・解除
        Set(_demoAct.Move, OnMove);
        Set(_demoAct.Fire, OnFire);

        // 引数によって処理を分岐
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
    /// 入力機器が変更された時の処理
    /// </summary>
    private void OnControlsChanged()
    {
        // 現在のコントロールスキームをログに表示
        // スキームから string に変更して入力機器を判別することが可能
        Debug.Log($"Control scheme changed:{_input.currentControlScheme}");
    }

    /// <summary>
    /// 移動の入力を受け取る
    /// </summary>
    /// <param name="context">コンテキスト</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        // 入力値を保存
        _dir = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        // 保存された入力値に基づいて移動処理を実装
        Vector2 move = _dir * _moveSpeed; // 移動量を計算
        _rb.MovePosition(_rb.position + move); // 任意の方法で移動処理を実装
    }

    /// <summary>
    /// 着火の入力を受け取る
    /// </summary>
    /// <param name="context">コンテキスト</param>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Fire();
        }
    }

    /// <summary>
    /// 着火
    /// </summary>
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
