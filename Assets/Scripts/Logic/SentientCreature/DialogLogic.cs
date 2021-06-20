using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogLogic : InterfaceLogicBase
{
    public static DialogLogic I;

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitDialog(newInstance);
    }
    private void InitDialog(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IDialog>(out IDialog dialog))
            return;
    }
}
public interface ITalker : ISentient 
{ 
    bool isTalking { get; set; }
}
public interface IDialog : IBase
{

}