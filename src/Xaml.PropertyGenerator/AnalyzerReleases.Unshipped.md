; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
XPG0001 | XamlPropertyGenerator | Error | Generated property must be a partial property
XPG0002 | XamlPropertyGenerator | Error | Containing type must be partial
XPG0003 | XamlPropertyGenerator | Error | No supported XAML platform was found
XPG0004 | XamlPropertyGenerator | Error | Unsupported property shape
XPG0005 | XamlPropertyGenerator | Info | Option is not supported by the target platform
