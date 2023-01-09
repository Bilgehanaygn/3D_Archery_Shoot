using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private BoxCollider _goalCollider;

    public static event System.Action OnGoal;


    private void Start() {
        _goalCollider = GetComponent<BoxCollider>();
    } 

    private void OnTriggerEnter(Collider other) {
        //goal
        Bullet bullet;
        if(other.TryGetComponent(out bullet)){
            Debug.Log("its a goal");
            OnGoal?.Invoke();
        }
    }

}
