using Microsoft.CodeAnalysis;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateDiagnostic = new(
            id: "XBG001",
            title: "Unsupported trigger delegate",
            messageFormat: "Event '{0}' uses delegate '{1}' which is not supported for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateReturnTypeDiagnostic = new(
            id: "XBG002",
            title: "Unsupported trigger delegate return type",
            messageFormat: "Event '{0}' delegate '{1}' must return void for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateOutParameterDiagnostic = new(
            id: "XBG003",
            title: "Unsupported trigger delegate parameter",
            messageFormat: "Event '{0}' delegate '{1}' uses an out parameter which is not supported for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerEventNotFoundDiagnostic = new(
            id: "XBG004",
            title: "Event not found",
            messageFormat: "Event '{0}' could not be found on type '{1}' for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ChangePropertyNotFoundDiagnostic = new(
            id: "XBG005",
            title: "Property not found",
            messageFormat: "Property '{0}' could not be found on type '{1}' for change property generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ActionMethodNotFoundDiagnostic = new(
            id: "XBG006",
            title: "Action method not found",
            messageFormat: "Method '{0}' could not be found on type '{1}' for action generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ActionMethodAmbiguousDiagnostic = new(
            id: "XBG007",
            title: "Action method ambiguous",
            messageFormat: "Method '{0}' on type '{1}' has multiple overloads; action generation requires an unambiguous target",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor GenericMemberNotSupportedDiagnostic = new(
            id: "XBG008",
            title: "Generic members not supported",
            messageFormat: "Member '{0}' uses generic type parameters which are not supported for typed generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ActionParameterModifierNotSupportedDiagnostic = new(
            id: "XBG009",
            title: "Unsupported parameter modifier",
            messageFormat: "Parameter '{1}' on method '{0}' uses modifier '{2}' which is not supported for action generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor StaticMemberNotSupportedDiagnostic = new(
            id: "XBG010",
            title: "Static member not supported",
            messageFormat: "Member '{0}' is static and cannot be used for typed generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor InvalidMultiDataTriggerTargetDiagnostic = new(
            id: "XBG011",
            title: "Invalid multi data trigger target",
            messageFormat: "Type '{0}' must derive from Avalonia.Xaml.Interactivity.StyledElementTrigger to use GenerateTypedMultiDataTrigger",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor InvalidInvokeCommandTargetDiagnostic = new(
            id: "XBG012",
            title: "Invalid invoke command action target",
            messageFormat: "Type '{0}' must derive from Avalonia.Xaml.Interactivity.StyledElementAction to use GenerateTypedInvokeCommandAction",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor MultiDataTriggerEvaluateMissingDiagnostic = new(
            id: "XBG013",
            title: "Evaluate method required",
            messageFormat: "Type '{0}' must declare a non-static bool Evaluate() method for multi data trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor MemberNotAccessibleDiagnostic = new(
            id: "XBG014",
            title: "Member not accessible",
            messageFormat: "Member '{0}' on type '{1}' must be accessible for typed generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor PropertySetterNotAccessibleDiagnostic = new(
            id: "XBG015",
            title: "Property setter not accessible",
            messageFormat: "Property '{0}' on type '{1}' must have an accessible setter for change property generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor PartialTypeRequiredDiagnostic = new(
            id: "XBG016",
            title: "Type must be partial",
            messageFormat: "Type '{0}' must be declared partial to use '{1}'",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor InitOnlySetterNotSupportedDiagnostic = new(
            id: "XBG017",
            title: "Init-only setter not supported",
            messageFormat: "Property '{0}' on type '{1}' has an init-only setter which is not supported for change property generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor NestedTypeNotSupportedDiagnostic = new(
            id: "XBG018",
            title: "Nested types not supported",
            messageFormat: "Type '{0}' must be a top-level type for typed generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
