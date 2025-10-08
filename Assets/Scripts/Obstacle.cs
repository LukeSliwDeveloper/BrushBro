using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Vector3.forward * Time.fixedDeltaTime * GameplayManager.Instance.RoadSpeed);
        if (_rigidbody.position.z >= 7f)
            Destroy(gameObject);
    }
}
