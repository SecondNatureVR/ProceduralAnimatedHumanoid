using UnityEngine;
using UnityEngine.InputSystem;

public class FoodDispenser : MonoBehaviour
{
    [SerializeField] public GameObject foodPrefab;
    [SerializeField] public Transform player;


    public void DispenseAway()
    {
        Vector3 awayPlayer = Vector3.ProjectOnPlane((transform.position - player.position).normalized, Vector3.up);
        Dispense(awayPlayer);
    }
    public void DispenseToward()
    {
        Vector3 towardPlayer = Vector3.ProjectOnPlane((player.position - transform.position).normalized, Vector3.up);
        Dispense(towardPlayer);
    }
    public void Dispense(Vector3 dir)
    {
        var food = Instantiate(foodPrefab, transform.position + dir , transform.rotation);
        var rb = food.GetComponent<Rigidbody>();
        rb.velocity = 12.0f * (Vector3.Slerp(dir, Random.onUnitSphere, 0.1f) + (0.5f * Vector3.up)); ;
        rb.angularVelocity = Random.onUnitSphere * 25.0f;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            return;
        DispenseAway();
    }

    public void AltFire(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            return;
        DispenseToward();
    }
}
