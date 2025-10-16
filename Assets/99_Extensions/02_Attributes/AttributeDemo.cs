using UnityEngine;
using CI;

public class AttributeDemo : MonoBehaviour
{
    // ---------------------------- SerializeField
    [HorizontalLine("Line1")]
    [Button("Push Button")]
    private void Btn()
    {
        Debug.Log("Push Button");
    }

    [HorizontalLine("Line2", "#ff1100")]
    [SerializeField] private float _aValue;
    [HorizontalLine("Line3", "#ffd900", 20f, 50f)]
    [SerializeField] private float _bValue;
    [Disable]
    [SerializeField] private float _cValue;
    [Space()]
    [SerializeField] private float _dValue;
    [Space(20f)]
    [SerializeField] private float _eValue;
    [SerializeField] private float _fValue;
    [SerializeField] private float _gValue;
}
