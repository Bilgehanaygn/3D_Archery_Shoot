using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //animation related fields
    private Animator _animator;

    //trajectory related fields
    private LineRenderer _renderer;
    private int _maxIterations = 20;

    private bool _blocked = false;

    //shoot related fields
    private Vector2 _firstTouch;
    private float _force = 0.05f;
    private Queue<Bullet> _bulletPool;

    [SerializeField] private GameObject _bulletPrefab;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponent<LineRenderer>();
        _bulletPool = new Queue<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_blocked){
            FingerDrag();
        }
    }

    private void FingerDrag(){
        //if clicked on the screen
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            //update animation state from moving to aiming by setting isAiming to true
            _animator.SetBool("aiming", true);
            _firstTouch = Input.GetTouch(0).position;
            Debug.Log("now true");
        }
        else if(Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)){
            RenderTrajectory(Input.GetTouch(0).position);
        }
        else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
            FingerRelease(Input.GetTouch(0).position);
        }
    }

    private void RenderTrajectory(Vector2 lastPosition){
        _renderer.positionCount = _maxIterations;
        for(int i=0;i<_maxIterations;i++){
            //no change in x
            float newX = transform.position.x;
            //change in Y is => V0t - 1/2gt^2;
            float newY = transform.position.y + 1.4f + ((_firstTouch.y - lastPosition.y) * i * Time.fixedDeltaTime * _force) + ((Physics.gravity.y*Mathf.Pow(Time.fixedDeltaTime *i,2))/2) ;
            float newZ = transform.position.z + 0.75f +  (Mathf.Abs(_firstTouch.x - lastPosition.x) * Time.fixedDeltaTime * i * _force);
            _renderer.SetPosition(i, new Vector3(newX, newY, newZ));
        }
    }
    private void ThrowAnBullet(Vector2 lastPosition){
        Bullet bullet = GetAnBullet();
        
        bullet.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z + 0.75f);
        bullet.gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
        bullet.gameObject.SetActive(true);

        bullet.GetComponent<Rigidbody>().AddForce(  
            new Vector3(
                0f,
                (_firstTouch.y - lastPosition.y)  * _force,
                Mathf.Abs(_firstTouch.x - lastPosition.x) * _force
            ),
            ForceMode.Impulse
        );
    }

    private Bullet GetAnBullet(){

        //if the queue already has an arrow, return it
        if(_bulletPool.Count > 0){
            var bullet = _bulletPool.Dequeue();
            // arrow.gameObject.SetActive(true);
            return bullet;
        }
        else{
            //otherwise instantiate a new one and return, this will be eventually destroyed
            var bullet = Instantiate(_bulletPrefab);
            bullet.GetComponent<Bullet>().SetPlayer(this);
            return bullet.GetComponent<Bullet>();
        }
    }

    private void FingerRelease(Vector2 fingerPosition){
        //set the animation back to idle
        //remove the trajectory
        //shoot an arrow
        _renderer.positionCount = 0;
        _animator.SetBool("aiming", false);
        StartCoroutine(AwaitForThrowAnimationAndThrowBullet(fingerPosition));
    }

    public void AddToPool(Bullet bullet){
        this._bulletPool.Enqueue(bullet);
    }


    private IEnumerator AwaitForThrowAnimationAndThrowBullet(Vector2 fingerPosition){
        _blocked = true;
        yield return new WaitForSecondsRealtime(0.6f);
        ThrowAnBullet(fingerPosition);
        //await for animation get back to its initial position
        yield return new WaitForSecondsRealtime(1.2f);
        _blocked = false;
        Debug.Log("now false");
    }
}
