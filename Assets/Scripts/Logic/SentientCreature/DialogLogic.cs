using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogLogic : InterfaceLogicBase
{
    public static DialogLogic I;

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitDialog(newBase as IDialog);
    }
    private void InitDialog(IDialog dialog)
    {
        if (dialog == null)
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