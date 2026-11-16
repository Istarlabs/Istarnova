#!/usr/bin/env bash
# SPDX-FileCopyrightText: 2026 The Istarnova Contributors
# SPDX-License-Identifier: GPL-3.0-or-later

#
# PreToolUse/Bash hook: refuse hand-built command lines that a script in Scripts/ already owns.
#
# CLAUDE.md documents the scripts, and the documentation alone does not hold: a reflex like
# `gh issue list` or `rg -l pattern` gets written before the rule is consulted, and every unique
# command line asks for permission again — which is exactly what the scripts exist to avoid.
# This hook moves the rule from "remembered" to "enforced".
#
# Blocked:   gh issue list                  ->  python3 Scripts/repo.py issues
#            git commit -m "..."            ->  python3 Scripts/repo.py commit
#            git log --oneline              ->  python3 Scripts/repo.py log
#            rg -l "SomeSymbol"             ->  python3 Scripts/find.py SomeSymbol
#            dotnet build                   ->  python3 Scripts/dotnet.py verify
#            grep/find/sed/awk over files   ->  find.py, or the Grep, Glob and Read tools
#            python3 -c / python3 <<'PY'    ->  find.py, Read, or a script written to a file
# Allowed:   git status / git diff, gh api, and every one of those tools behind a pipe,
#            where it filters another command's output instead of reading the tree.
#
# Working through the scripts and the harness tools is the rule, and it has no exception and no
# opt-out: an escape hatch is easier to type than the right subcommand, so it becomes the first reflex
# instead of the last resort. A script that cannot do the thing is a reason to extend the script.
#
# `\grep` counts like `grep` (the profile aliases ls and cat, so the backslash is a habit here), and
# inline python counts like the grep it replaces - the interpreter is not the problem, code on the
# command line is.
#
# The same hook as in PersonalMediRecords, minus the rules for what only that repository has
# (db.py, the EF commands).
#
# Reads the hook payload on stdin, writes a PreToolUse deny decision on stdout when it objects.

set -uo pipefail

payload="$(cat)"
command="$(printf '%s' "$payload" | jq -r '.tool_input.command // ""')"

deny() {
    jq -nc --arg reason "$1" \
        '{hookSpecificOutput: {hookEventName: "PreToolUse",
                               permissionDecision: "deny",
                               permissionDecisionReason: $reason}}'
    exit 0
}

# What may sit between a separator and the command name without hiding it: VAR=value prefixes, an
# `xargs` (with its own flags), and the backslash that escapes a shell alias.
prefix='([A-Za-z_][A-Za-z0-9_]*=[^[:space:]]*[[:space:]]+)*(xargs[[:space:]]+(-[^[:space:]]+[[:space:]]+)*)?\\?'

# Anywhere a command can start: line start, or after ; && || ( - the pipe included.
start="(^|[;&|(][[:space:]]*)${prefix}"

# The same, minus the pipe. Behind a pipe grep/sed/awk/head filter another command's output, which
# is what they are for and what no script replaces; at the start of a line they read the tree, and
# that is the Grep, Glob and Read tools' job.
reads_files="(^|[;&(][[:space:]]*)${prefix}"

matches() {
    printf '%s' "$command" | grep -qE "$1"
}

if matches "${start}gh[[:space:]]+(issue|pr|label)\b"; then
    deny "Scripts/repo.py owns the GitHub workflow: issues, issue, issue-create, comment, close, \
label, pr, pr-create, pr-edit, pr-merge. It sets GH_PAGER, reads --json instead of the localized \
human output, and carries the guards (no merge unless GitHub reports MERGEABLE). Run \
'python3 Scripts/repo.py --help' and use the subcommand. 'gh api' and 'gh repo' are not covered \
and stay available."
fi

if matches "${start}git[[:space:]]+(commit|push)\b" \
    || matches "${start}git[[:space:]]+(checkout[[:space:]]+-b|switch[[:space:]]+-c)\b"; then
    deny "Scripts/repo.py owns branching, committing and pushing: 'branch' refuses a dirty tree \
and branches off an up-to-date master, 'commit' appends the Co-Authored-By trailer and refuses \
master without --allow-master, 'push' sets the upstream. Read-only git (status, diff, show) \
is fine as it is."
fi

