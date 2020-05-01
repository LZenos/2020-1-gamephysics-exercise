using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPursuit : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    [SerializeField]
    private float _maxSpeed = 0.2f;

    /// <summary>
    /// 예측 수준입니다.
    /// </summary>
    [SerializeField]
    private int _predictLevel = 10;

    private bool _isPursuit = false;

    private Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// Target Agent가 이전 프레임에 있던 위치입니다.
    /// </summary>
    private Vector3 _preTargetPos = Vector3.zero;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPursuit = !_isPursuit;
        }

        if (_isPursuit)
        {
            // pursuit
            // 3: 적당히 빠르게 해주기 위함.
            _velocity = _velocity + pursuit(_target);

            // 속도를 기반으로 새로운 위치 계산.
            transform.position = transform.position + _velocity;

            // 타겟의 속도를 계산하기 위해 타겟의 이전 위치를 저장
            if (_preTargetPos != _target.position) { _preTargetPos = _target.position; }
        }
    }

    private Vector3 pursuit(Transform target_agent)
    {
        Vector3 dir_target = Vector3.Normalize(target_agent.position - transform.position);

        if (Vector3.Dot(dir_target, transform.forward) > 0 && Vector3.Dot(transform.forward, target_agent.forward) < -0.95)
        {
            // 타겟의 이동 방향이 자신과 거의 겹칠 경우 예측하지 않고 쫓아갑니다.
            return seek(target_agent.position);
        }
        else
        {
            // 예측한 장소로 이동합니다.
            return seek(_preTargetPos + ((target_agent.position - _preTargetPos) * Time.deltaTime * (float)(_predictLevel * 1000)));
        }
    }

    private Vector3 seek(Vector3 target_pos)
    {
        // 방향 변경을 위함.
        Vector3 dir = (target_pos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.0f)
        {
            transform.forward = dir;
        }

        Vector3 desired_velocity = dir * _maxSpeed;

        return (desired_velocity - _velocity);
    }
}