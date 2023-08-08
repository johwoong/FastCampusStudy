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
                    // �ٽ� ����
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
                    // �ٽ� ����
                    _isSelectAi = false;
                }
                break;
            case eActionState.RUN:
                if (!_isAttack && Vector3.Distance(_destPos, transform.position) <= _attackRange)
                {
                    // �÷��̾���� �Ÿ��� _attackRange ���� �ְ�, ���� ���� �ƴ� ���
                    _isAttack = true;
                    nav.velocity = Vector3.zero;
                    ChangedAction(eActionState.ATTACK);
                    transform.LookAt(_destPos);
                }
                else if (_isAttack && Vector3.Distance(_destPos, transform.position) > _attackRange)
                {
                    // �÷��̾���� �Ÿ��� _attackRange �ۿ� �ְ�, ���� ���� ���
                    _isAttack = false;
                }
                break;
            case eActionState.ATTACK:
                if (Vector3.Distance(_destPos, transform.position) > _attackRange)
                {
                    // ���� ������ ����� �ٽ� ���� ���·� ��ȯ
                    _isAttack = false;
                    ChangedAction(eActionState.RUN);
                }
                else if (!_isAttack)
                {
                    // ���� ���� ����
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
                // ���.
                ChangedAction(eActionState.IDLE);
                _timeWait = Random.Range(minTime, maxTime);
            }
            else
            { // �ȱ�
                if (!_isAttack) // �߰��� ����: ���� ���� �ƴ� ���� �ȱ� ���·� ����
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

        bool playerDetected = false; // �÷��̾ �����ߴ��� ���θ� ����

        foreach (Collider col in t_cols)
        {
            // �÷��̾� ����
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
                // �÷��̾���� �Ÿ��� _attackRange ���� �ְ�, ���� ���� �ƴ� ���
                _isAttack = true;
                ChangedAction(eActionState.ATTACK);
            }
            else if (!_isAttack)
            {
                // �÷��̾���� �Ÿ��� _attackRange �ۿ� �ְ�, ���� ���� �ƴ� ���
                ChangedAction(eActionState.RUN);
                nav.SetDestination(_destPos); // �÷��̾�� �����ϵ��� �׺���̼� ������ ����
            }
            else if (_isAttack && Vector3.Distance(_destPos, transform.position) > _attackRange)
            {
                // �÷��̾���� �Ÿ��� _attackRange �ۿ� �ְ�, ���� ���� ���
                ChangedAction(eActionState.RUN);
                nav.SetDestination(_destPos); // �÷��̾�� �����ϵ��� �׺���̼� ������ ����
            }
        }
        else
        {
            // �÷��̾ �������� ���� ���
            if (_isAttack)
            {
                _isAttack = false;
                ChangedAction(eActionState.IDLE); // �÷��̾ ������ ����� Idle ���·� ��ȯ
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



