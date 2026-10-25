#!/usr/bin/env bash
set -euo pipefail
project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
unity_binary="${SLINGSHOT_UNITY:-/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity}"
build_method=ModernBuild.Android
case "${1:-release}" in
  release) ;;
  development) build_method=ModernBuild.AndroidDevelopment ;;
  *) echo 'Usage: scripts/build_android.sh [release|development]' >&2; exit 2 ;;
esac
mkdir -p "$project_root/work/logs"
"$unity_binary" -batchmode -nographics -accept-apiupdate \
  -projectPath "$project_root/unity" -buildTarget Android \
  -executeMethod "$build_method" -quit \
  -logFile "$project_root/work/logs/android-${1:-release}.log"
