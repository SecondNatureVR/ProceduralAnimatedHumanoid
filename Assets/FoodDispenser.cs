using UnityEngine;
using UnityEngine.InputSystem;

public class FoodDispenser : MonoBehaviour
{
    [SerializeField] public GameObject foodPrefab;

    public void Dispense()
    {
        var food = Instantiate(foodPrefab, transform.position + transform.forward, transform.rotation);
        var rb = food.GetComponent<Rigidbody>();
        rb.velocity = 8.0f * (Quaternion.AngleAxis(Random.Range(-90, 90), Vector3.up) * transform.forward + Vector3.up);
        rb.angularVelocity = rb.transform.up * -10;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            return;
        Dispense();
    }
}
