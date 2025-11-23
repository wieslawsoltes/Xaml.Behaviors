// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private const string GenerateTypedActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedActionAttribute";
        private const string GenerateTypedTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedTriggerAttribute";
        private const string GenerateTypedChangePropertyActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedChangePropertyActionAttribute";
        private const string GenerateTypedDataTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedDataTriggerAttribute";
        private const string GenerateTypedMultiDataTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedMultiDataTriggerAttribute";
        private const string TriggerPropertyAttributeName = "Xaml.Behaviors.SourceGenerators.TriggerPropertyAttribute";
        private const string GenerateTypedInvokeCommandActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedInvokeCommandActionAttribute";
        private const string ActionCommandAttributeName = "Xaml.Behaviors.SourceGenerators.ActionCommandAttribute";
        private const string ActionParameterAttributeName = "Xaml.Behaviors.SourceGenerators.ActionParameterAttribute";

        private static void RegisterAttributeSources(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("GenerateTypedActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedActionAttribute : Attribute
                        {
                            public GenerateTypedActionAttribute()
                            {
                            }

                            public GenerateTypedActionAttribute(Type targetType, string methodName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedTriggerAttribute : Attribute
                        {
                            public GenerateTypedTriggerAttribute()
                            {
                            }

                            public GenerateTypedTriggerAttribute(Type targetType, string eventName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedChangePropertyActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedChangePropertyActionAttribute : Attribute
                        {
                            public GenerateTypedChangePropertyActionAttribute()
                            {
                            }

                            public GenerateTypedChangePropertyActionAttribute(Type targetType, string propertyName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedDataTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedDataTriggerAttribute : Attribute
                        {
                            public GenerateTypedDataTriggerAttribute(Type type)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedMultiDataTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                        internal sealed class GenerateTypedMultiDataTriggerAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class TriggerPropertyAttribute : Attribute
                        {
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedInvokeCommandActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                        internal sealed class GenerateTypedInvokeCommandActionAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class ActionCommandAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class ActionParameterAttribute : Attribute
                        {
                        }
                    }
                    """, Encoding.UTF8));
            });
        }
    }
}
