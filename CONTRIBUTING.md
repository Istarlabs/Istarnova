# Contributing to Istarnova

Bug reports, suggestions and pull requests are welcome. Discussion in German or English; code,
comments and commit messages in English.

## Before you start

Open an issue first for anything bigger than a small fix, so the approach can be agreed on before the
work is done.

## Making changes

- Follow the conventions in [CLAUDE.md](CLAUDE.md): file header region, XML documentation on every
  public API, one class per file, tests in `Istarnova.Tests` mirroring the library's structure.
- Every source file carries two SPDX lines in its header. `python3 Scripts/spdx.py apply` adds them.
- Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/), e.g.
  `feat(export): ...` or `fix(string): ...`.
- The scripts in `Scripts/` come from [devscripts](https://github.com/Istarlabs/devscripts), pinned as a submodule:
  clone with `git clone --recurse-submodules`, or run `git submodule update --init` in an existing clone.
- Before opening a pull request, run:

```bash
python3 Scripts/dotnet.py verify    # build, tests, and the warnings on the lines you changed
python3 Scripts/spdx.py check       # licence lines present
```

CI runs the same commands.

## Licence

Istarnova is licensed under the [GNU General Public License v3.0 or later](LICENSE.md). By submitting a contribution you agree
that it is licensed under the same terms. Add yourself to [AUTHORS.md](AUTHORS.md) in your first pull
request.

## Code of Conduct

Everyone taking part follows the [Code of Conduct](CODE_OF_CONDUCT.md) - in the repository, its
issues and pull requests, and anywhere else the project goes.
