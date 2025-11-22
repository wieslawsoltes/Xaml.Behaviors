using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using System.Windows.Input;

namespace SourceGeneratorSample.ViewModels
{
    [GenerateTypedMultiDataTrigger]
    public partial class ValidationTrigger : StyledElementTrigger
    {
        [TriggerProperty]
        private bool _isValid;

        [TriggerProperty]
        private int _retryCount;

        private bool Evaluate()
        {
            // Trigger when Valid is true AND RetryCount < 3
            return IsValid && RetryCount < 3;
        }
    }

    [GenerateTypedMultiDataTrigger]
    public partial class ValidationResetTrigger : StyledElementTrigger
    {
        [TriggerProperty]
        private bool _isValid;

        [TriggerProperty]
        private int _retryCount;

        private bool Evaluate()
        {
            // Trigger when the previous condition is no longer met
            return !(IsValid && RetryCount < 3);
        }
    }

    [GenerateTypedInvokeCommandAction]
    public partial class TypedCommandAction : StyledElementAction
    {
        [ActionCommand]
        private ICommand? _command;

        [ActionParameter]
        private string? _commandParameter;
    }
}
