using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float timeScale = 1f;
    private void Start()
    {
        Time.timeScale = timeScale; 
    }

    private void OnGUI()
    {
        Time.timeScale = timeScale;
    }
}
