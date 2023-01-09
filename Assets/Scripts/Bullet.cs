using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody _rigidBody;
    private bool _hit = false;
    private PlayerController _player;

    private void Start() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    //set the player this arrow belongs to
    public void SetPlayer(PlayerController player)=>this._player=player;

    private void Update() {
        if(!this._hit){
            // SpinArrowInTheAir();
        }
    }

    // private void OnTriggerEnter(Collider other) {
    //     if(!_hit){
    //         _hit = true;
            // _rigidBody.velocity = Vector3.zero;
            // _rigidBody.isKinematic = true;
        // }
    // }

    private void OnCollisionEnter(Collision other) {
        if(!_hit){
            _hit = true;
            StartCoroutine(BackToPoolTimeOut());
        }
    }

    // private void SpinArrowInTheAir(){
    //     // float combinedVelocity = Mathf.Sqrt(_rigidBody.velocity.y * _rigidBody.velocity.z);

    //     float fallAngle = Mathf.Atan2(_rigidBody.velocity.y, _rigidBody.velocity.z) * Mathf.Rad2Deg;

    //     this.gameObject.transform.rotation = Quaternion.Euler(-fallAngle, transform.rotation.y, transform.rotation.z);
    // }

    //return the arrow to the arrow pool with 5 seconds after it collides anywhere
    private IEnumerator BackToPoolTimeOut(){
        yield return new WaitForSecondsRealtime(5.0f);
        ResetBulletStats();
        _player.AddToPool(this);
    }

    private void ResetBulletStats(){
        this.gameObject.SetActive(false);
        // _rigidBody.isKinematic = false;
        _rigidBody.velocity = Vector3.zero;
        this._hit = false;
    }
}
