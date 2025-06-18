// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Avalonia.Xaml.Interactivity;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
internal class MethodHelper
{
    private MethodDescriptor? _cachedMethodDescriptor;
    private readonly List<MethodDescriptor> _methodDescriptors = [];

    public Type? TargetObjectType { get; private set; }

    public MethodDescriptor? FindBestMethod(object? parameter)
    {
        if (parameter is null)
        {
            return _cachedMethodDescriptor;
        }

        var parameterTypeInfo = parameter.GetType().GetTypeInfo();

        MethodDescriptor? mostDerivedMethod = null;

        // Loop over the methods looking for the one whose type is closest to the type of the given parameter.
        foreach (var currentMethod in _methodDescriptors)
        {
            var currentTypeInfo = currentMethod.SecondParameterTypeInfo;

            if (currentTypeInfo is not null && currentTypeInfo.IsAssignableFrom(parameterTypeInfo))
            {
                if (mostDerivedMethod is null || !currentTypeInfo.IsAssignableFrom(mostDerivedMethod.SecondParameterTypeInfo))
                {
                    mostDerivedMethod = currentMethod;
                }
            }
        }

        return mostDerivedMethod ?? _cachedMethodDescriptor;
    }

    public void UpdateTargetType(Type newTargetType, string? methodName)
    {
        if (newTargetType == TargetObjectType)
        {
            return;
        }

        TargetObjectType = newTargetType;

        UpdateMethodDescriptors(methodName);
    }

    public void UpdateMethodDescriptors(string? methodName)
    {
        _methodDescriptors.Clear();
        _cachedMethodDescriptor = null;

        if (string.IsNullOrEmpty(methodName) || TargetObjectType is null)
        {
            return;
        }

        // Find all public methods that match the given name  and have either no parameters,
        // or two parameters where the first is of type Object.
        foreach (var method in TargetObjectType.GetRuntimeMethods())
        {
            if (string.Equals(method.Name, methodName, StringComparison.Ordinal)
                && (method.ReturnType == typeof(void) || method.ReturnType == typeof(Task))
                && method.IsPublic)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    // There can be only one parameterless method of the given name.
                    _cachedMethodDescriptor = new MethodDescriptor(method, parameters);
                }
                else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(object))
                {
                    _methodDescriptors.Add(new MethodDescriptor(method, parameters));
                }
            }
        }

        // We didn't find a parameterless method, so we want to find a method that accepts null
        // as a second parameter, but if we have more than one of these it is ambiguous which
        // we should call, so we do nothing.
        if (_cachedMethodDescriptor is null)
        {
            foreach (var method in _methodDescriptors)
            {
                var typeInfo = method.SecondParameterTypeInfo;
                if (typeInfo is not null && (!typeInfo.IsValueType || typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    if (_cachedMethodDescriptor is not null)
                    {
                        _cachedMethodDescriptor = null;
                        return;
                    }

                    _cachedMethodDescriptor = method;
                }
            }
        }
    }

    [DebuggerDisplay($"{{{nameof(MethodInfo)}}}")]
    public class MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] methodParameters)
    {
        public MethodInfo MethodInfo { get; private set; } = methodInfo;

        public ParameterInfo[] Parameters { get; private set; } = methodParameters;

        public int ParameterCount => Parameters.Length;

        public TypeInfo? SecondParameterTypeInfo
        {
            get => ParameterCount < 2 ? null : Parameters[1].ParameterType.GetTypeInfo();
        }
    }
}
