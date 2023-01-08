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

    private bool _isAiming;

    //shoot related fields
    private Vector2 _firstTouch;
    private float _force = 0.05f;
    private Queue<Arrow> _arrowPool;

    [SerializeField] private GameObject _arrowPrefab;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponent<LineRenderer>();
        _arrowPool = new Queue<Arrow>();
    }

    // Update is called once per frame
    void Update()
    {
        FingerDrag();
    }

    private void FingerDrag(){
        //if clicked on the screen
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            //update animation state from moving to aiming by setting isAiming to true
            _animator.SetBool("aiming", true);
            _firstTouch = Input.GetTouch(0).position;
            _isAiming = true;
        }
        else if(Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)){
            RenderTrajectory(Input.GetTouch(0).position);
        }
        else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
            FingerRelease();
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
    private void ThrowAnArrow(Vector2 lastPosition){
        Arrow arrow = GetAnArrow();
        
        arrow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z + 0.75f);
        arrow.gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
        arrow.gameObject.SetActive(true);

        arrow.GetComponent<Rigidbody>().AddForce(  
            new Vector3(
                0f,
                (_firstTouch.y - lastPosition.y)  * _force,
                Mathf.Abs(_firstTouch.x - lastPosition.x) * _force
            ),
            ForceMode.Impulse
        );
    }

    private Arrow GetAnArrow(){

        //if the queue already has an arrow, return it
        if(_arrowPool.Count > 0){
            var arrow = _arrowPool.Dequeue();
            // arrow.gameObject.SetActive(true);
            return arrow;
        }
        else{
            //otherwise instantiate a new one and return, this will be eventually destroyed
            var arrow = Instantiate(_arrowPrefab);
            arrow.GetComponent<Arrow>().SetPlayer(this);
            return arrow.GetComponent<Arrow>();
        }
    }

    private void FingerRelease(){
        //set the animation back to idle
        //remove the trajectory
        //shoot an arrow
        _renderer.positionCount = 0;
        _animator.SetBool("aiming", false);
        ThrowAnArrow(Input.GetTouch(0).position);
        _isAiming = false;
    }

    public void AddToPool(Arrow arrow){
        this._arrowPool.Enqueue(arrow);
    }
}
