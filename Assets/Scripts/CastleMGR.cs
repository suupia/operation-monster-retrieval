using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMGR : Facility
{
    public override void SetDirection(Vector2 directionVector)
    {
        //Castle‚ÍŒü‚«‚ğ•Ï‚¦‚é‚±‚Æ‚Í‚È‚¢
    }
    public override void Die()
    {

        Debug.Log($"HP‚ª0ˆÈ‰º‚É‚È‚Á‚½‚Ì‚ÅAé‚ğ”j‰ó‚µ‚Ü‚· gridPos:{gridPos}‚Ìé");

        GameManager.instance.WinTheGame();

        Destroy(this.gameObject);
    }
}
