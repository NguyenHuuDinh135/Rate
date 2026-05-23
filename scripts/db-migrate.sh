#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet ef database update \
  --project "${ROOT_DIR}/src/Infrastructure/Infrastructure.csproj" \
  --startup-project "${ROOT_DIR}/src/Web/Web.csproj"

