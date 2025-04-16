using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Xaml.Interactions.Custom;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ElseIfAction : IfElseActionBase
{
    public ElseIfAction()
    {
        ConditionType = ConditionType.ElseIf;
    }
}
