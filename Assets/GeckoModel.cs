using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Build;
using UnityEngine;

[ExecuteInEditMode]
public class GeckoModel : MonoBehaviour
{
    [SerializeField] public GeckoStats stats;
    private GeckoController_Full gecko;
    private Material geckoMaterial;

    public float Fatness => Mathf.Lerp(0, 100, (20 + stats.foodEaten) / 100);
    public void Start()
    {
        geckoMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        gecko = GetComponentInChildren<GeckoController_Full>();
        stats.OnChanged += HandleStatsChanged;
        HandleStatsChanged();
    }

    private void OnDestroy()
    {
        stats.OnChanged -= HandleStatsChanged; 
    }

    // TODO: GameObject -> useful notion of edible gecko food
    public void Eat(GameObject g)
    {
        stats.foodEaten += 1;
    }

    private void HandleStatsChanged()
    {
        UpdateMaterials();
        UpdateController();
    }

    private void UpdateController() {
        float t = Mathf.InverseLerp(0, 100, Fatness);
        gecko.speedModifier = Mathf.Lerp(3f, 0.5f, Mathf.Sqrt(t));
    }

    private void UpdateMaterials()
    {
        geckoMaterial.SetFloat("_Fatness", Fatness);
    }
}
