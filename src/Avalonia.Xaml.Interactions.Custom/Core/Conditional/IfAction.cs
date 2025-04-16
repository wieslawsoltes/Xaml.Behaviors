using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Xaml.Interactions.Custom;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class IfAction : IfElseActionBase
{
    public IfAction()
    {
        ConditionType = ConditionType.If;
    }
}
