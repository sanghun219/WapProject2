using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIinformationMgr : MonoBehaviour
{
    #region SINGLETON
    private static UIinformationMgr m_Inst;
    public static UIinformationMgr GetInst()
    {
        if (m_Inst == null)
        {
            m_Inst = FindObjectOfType(typeof(UIinformationMgr)) as UIinformationMgr;
            if (m_Inst == null)
                Debug.Log("UIinformationMgr isn't exist");
        }

        return m_Inst;
    }
    #endregion
    public Text Life;
    public int lifePoint;
    //boss
    public Image BossLifeBar;
    public Text BossLifeText;
    //이벤트 등록을위해
    public BattleZone battleZone;
    public Boss boss;
    private float bossHp;
    private float bossMaxHp;

    public void Start()
    {        
        battleZone.OnEndBattleZone += ActivateBossUI;
        boss.onDeath += DeActivateBossUI;
    }
    //플레이어 체력 UI
    public void InGameUIupdate()
    {
        Life.text = "X " + lifePoint;
        if (bossHp < 0) return;
        BossLifeBar.fillAmount = bossHp / bossMaxHp;
        BossLifeText.text = string.Format("HP{0}/{1}", bossHp, bossMaxHp);
    }
    

    public void SetPlayerHp(int lifePoint)
    {
        this.lifePoint = lifePoint;
    }

    

    public void SetBossHP(float hp,float MaxHp)
    {
       bossHp = hp;
       bossMaxHp = MaxHp;
    }

    public void ActivateBossUI()
    {
        if (!transform.Find("BossHP_IMG").gameObject.activeInHierarchy)
        {
            transform.Find("BossHP_IMG").gameObject.SetActive(true);
        }
    }

    public void DeActivateBossUI()
    {
        if (transform.Find("BossHP_IMG").gameObject.activeInHierarchy)
        {
            transform.Find("BossHP_IMG").gameObject.SetActive(false);
        }
    }
}
