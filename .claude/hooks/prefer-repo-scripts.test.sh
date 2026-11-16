#!/usr/bin/env bash
# SPDX-FileCopyrightText: 2026 The Istarnova Contributors
# SPDX-License-Identifier: GPL-3.0-or-later

#
# Checks prefer-repo-scripts.sh against the commands it must refuse and the ones it must not break.
#
# The DENY block is not invented: every line is a command actually run while the hook was in place
# and silent (in PersonalMediRecords, where it comes from). The ALLOW block is the
# other half of the job - a hook that refuses the working habits gets switched off, and then it
# guards nothing.
#
#   bash .claude/hooks/prefer-repo-scripts.test.sh

set -uo pipefail

hook="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/prefer-repo-scripts.sh"
failures=0

check() {
    local want="$1" cmd="$2"
    local out got
    out="$(jq -nc --arg cmd "$cmd" '{tool_input:{command:$cmd}}' | bash "$hook")"
    [ -z "$out" ] && got="ALLOW" || got="DENY"

    if [ "$got" = "$want" ]; then
        printf '  ok   %-6s %s\n' "$got" "$cmd"
    else
        printf '  FAIL want=%s got=%s  %s\n' "$want" "$got" "$cmd"
        failures=$((failures + 1))
    fi
}

echo "must be refused:"
check DENY 'grep -rn "FluentSelect" Components/'
check DENY '\grep -rn "FluentSelect" Components/'
check DENY '\grep -c "new Unit" UnitConfiguration.cs'
check DENY '\grep -oP "VALUES \(" schema.sql | sort -u | tr "\n" " "'
check DENY 'for f in $(\grep -rl "FluentSelect" .); do echo "$f"; done'
check DENY '\grep -rl "HasData" Configurations | xargs \grep -ln "Unit"'
check DENY '\find /home/kurokyuu/dev -name "Unit.cs" -not -path "*/obj/*"'
check DENY '\sed -n "2377,2400p" deploy/db-init/schema.sql'
check DENY '\cat Scripts/repo.py'
check DENY 'awk "{print \$1}" units.csv'
check DENY 'rg -l "SomeSymbol"'
check DENY 'python3 -c "import os; print(os.getcwd())"'
check DENY "python -c 'print(1)'"
check DENY 'python3 - <<PY
import json
PY'
check DENY "python3 - <<'PY'
import json
PY"
check DENY 'python3 <<PY
print(1)
PY'
check DENY 'python3 -u -c "print(1)"'
check DENY '\ls Data/ | python3 -c "import sys; print(sys.stdin.read())"'
check DENY 'gh issue list'
check DENY 'git commit -m "wip"'
check DENY 'dotnet build'
check DENY 'dotnet test Istarnova.Tests/Istarnova.Tests.csproj'
check DENY 'SOME_FLAG=1 \grep -rn "x" .'
check DENY 'SOME_FLAG=1 \find . -name "*.cs"'
# git grep is a raw sweep behind a subcommand, and it went unnoticed while every honest spelling was
# refused - the same shape as the escape hatch above.
check DENY 'GIT_PAGER=cat git grep -n "Toast" -- Components/'
check DENY 'git grep -ln "HasData"'
# Read-only history went through git directly while repo.py owned the rest; now it has 'log'.
check DENY 'GIT_PAGER=cat git log --oneline -4'
check DENY 'git --no-pager log -5'
check DENY 'GIT_PAGER=cat git -C /home/kurokyuu/dev/projects/dotnet/Istarnova log --oneline -3 -- Istarnova/Extensions/NumberExtensions.cs'

echo
echo "must keep working:"
check ALLOW 'python3 Scripts/dotnet.py verify'
check ALLOW 'python3 Scripts/repo.py pr 414'
check ALLOW 'python3 /home/kurokyuu/dev/projects/dotnet/Istarnova/Scripts/repo.py status'
check ALLOW 'python3 Scripts/csharp_check.py -c config.json'
check ALLOW 'python3 Scripts/repo.py issues | head -40'
check ALLOW 'git status --short'
check ALLOW 'python3 Scripts/find.py AlphanumComparator --lines'
check ALLOW 'python3 Scripts/find.py "CsvHelper" --text'
check ALLOW 'python3 Scripts/spdx.py check'
check ALLOW 'python3 Scripts/dotnet.py test | \grep -c passed'
check ALLOW 'git diff --stat master...HEAD'
check ALLOW 'gh api repos/:owner/:repo'
check ALLOW '\ls -1 Scripts/'
check ALLOW 'wc -l < units.csv'

echo
if [ "$failures" -eq 0 ]; then
    echo "all checks passed"
else
    echo "$failures check(s) failed"
    exit 1
fi
