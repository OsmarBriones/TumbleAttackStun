using HarmonyLib;

namespace TumbleAttackStun
{
	[HarmonyPatch(typeof(EnemyDirector), "Start")]
	internal static class ReloadConfigOnLevelStart
	{
		static void Postfix()
		{
			try
			{
				TumbleAttackStunPlugin.ReloadConfig();
			}
			catch (System.Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogError($"Failed reloading config on level start: {ex}");
			}
		}
	}
}