using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility : MonoBehaviour
{
    Animator animator;

    [SerializeField] protected Vector2Int gridPos;

    [SerializeField] protected Vector2Int targetCharacterPos;

    [SerializeField] protected int level;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected int atk;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected int attackRange;

    protected bool isAttacking = false;

    protected bool isFristBattle = true;

    //プロパティ
     public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <=0)
            {
                Destroy();
            }

        }
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        gridPos = GameManager.instance.ToGridPosition(transform.position);

    }

    public abstract void Destroy();

}
