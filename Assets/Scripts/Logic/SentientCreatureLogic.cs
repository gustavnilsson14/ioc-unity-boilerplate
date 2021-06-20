using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskType
{
    NONE, FLEE, FIGHT, EAT, SLEEP, WORK, SOCIALIZE, THINK
}

public class SentientCreatureLogic : InterfaceLogicBase
{
    public static SentientCreatureLogic I;
    private static float favorForgetThreshold = 0.1f;


    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitSentient(newBase as ISentient);
    }
    private void InitSentient(ISentient sentient)
    {
        if (sentient == null)
            return;
        sentient.generalDisposition = sentient.GetInitialDisposition();
        sentient.currentDisposition = Disposition.Default;
        sentient.bonds = new Dictionary<int, float>();
        sentient.tasks = new List<Task>();
    }

    public void SetCurrentDisposition(ISentient sentient, Disposition disposition)
    {
        Disposition dispositionDelta = disposition - Disposition.Default;
        Debug.Log(dispositionDelta / 100);
        sentient.generalDisposition += dispositionDelta / 100;
        sentient.currentDisposition = disposition;
    }
    public Disposition GetTotalDisposition(ISentient sentient)
    {
        return (sentient.currentDisposition / 2) + (sentient.generalDisposition / 2);
    }
    public float GetHostility(ISentient sentient)
    {
        return Mathf.Clamp(GetTotalDisposition(sentient).passion + (GetTotalDisposition(sentient).stress * 2) - (GetTotalDisposition(sentient).mood * 2), 0, 1);
    }
    public float GetCalm(ISentient sentient)
    {
        return Mathf.Clamp(GetTotalDisposition(sentient).mood + 1 - (GetTotalDisposition(sentient).stress * 2), 0, 1);
    }
    public float GetHappyness(ISentient sentient)
    {
        return Mathf.Clamp((GetTotalDisposition(sentient).mood * 1.1f) + GetTotalDisposition(sentient).passion - (GetTotalDisposition(sentient).stress * 2), 0, 1);
    }
    public void SetBond(ISentient sentient, ISentient bondedCreature, float favor)
    {
        if (!sentient.bonds.ContainsKey(bondedCreature.uniqueId))
        {
            sentient.bonds.Add(bondedCreature.uniqueId, favor);
            return;
        }
        sentient.bonds[bondedCreature.uniqueId] += favor;
    }
    public void UpdateBonds(ISentient sentient)
    {
        List<int> forgetList = new List<int>();
        foreach (KeyValuePair<int, float> entry in sentient.bonds)
        {
            if (!UpdateBond(out int idToForget, sentient, entry.Key, entry.Value))
                continue;
            forgetList.Add(idToForget);
        }
        forgetList.ForEach(x => sentient.bonds.Remove(x));
    }
    private bool UpdateBond(out int idToForget, ISentient sentient, int uniqueId, float favor)
    {
        idToForget = 0;
        float newFavor = Mathf.Lerp(favor, 0, 0.1f);
        if (newFavor < SentientCreatureLogic.favorForgetThreshold)
        {
            idToForget = uniqueId;
            return true;
        }
        sentient.bonds[uniqueId] = newFavor;
        return false;
    }

    public void AddTask(ISentient sentient, Task newTask) {
        sentient.tasks.Add(newTask);
        sentient.tasks = sentient.tasks.OrderByDescending(x => (x.weightOverride == -1) ? x.weight : x.weightOverride).ToList();
    }

    public bool IsBusyForSubTask(ISentient sentient, Task task)
    {
        if (sentient.activeTasks.Find(x => x.task == task.task) != null)
            return true;
        return sentient.GetMaxFocus() <= sentient.activeTasks.Sum(x => x.target.GetRequiredFocus()) + task.target.GetRequiredFocus();
    }

    public void WorkOnTask(ISentient sentient, Task task) {
        sentient.activeTasks.Find(x => x == task);
    }
}
public interface ISentient : IBase
{
    Disposition generalDisposition { get; set; }
    Disposition currentDisposition { get; set; }
    Disposition GetInitialDisposition();
    int GetMaxFocus();
    List<Task> tasks { get; set; }
    List<Task> activeTasks { get; set; }
    /// <summary>
    /// This creatures bonds
    /// The key is the id of the creature as an int, the value is the favor for the creature as a float
    /// </summary>
    Dictionary<int, float> bonds { get; set; }
}
public interface ITaskTarget : IBase
{
    List<TaskType> GetTaskTypes();
    int GetRequiredFocus();
}

[System.Serializable]
public class Task : IEquatable<Task>
{
    public ITaskTarget target;
    public TaskType task;
    public int remainingEffort;
    public int weight;
    public int weightOverride = -1;
    public Task(Task task)
    {
        this.target = task.target;
        this.task = task.task;
        this.remainingEffort = task.remainingEffort;
        this.weight = task.weight;
        this.weightOverride = task.weightOverride;
    }
    public Task(ITaskTarget target, TaskType task, int remainingEffort, int weight, int weightOverride = -1)
    {
        this.target = target;
        this.task = task;
        this.remainingEffort = remainingEffort;
        this.weight = weight;
        this.weightOverride = weightOverride;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Task);
    }

    public bool Equals(Task other)
    {
        return other != null &&
               EqualityComparer<ITaskTarget>.Default.Equals(target, other.target) &&
               task == other.task;
    }

    public override int GetHashCode()
    {
        int hashCode = -1159314444;
        hashCode = hashCode * -1521134295 + EqualityComparer<ITaskTarget>.Default.GetHashCode(target);
        hashCode = hashCode * -1521134295 + task.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(Task left, Task right)
    {
        return EqualityComparer<Task>.Default.Equals(left, right);
    }

    public static bool operator !=(Task left, Task right)
    {
        return !(left == right);
    }
}



[System.Serializable]
public class Disposition
{
    public float mood { get; private set; }
    public float passion { get; private set; }
    public float stress { get; private set; }
    public static Disposition operator *(Disposition a, Disposition b)
    {
        return new Disposition(a.mood * b.mood, a.passion * b.passion, a.stress * b.stress);
    }
    public static Disposition operator /(Disposition a, float b)
    {
        return new Disposition(a.mood / b, a.passion / b, a.stress / b);
    }
    public static Disposition operator +(Disposition a, Disposition b)
    {
        return new Disposition( 
            Mathf.Clamp(a.mood + b.mood, 0, 1),
            Mathf.Clamp(a.passion + b.passion, 0, 1),
            Mathf.Clamp(a.stress + b.stress, 0, 1)
        );
    }
    public static Disposition operator -(Disposition a, Disposition b)
    {
        return new Disposition(a.mood - b.mood, a.passion - b.passion, a.stress - b.stress);
    }
    public Disposition(float mood, float passion, float stress) {
        this.mood = mood;
        this.passion = passion;
        this.stress = stress;
    }
    public static Disposition Default => new Disposition(0.5f, 0.5f, 0.5f);
    public override string ToString()
    {
        return $"{base.ToString()} MOOD: {mood}, PASSION: {passion}, STRESS: {stress}";
    }
}