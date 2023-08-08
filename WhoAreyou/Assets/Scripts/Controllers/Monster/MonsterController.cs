using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class MonsterController : MonoBehaviour
{

    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected Vector3 _posTarget;
    [SerializeField]
    protected bool _isDeath;
    [SerializeField]
    protected bool _isAttack;
    [SerializeField]
    protected float limitX;
    [SerializeField]
    protected float limitZ;
    [SerializeField]
    float minTime = 2;
    [SerializeField]
    float maxTime = 7;
    [SerializeField]
    protected bool _isBattle;
    [SerializeField]
    protected float _walkSpeed;
    [SerializeField]
    protected float _runSpeed;
    [SerializeField]
    eCharacter _eCh;
    [SerializeField]
    protected bool isFounded;
    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;

    [SerializeField]
    BoxCollider weapon;

    protected NavMeshAgent nav;
    protected Vector3 _startPos;
    [SerializeField]
    protected float _timeWait;
    protected bool _isSelectAi;
    protected int _characterStd;


    Stat stat;

    [SerializeField]
    Slider hpBar;


    protected enum eActionState
    {
        IDLE = 0,
        WALK,
        RUN,
        ATTACK,
        HIT,
        DIE,
    }

    protected  enum eCharacter
    {
        Fierce = 0,
        Lazy
    }

    [SerializeField]
    protected eActionState _stateAction;

    protected Animator _ctrlAnim;

    private void Awake()
    {
        Stat stat = GetComponent<Stat>();
        stat.Hp = stat.MaxHp;
    }
    protected void Start()
    {
        Init();
        stat = GetComponent<Stat>();
        switch (_eCh)
        {
            case eCharacter.Fierce:
                _characterStd = 25;
                break;
            case eCharacter.Lazy:
                _characterStd = 80;
                break;
        }
    }

    protected void Update()
    {
        if (_isDeath)
        {
            ChangedAction(eActionState.DIE);
            return;
        }
        hpBar.value = (float)stat.Hp / (float)stat.MaxHp;
        switch (_stateAction)
        {
            case eActionState.IDLE:
                if (_timeWait <= 0)
                {
                    // 다시 선택
                    _isSelectAi = false;
                }
                else
                {
                    _timeWait -= Time.deltaTime;
                }
                break;
            case eActionState.WALK:
                if (Vector3.Distance(_posTarget, transform.position) <= 0.3f)
                {
                    // 다시 선택
                    _isSelectAi = false;
                }
                break;
            case eActionState.RUN:
                if (!_isAttack && Vector3.Distance(_destPos, transform.position) <= _attackRange)
                {
                    // 플레이어와의 거리가 _attackRange 내에 있고, 공격 중이 아닌 경우
                    _isAttack = true;
                    nav.velocity = Vector3.zero;
                    ChangedAction(eActionState.ATTACK);
                    transform.LookAt(_destPos);
                }
                else if (_isAttack && Vector3.Distance(_destPos, transform.position) > _attackRange)
                {
                    // 플레이어와의 거리가 _attackRange 밖에 있고, 공격 중인 경우
                    _isAttack = false;
                }
                break;
            case eActionState.ATTACK:
                if (Vector3.Distance(_destPos, transform.position) > _attackRange)
                {
                    // 공격 범위를 벗어나면 다시 추적 상태로 전환
                    _isAttack = false;
                    ChangedAction(eActionState.RUN);
                }
                else if (!_isAttack)
                {
                    // 공격 로직 실행
                    _isAttack = true;
                }
                break;
            case eActionState.HIT:
                Debug.Log(stat.Hp);
                if(stat.Hp <= 0)
                {
                    _isDeath = true;
                }
                break;
        }

        ProcessAI();
        Sight();
    }

    public abstract void Init();



    protected void ChangedAction(eActionState state)
    {
        switch (state)
        {
            case eActionState.IDLE:
                _stateAction = state;
                _ctrlAnim.SetInteger("state", (int)_stateAction);
                break;
            case eActionState.WALK:
                if (_stateAction != eActionState.WALK)
                {
                    nav.speed = _walkSpeed;
                    nav.stoppingDistance = 0;
                    _stateAction = state;
                    _ctrlAnim.SetInteger("state", (int)_stateAction);
                }
                break;
            case eActionState.RUN:
                nav.speed = _runSpeed;
                nav.stoppingDistance = _attackRange;
                _stateAction = state;
                _ctrlAnim.SetInteger("state", (int)_stateAction);
                break;
            case eActionState.ATTACK:
                if (_stateAction == eActionState.RUN || _stateAction == eActionState.HIT)
                {
                    _isAttack = false;
                    _stateAction = state;
                    _ctrlAnim.SetInteger("state", (int)_stateAction);
                }
                break;
            case eActionState.HIT:
                _stateAction = state;
                _ctrlAnim.SetInteger("state", (int)_stateAction);
                break;
            case eActionState.DIE:
                _stateAction = state;
                _ctrlAnim.SetInteger("state", (int)_stateAction);
                break;
        }
    }


    void ProcessAI()
    {
        if (!_isSelectAi)
        {
            int r = Random.Range(0, 100);
            if (r > 80)
            {
                // 대기.
                ChangedAction(eActionState.IDLE);
                _timeWait = Random.Range(minTime, maxTime);
            }
            else
            { // 걷기
                if (!_isAttack) // 추가된 조건: 공격 중이 아닐 때만 걷기 상태로 변경
                {
                    ChangedAction(eActionState.WALK);
                    _posTarget = GetRandomPos(_startPos, limitX, limitZ);
                    nav.SetDestination(_posTarget);
                }
            }
            _isSelectAi = true;
        }
    }
    protected Vector3 GetRandomPos(Vector3 center, float limitX, float limitZ)
    {
        float rx = Random.Range(-limitX, limitX);
        float rz = Random.Range(-limitZ, limitZ);

        Vector3 rv = new Vector3(rx, 0, rz);
        return center + rv;
    }

    protected void OnWeapon()
    {
        weapon.enabled = true;
    }

    protected void OffWeapon()
    {
        weapon.enabled = false;
    }

    void DestroyMonster()
    {
        Destroy(gameObject);
    }



    protected void Sight()
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, _scanRange);

        bool playerDetected = false; // 플레이어를 감지했는지 여부를 추적

        foreach (Collider col in t_cols)
        {
            // 플레이어 감지
            if (col.CompareTag("Player"))
            {
                playerDetected = true;
                _destPos = col.transform.position;
                break;
            }
        }

        if (playerDetected)
        {
            if (!_isAttack && Vector3.Distance(_destPos, transform.position) <= _attackRange)
            {
                // 플레이어와의 거리가 _attackRange 내에 있고, 공격 중이 아닌 경우
                _isAttack = true;
                ChangedAction(eActionState.ATTACK);
            }
            else if (!_isAttack)
            {
                // 플레이어와의 거리가 _attackRange 밖에 있고, 공격 중이 아닌 경우
                ChangedAction(eActionState.RUN);
                nav.SetDestination(_destPos); // 플레이어에게 접근하도록 네비게이션 목적지 설정
            }
            else if (_isAttack && Vector3.Distance(_destPos, transform.position) > _attackRange)
            {
                // 플레이어와의 거리가 _attackRange 밖에 있고, 공격 중인 경우
                ChangedAction(eActionState.RUN);
                nav.SetDestination(_destPos); // 플레이어에게 접근하도록 네비게이션 목적지 설정
            }
        }
        else
        {
            // 플레이어를 감지하지 못한 경우
            if (_isAttack)
            {
                _isAttack = false;
                ChangedAction(eActionState.IDLE); // 플레이어가 범위를 벗어나면 Idle 상태로 전환
                _timeWait = Random.Range(minTime, maxTime);
            }
        }
    }

    private void HitEvent()
    {
        ChangedAction(eActionState.ATTACK);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = GameObject.Find("Player");
        if(other.CompareTag("weapon"))
        {
            stat.OnAttacked(player.GetComponent<Stat>());
            ChangedAction(eActionState.HIT);
        }
    }




}



