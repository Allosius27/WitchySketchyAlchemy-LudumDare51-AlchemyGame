using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothTime = 0.5f;
    [SerializeField] private float _minX, _maxX;

    private float _yLock;
    private Vector3 _currentVel;

    void Start() {
        _yLock = transform.position.y;
        
      
    }


    void Update() {
        if (!_player) return;

        var target = new Vector3(Mathf.Clamp(_player.position.x, _minX, _maxX), _yLock, -10);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _currentVel, _smoothTime);
    }
}