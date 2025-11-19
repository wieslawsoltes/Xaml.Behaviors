#!/bin/bash
export IsDocFx=true
dotnet tool restore
dotnet docfx docfx/docfx.json
