using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMGR : MonoBehaviour
{
    Rigidbody2D rd2D;
    BoxCollider2D boxCollider2D;
    Animator animator;

    [SerializeField] Vector2Int gridPos;

    [SerializeField] int thisCharacterID;

    public bool isAttacking = false;
    public bool isMoving = false;

    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] float attackInterval;  
    [SerializeField] float attackRange;
    [SerializeField] int spd;
    [SerializeField] int coolTime;

    private GameObject damageTextGO;
    [SerializeField] private Text damageText;
    [SerializeField] private int drawDamageTime;

    private Direction direction;

    private enum Direction
    {
        Front,
        Back,
        Right,
        Left,
        DiagRightFront,
        DiagLeftFront,
        DiagRightBack,
        DiagLeftBack
    }

}
