using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Button _buyBtn;
    [SerializeField] private StoreProcess.ItemType _itemType;

    // ---------------------------- Field



    // ---------------------------- UnityMessage
    private void Start()
    {
        _buyBtn.onClick.AddListener(() =>
        {
            _gameManager.TryBuyItem(_itemType);

        });
    }



    // ---------------------------- PublicMethod





    // ---------------------------- PrivateMethod





}
