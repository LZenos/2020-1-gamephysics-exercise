using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Vector3 _pickPos = Vector3.zero;
    private Vector3 _fleePos = Vector3.zero;

    [SerializeField]
    private float _maxSpeed = 1.0f;

    [SerializeField]
    private float _deceleration = 1.0f;

    private bool _isSeek = false;

    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _isSeek = true;

            Vector3 mouse_pos = Input.mousePosition;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                _pickPos = hit.point;
                _pickPos.y = 0.0f;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mouse_pos = Input.mousePosition;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                _fleePos = hit.point;
                _fleePos.y = 0.0f;
            }
        }

        // seek
        // 3: 적당히 빠르게 해주기 위함.
        _velocity = _velocity + (seek(_pickPos) * Time.deltaTime * 3);

        // 속도를 기반으로 새로운 위치 계산.
        transform.position = transform.position + _velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_fleePos, 10.0f);
    }

    private Vector3 seek(Vector3 target_pos)
    {
        // 방향 변경을 위함.
        Vector3 distance = target_pos - transform.position;
        if (distance.sqrMagnitude > 0.005f)
        {
            transform.forward = _velocity.normalized;
        }

        Vector3 desired_velocity = distance.normalized * _maxSpeed;

        return (desired_velocity - _velocity);
    }

    private Vector3 flee(Vector3 target_pos)
    {
        // seek의 반대 방향 사용.
        Vector3 desired_velocity = ((transform.position - target_pos).normalized) * _maxSpeed;

        return (desired_velocity - _velocity);
    }

    private Vector3 arrive(Vector3 target_pos)
    {
        float distance = Vector3.Distance(target_pos, transform.position);

        if (distance > 0.0f)
        {
            Vector3 to_target = target_pos - transform.position;

            float _speed = distance / _deceleration;

            // 최대 속도로 제한.
            _speed = Mathf.Min(_speed, _maxSpeed);

            Vector3 desired_velocity = to_target / distance * _speed;

            return (desired_velocity - _velocity);
        }

        return Vector3.zero;
    }
}
