# TumbleAttackStun — Local Agent Context

This is an independent BepInEx 5.x mod repository for R.E.P.O. It stuns enemies
after a tumble attack. `TumbleAttackStunPlugin.cs` is the entry point;
`HurtCollider_EnemyHurt_Patch.cs` contains the gameplay patch; and
`ReloadConfigOnLevelStart.cs` refreshes configuration. The project targets
`net48` and builds `TumbleAttackStun.dll`.

## Working rules

- This repository must remain usable after a normal clone. Do not require its
  parent workspace for normal builds or documentation.
- Build with `dotnet build TumbleAttackStun.csproj`. The current post-build
  target deploys to the game plugin folder.
- Preserve existing local modifications. Do not reset, discard, or overwrite
  unrelated work.
- Before making a shared-code decision, determine whether it belongs only here
  or should be promoted to RepoAPI. This mod has not yet been migrated to the
  workspace's RepoAPI/submodule convention.
- Maintain documentation with implementation. Update this file, README,
  CHANGELOG, and create or update `ARCHITECTURE.md` when runtime or design
  facts change.

Before the first edit of every task, synchronize
`external/RepoKit` once and follow its synchronization gate. Read
its `VERSION.md`, `REPO_MODS_WORKSPACE.md`, and
`REPO_MODS_METHODOLOGY.md`; do not repeat that check during the same task
unless shared guidance changes.