# `-C` included: pointing git at the checkout next door is the same habit, one flag further along.
if matches "${start}git[[:space:]]+(-C[[:space:]]+[^[:space:]]+[[:space:]]+|--no-pager[[:space:]]+)*log\b"; then
    deny "Scripts/repo.py owns the history: 'python3 Scripts/repo.py log [PATH ...] [-n 20] \
[--range master..HEAD]' prints one line per commit with no pager to remember. For who wrote a \
passage and why, 'repo.py blame FILE --lines 24,48'."
fi

if matches "${start}git[[:space:]]+grep\b"; then
    deny "'git grep' is the same raw sweep as grep, one subcommand further along. Use \
'python3 Scripts/find.py Symbol' for a symbol and 'python3 Scripts/find.py \"some text\" --text' for \
anything else - the latter searches every file type without a word boundary, which is what git grep \
was being used for."
fi

if matches "${start}rg\b" || matches "${reads_files}grep\b"; then
    deny "For 'where is this symbol used, and is it used at all', use \
'python3 Scripts/find.py Symbol [--lines] [--all] [--type project] [--nuget]' - it excludes obj/ and \
bin/, where the source generators emit a second copy of everything, and it exits 1 when the symbol \
appears nowhere. For any other text search use 'find.py \"text\" --text' or the Grep tool. Behind a \
pipe grep stays available as a filter."
fi

if matches "${reads_files}find\b"; then
    deny "Locating files by name or pattern is the Glob tool's job, and 'where is this symbol used' \
is 'python3 Scripts/find.py Symbol'; 'find.py --types --in DIR' lists what each file declares. A raw \
find re-implements both and walks obj/ and bin/, where the source generators emit a second copy of \
every file. Behind a pipe find stays available."
fi

if matches "${reads_files}(sed|awk)\b"; then
    deny "Reading or slicing a file is the Read tool, which takes offset and limit and needs no \
line arithmetic; changing one is the Edit tool, which fails loudly instead of silently matching \
nothing. Behind a pipe sed and awk stay available as filters on another command's output."
fi

# wc is deliberately not here: counting lines or bytes is the one thing Read does not do.
if matches "${reads_files}(cat|head|tail)[[:space:]]+[^|]*[^[:space:]|]"; then
    deny "Reading a file - whole, first lines or last - is the Read tool. It reports the line \
numbers, handles the encoding, and does not need the alias escaping this machine's profile forces \
on cat and ls. Behind a pipe these stay available as filters."
fi

# Only inline code is refused, never the interpreter: `-c` and a heredoc have to sit where python's
# own flags sit, so `python3 Scripts/some_script.py -c config.json` - where the -c belongs to the script - is
# untouched. Blocked behind a pipe too, unlike sed and awk: a filter can be written in the shell or
# put in a file, and "just this once, inline" is how the habit came back.
if matches "${start}python3?[[:space:]]+(-[A-Za-z]+[[:space:]]+)*(-c[[:space:]'\"]|-?[[:space:]]*<<)"; then
    deny "python3 is how every script here runs, but code on the command line is not. Searching is \
'python3 Scripts/find.py Symbol', reading a file is the Read tool (it takes offset and limit), and \
listing or locating files is Glob. When a genuine data question has no tool behind it - parsing an \
export, inspecting an archive - write the script to a file and run that: Scripts/ if it will be \
wanted again, the scratchpad if it truly will not. A file can be read back, corrected and re-run; a \
heredoc is gone once it scrolls past."
fi

if matches "${start}dotnet[[:space:]]+(build|test)\b"; then
    deny "Scripts/dotnet.py owns the build: 'verify' builds, tests and reports warnings on \
the lines you changed, all with English SDK output on a machine whose SDK is German; \
'test --filter X' runs a subset. See 'python3 Scripts/dotnet.py --help'."
fi

exit 0
