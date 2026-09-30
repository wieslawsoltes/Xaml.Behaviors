#!/usr/bin/env bash
# Compares the public API of the Avalonia assemblies against a baseline directory (strict mode).
# Usage: build/UnoPort/api-compat.sh <baseline-dir> [Configuration]
# Create the baseline by building the solution before a refactoring and copying
# src/Xaml.Behaviors*/bin/<Configuration>/net10.0/Xaml.Behaviors*.dll into <baseline-dir>.
set -euo pipefail
baseline="$1"
configuration="${2:-Release}"
root="$(cd "$(dirname "$0")/../.." && pwd)"
tool="$(ls -d "$HOME"/.nuget/packages/microsoft.dotnet.apicompat.tool/*/tools/net8.0/any/Microsoft.DotNet.ApiCompat.Tool.dll | tail -1)"
framework="$(ls -d "$(dirname "$(command -v dotnet)")"/packs/Microsoft.NETCore.App.Ref/10.*/ref/net10.0 2>/dev/null | tail -1)"
[ -z "$framework" ] && framework="$(ls -d "$HOME"/.dotnet/packs/Microsoft.NETCore.App.Ref/10.*/ref/net10.0 | tail -1)"
avalonia="$(ls -d "$HOME"/.nuget/packages/avalonia/*/ref/net10.0 | tail -1)"
status=0
for left in "$baseline"/Xaml.Behaviors*.dll; do
  name="$(basename "$left" .dll)"
  right="$(ls "$root"/src/"$name"/bin/"$configuration"/net10.0/"$name".dll 2>/dev/null || true)"
  [ -z "$right" ] && { echo "skip $name (not built)"; continue; }
  refs="$framework,$avalonia,$(dirname "$right")"
  if dotnet "$tool" -l "$left" -r "$right" --left-assembly-references "$framework,$avalonia,$baseline" --right-assembly-references "$refs" --strict-mode 2>&1 | grep -v "Could not resolve reference" ; then :; fi
  if ! dotnet "$tool" -l "$left" -r "$right" --left-assembly-references "$framework,$avalonia,$baseline" --right-assembly-references "$refs" --strict-mode >/dev/null 2>&1; then
    echo "API differences in $name"; status=1
  fi
done
exit $status
