using UnityEngine;
using UnityEngine.InputSystem;

public class FoodDispenser : MonoBehaviour
{
    [SerializeField] public GameObject foodPrefab;
    [SerializeField] public Transform player;

    public void Dispense()
    {
        Vector3 awayPlayer = Vector3.ProjectOnPlane((transform.position - player.position).normalized, Vector3.up);
        var food = Instantiate(foodPrefab, transform.position + awayPlayer, transform.rotation);
        var rb = food.GetComponent<Rigidbody>();
        rb.velocity = 12.0f * (Vector3.Slerp(awayPlayer, Random.onUnitSphere, 0.1f) + (0.5f * Vector3.up)); ;
        rb.angularVelocity = Random.onUnitSphere * 25.0f;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            return;
        Dispense();
    }
}
