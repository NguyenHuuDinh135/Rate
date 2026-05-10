#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 ]]; then
  echo "Usage: ./db-add-migration.sh <MigrationName>"
  exit 1
fi

MIGRATION_NAME="$1"
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet ef migrations add "${MIGRATION_NAME}" \
  --project "${ROOT_DIR}/src/Infrastructure/Infrastructure.csproj" \
  --startup-project "${ROOT_DIR}/src/Web/Web.csproj" \
  --output-dir Data/Migrations

