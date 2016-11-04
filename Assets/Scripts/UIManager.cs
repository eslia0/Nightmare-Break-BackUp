using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
	private Image hpUI;
    [SerializeField]
    private Image mpUI;
    [SerializeField]
    private Image potionUI;
    private const float checkTime = 0.1f;
    [SerializeField]
    private Image[] skillUI; // 0 - SKill1 // 1 - SKill2 // 2 - Skill3 // 3 - Skill4 //
    private CharacterStatus characterInfo;
    private CharacterManager characterSkill;

    void Awake()
    {
        //characterInfo = GameObject.FindWithTag("Player").GetComponent<CharacterStatus>();
        //characterSkill = GameObject.FindWithTag("Player").GetComponent<CharacterManager>();

    }

    void SetCharacter()
    {
        characterInfo = GameObject.FindWithTag("Player").GetComponent<CharacterStatus>();
        characterSkill = GameObject.FindWithTag("Player").GetComponent<CharacterManager>();
    }

    /*
    IEnumerator HealthPointUI()
    {
        while (true)
        {
            hpUI.fillAmount = (float)(1 / characterInfo.healthPoint);
            yield return new WaitForSeconds(checkTime);
        }
    }

    IEnumerator ManaPointUI()
    {
        while (true)
        {
            mpUI.fillAmount = (float)(1 / characterInfo.manaPoint);
            yield return new WaitForSeconds(checkTime);
        }
    }

    IEnumerator SkillCoolTimeUI()
    {
        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                skillUI[i].fillAmount = 1 / characterSkill.skillCoolTime[i];
            }

            yield return new WaitForSeconds(checkTime);
        }
    }

    IEnumerator PotionCoolTimeUI()
    {
        while (true)
        {
            potionUI.fillAmount = (float)(characterSkill.skillCoolTime[4]);
            yield return new WaitForSeconds(checkTime);
        }
    }
    */


}
