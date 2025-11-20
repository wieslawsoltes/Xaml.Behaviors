# SplitViewStateSetter

Describes a state for `SplitViewStateBehavior` using size conditions.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| MinWidth | `double` | Gets or sets minimum bounds width. |
| MinWidthOperator | `ComparisonConditionType` | Gets or sets minimum width comparison operator. Default is `GreaterThanOrEqual`. |
| MaxWidth | `double` | Gets or sets maximum bounds width. Default is `PositiveInfinity`. |
| MaxWidthOperator | `ComparisonConditionType` | Gets or sets maximum width comparison operator. Default is `LessThan`. |
| MinHeight | `double` | Gets or sets minimum bounds height. |
| MinHeightOperator | `ComparisonConditionType` | Gets or sets minimum height comparison operator. Default is `GreaterThanOrEqual`. |
| MaxHeight | `double` | Gets or sets maximum bounds height. Default is `PositiveInfinity`. |
| MaxHeightOperator | `ComparisonConditionType` | Gets or sets maximum height comparison operator. Default is `LessThan`. |
| DisplayMode | `SplitViewDisplayMode` | Gets or sets the `SplitView.DisplayMode` to apply. |
| PanePlacement | `SplitViewPanePlacement` | Gets or sets the `SplitView.PanePlacement` to apply. |
| IsPaneOpen | `bool` | Gets or sets the `SplitView.IsPaneOpen` to apply. |
| TargetSplitView | `SplitView` | Gets or sets the target `SplitView`. If not set, the behavior's associated object is used. |
