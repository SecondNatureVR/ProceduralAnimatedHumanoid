using UnityEngine;
using UnityEngine.InputSystem;

public class FoodDispenser : MonoBehaviour
{
    [SerializeField] public GameObject foodPrefab;

    public void Dispense()
    {
        var food = Instantiate(foodPrefab, transform.position + transform.forward, transform.rotation);
        food.GetComponent<Rigidbody>().velocity = 10.0f * (transform.forward + Vector3.up);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            return;
        Dispense();
    }
}
