using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    None,
    Fire,
    Water,
    Wind,
    Earth
}

[Serializable]
public struct SkillData
{
    public SkillType skillType;
    public int skillPower;
    public float skillCooldown;
}

[Serializable]
public struct PlayerData
{
    public SkillData skillData;
    public int money;
}

public class PlayerCtrl : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private List<SkillData> _skillDataList;

    // ---------------------------- Field
    public PlayerData PlayerData;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        PlayerData = new PlayerData
        {
            skillData = new SkillData { skillType = SkillType.None, skillPower = 0, skillCooldown = 0f },
            money = 1000
        };
    }



    // ---------------------------- PublicMethod
    public void LoadPlayer()
    {
        Debug.Log("Player data loaded.");
        PlayerData.money = PlayerPrefs.GetInt("Money", 1000);
        var skillTypeString = PlayerPrefs.GetString("SkillType", SkillType.None.ToString());
        if (Enum.TryParse(skillTypeString, out SkillType skillType))
        {
            SetCurrentSkill(skillType);
        }
        else
        {
            SetCurrentSkill(SkillType.None);
        }
    }

    public void SavePlayer()
    {
        Debug.Log("Player data saved.");
        PlayerPrefs.SetInt("Money", PlayerData.money);
        PlayerPrefs.SetString("SkillType", PlayerData.skillData.skillType.ToString());
    }

    public void GetItem(StoreProcess.ItemData data)
    {
        var price = data.itemPrice;



        var type = data.itemType;

        switch (type)
        {
            case StoreProcess.ItemType.Skill_Fire:
                SetCurrentSkill(SkillType.Fire);
                break;
            case StoreProcess.ItemType.Skill_Water:
                SetCurrentSkill(SkillType.Water);
                break;
            case StoreProcess.ItemType.Skill_Wind:
                SetCurrentSkill(SkillType.Wind);
                break;
            case StoreProcess.ItemType.Skill_Earth:
                SetCurrentSkill(SkillType.Earth);
                break;
            default:
                Debug.Log("Item is not a skill.");
                break;
        }
    }


    public void SetCurrentSkill(SkillType skillType)
    {
        PlayerData.skillData = _skillDataList.Find(skill => skill.skillType == skillType);

    }

    // ---------------------------- PrivateMethod

    private void UseSkill()
    {
        var skillData = PlayerData.skillData;
        if (skillData.skillType != SkillType.None)
        {
            Debug.Log($"Using skill: {skillData.skillType} with power {skillData.skillPower} and cooldown {skillData.skillCooldown}");
            // Implement skill usage logic here
        }
        else
        {
            Debug.Log("No skill selected or skill not found.");
        }
    }



}
