using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private Vector3 _dist = new Vector3(-5f, 3f, 1f);

    
    void Start()
    {
        transform.position = _playerTransform.position + _dist;
        transform.LookAt(new Vector3(_playerTransform.position.x + 5, _playerTransform.position.y, _playerTransform.position.z + 7));

    }

}
