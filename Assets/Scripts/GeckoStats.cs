using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Gecko Stats")]
[Serializable]
[ExecuteInEditMode]
public class GeckoStats : ScriptableObject
{
    public event Action OnChanged;
    [SerializeField] private int _foodEaten = 0;
    public int foodEaten
    {
        get
        {
            return _foodEaten;
        }
        set
        {
            if (_foodEaten != value)
            {
                _foodEaten = value;
                OnChanged?.Invoke();
            }
        }
    }

    private void OnValidate()
    {
        OnChanged?.Invoke();
    }
}
