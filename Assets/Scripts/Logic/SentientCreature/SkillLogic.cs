using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum SkillType
{ 
    MELEE, CONSTRUCTION, PERSUASION, PEWPEW, APPRAISAL, TRICKS, HISTORY, SHOOTING
}
public enum SkillCheckQuality 
{ 
    FAIL, BAD, DECENT, GOOD, GREAT, EXCELLENT, PERFECT, MARVELLOUS
}

public class SkillLogic : InterfaceLogicBase
{
    public static SkillLogic I;
    public List<Skill> skillTypes;
    public int skillMin = 0;
    public int skillMax = 100;
    private System.Random random = new System.Random();
    private float dispositionMultiplier = 10;

    protected override void RegisterInstances()
    {
        base.RegisterInstances();
        CreateUndefinedSkills();
    }

    private void CreateUndefinedSkills()
    {
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType))) {
            if (skillTypes.Find(x => x.skillType == skillType) != null)
                continue;
            skillTypes.Add(new Skill(skillType,"Undescribed",0));
        }
    }


    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitSkilled(newBase as ISkilled);
    }
    private void InitSkilled(ISkilled skilled)
    {
        if (skilled == null)
            return;
        skilled.onExperienceGain = new SkillEvent();
        skilled.onLevelUp = new SkillEvent();
        skilled.onSkillCheck = new SkillCheckEvent();
        AddSkills(skilled);
    }

    private void AddSkills(ISkilled skilled)
    {
        skillTypes.ForEach(x => skilled.GetSkills().Add(new Skill(x)));
    }

    public bool SkillCheck(out SkillCheckQuality quality, ISkilled skilled, SkillType skillType, int difficulty, SkillCheckQuality requirement = SkillCheckQuality.BAD) {
        quality = SkillCheckQuality.FAIL;
        Skill skill = skilled.GetSkills().Find(x => x.skillType == skillType);
        int roll = skill.value + random.Next(skillMin, skillMax) + GetRollBonus(skilled);
        if (roll < difficulty) {
            skilled.onSkillCheck.Invoke(skilled, skill, quality, false);
            return false;
        }
        int qualityValue = roll - difficulty;
        quality = GetQuality(qualityValue);
        bool result = quality >= requirement;
        skilled.onSkillCheck.Invoke(skilled, skill, quality, result);
        return result;
    }

    private int GetRollBonus(ISkilled skilled)
    {
        Disposition disposition = SentientCreatureLogic.I.GetTotalDisposition(skilled);
        return Mathf.RoundToInt((disposition.mood + disposition.passion - (disposition.stress * 2)) * dispositionMultiplier);
    }

    public void GainExperience(ISkilled skilled, SkillType skillType, float experience) {
        Skill skill = skilled.GetSkills().Find(x => x.skillType == skillType);
        skill.experience += experience * GetExperienceMultiplier(skilled as ISentient);
        skilled.onExperienceGain.Invoke(skilled, skill);
        if (!IsExperienceEnough(skill))
            return;
        LevelUp(skilled, skill);
    }

    private float GetExperienceMultiplier(ISentient sentient)
    {
        return (SentientCreatureLogic.I.GetTotalDisposition(sentient).passion / 2) + 0.75f;
    }

    private void LevelUp(ISkilled skilled, Skill skill)
    {
        skill.value += 1;
        skill.experience = 0;
        skilled.onLevelUp.Invoke(skilled, skill);
    }

    private bool IsExperienceEnough(Skill skill)
    {
        return skill.experience > skill.value;
    }

    private SkillCheckQuality GetQuality(int qualityValue)
    {
        if (qualityValue < 10)
            return SkillCheckQuality.BAD;
        if (qualityValue < 30)
            return SkillCheckQuality.DECENT;
        if (qualityValue < 50)
            return SkillCheckQuality.GOOD;
        if (qualityValue < 70)
            return SkillCheckQuality.GREAT;
        if (qualityValue < 100)
            return SkillCheckQuality.EXCELLENT;
        if (qualityValue < 130)
            return SkillCheckQuality.PERFECT;
        return SkillCheckQuality.MARVELLOUS;
    }

    public float ReduceBySkill(ISkilled skilled, SkillType skillType, float value)
    {
        Skill skill = skilled.GetSkills().Find(x => x.skillType == skillType);
        if (skill == null)
            return value;
        float multiplier = Mathf.Clamp((float)skill.value, 1, skillMax) / (float)skillMax;
        return Mathf.Clamp(value - (value * multiplier), 0, float.MaxValue);
    }
}
public interface ISkilled : ISentient {
    List<Skill> GetSkills();
    SkillEvent onExperienceGain { get; set; }
    SkillEvent onLevelUp { get; set; }
    SkillCheckEvent onSkillCheck { get; set; }
}

public class SkillEvent : UnityEvent<ISkilled, Skill> { }
public class SkillCheckEvent : UnityEvent<ISkilled, Skill, SkillCheckQuality, bool> { }

[System.Serializable]
public class Skill {
    public SkillType skillType;
    public string description;
    public int value;
    public float experience = 0;
    public Skill(Skill skill)
    {
        this.skillType = skill.skillType;
        this.description = skill.description;
        this.value = skill.value;
    }
    public Skill(SkillType skillType, string description, int value)
    {
        this.skillType = skillType;
        this.description = description;
        this.value = value;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is Skill))
            return false;
        return this.skillType == (obj as Skill).skillType;
    }
    public override int GetHashCode()
    {
        return 363513814 + EqualityComparer<SkillType>.Default.GetHashCode(skillType);
    }
}
