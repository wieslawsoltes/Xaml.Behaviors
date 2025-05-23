using System;
using System.Text;

namespace BehaviorsTestApplication.Views.Pages;

public interface ISamplePage
{
    string Header => SamplePageHelper.GenerateHeader(GetType());
}

internal static class SamplePageHelper
{
    public static string GenerateHeader(Type type)
    {
        var name = type.Name;
        if (name.EndsWith("View", StringComparison.Ordinal))
        {
            name = name.Substring(0, name.Length - 4);
        }

        var sb = new StringBuilder();
        if (name.Length > 0)
        {
            sb.Append(name[0]);
            for (var i = 1; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c) && !char.IsUpper(name[i - 1]))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
