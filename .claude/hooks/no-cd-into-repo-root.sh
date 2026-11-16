#!/usr/bin/env bash
# SPDX-FileCopyrightText: 2026 The Istarnova Contributors
# SPDX-License-Identifier: GPL-3.0-or-later

#
# PreToolUse/Bash hook: refuse commands that cd into the repository root.
#
# The repository root is already the working directory and it persists between tool calls, so the
# `cd` is pure noise — and inside a compound command line it turns an otherwise-allowlisted command
# into a unique one, which is what triggers a permission prompt.
#
# Blocked:   cd /path/to/repo && git status
#            R=/path/to/repo; cd $R
# Allowed:   cd Data/icd10gm2026syst-claml   (a subdirectory a script genuinely needs)
#            anything without a cd to the root
#
# Reads the hook payload on stdin, writes a PreToolUse deny decision on stdout when it objects.

set -uo pipefail

REPO="$(realpath "$(dirname "$0")/../..")"

payload="$(cat)"
command="$(printf '%s' "$payload" | jq -r '.tool_input.command // ""')"
cwd="$(printf '%s' "$payload" | jq -r '.cwd // ""')"

# Only refuse while the claim below is actually true. A previous call may have left the shell in a
# subdirectory, and then cd-ing back to the root is the fix, not the mistake.
if [ -n "$cwd" ] && [ "$(realpath "$cwd" 2>/dev/null)" != "$REPO" ]; then
    exit 0
fi

deny() {
    jq -nc --arg reason "$1" \
        '{hookSpecificOutput: {hookEventName: "PreToolUse",
                               permissionDecision: "deny",
                               permissionDecisionReason: $reason}}'
    exit 0
}

# A cd to the repository root, at the start of the line or after a ; && || separator.
if printf '%s' "$command" | grep -qE "(^|[;&|][[:space:]]*)cd[[:space:]]+\"?'?${REPO}/?\"?'?([[:space:]]*($|[;&|]))"; then
    deny "The repository root is already the working directory and persists between calls, so this \
cd does nothing except make the command line unique enough to need a permission prompt. Drop the \
'cd ${REPO}' and run the command on its own. If you need a different directory, use an absolute \
path instead."
fi

# git -C /path/to/repo — the same no-op with a flag instead of a cd.
if printf '%s' "$command" | grep -qE -- "-C[[:space:]]+[\"']?${REPO}/?[\"']?([[:space:]]|$)"; then
    deny "git -C '${REPO}' points at the directory the command already runs in. Drop the -C."
fi

# The same thing wearing a variable, whichever way the variable is then used: R=<repo>; cd \$R,
# or R=<repo>; git -C \$R. Assigning the root at all is the tell, so that is what is matched —
# the first two rules kept being sidestepped by a new way of spending the variable.
if printf '%s' "$command" | grep -qE "(^|[[:space:];&|])[A-Za-z_][A-Za-z0-9_]*=[\"']?${REPO}/?[\"']?([[:space:]]|;|$)"; then
    deny "Assigning the repository root to a variable is the same no-op as cd-ing there, no matter \
whether it is then spent on 'cd \$VAR' or 'git -C \$VAR'. The command already runs in the \
repository root — drop the variable. For a path outside the repository, write it out in full."
fi

exit 0
