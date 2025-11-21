using HarmonyLib;
using UnityEngine;

namespace TumbleAttackStun
{
	// Postfix the private/protected HurtCollider.EnemyHurt(Enemy) method.
	[HarmonyPatch]
	internal static class HurtCollider_EnemyHurt_Patch
	{
		static System.Reflection.MethodBase TargetMethod()
		{
			// target the method by name/signature because it's non-public
			return AccessTools.Method(typeof(HurtCollider), "EnemyHurt", new System.Type[] { typeof(Enemy) });
		}

		// Postfix receives the instance, the Enemy param, and the returned bool result
		static void Postfix(HurtCollider __instance, Enemy _enemy, bool __result)
		{
			if (!__result || _enemy == null)
			{
				return;
			}

			try
			{
				// Check if the HurtCollider belongs to a PlayerTumble
				PlayerTumble playerTumble = __instance.GetComponentInParent<PlayerTumble>();
				if (playerTumble == null)
				{
					return;
				}

				// Only apply stun if the player is currently tumbling (use reflection helper)
				if (!ReflectionHelpers.TryGetIsTumbling(playerTumble))
				{
					return;
				}

				// Read stun time from config (safely clamp)
				float stunTime = 0f;
				if (TumbleAttackStunPlugin.StunSecondsConfig != null)
				{
					stunTime = Mathf.Max(0f, TumbleAttackStunPlugin.StunSecondsConfig.Value);
				}
				else
				{
					stunTime = 10f; // fallback
				}

				if (stunTime <= 0f)
				{
					return;
				}

				// Apply stun using reflection helper
				if (ReflectionHelpers.TrySetEnemyStun(_enemy, stunTime))
				{
					TumbleAttackStunPlugin.Logger?.LogInfo($"Applied stun {stunTime}s to enemy {_enemy.name}");
				}
			}
			catch (System.Exception ex)
			{
				// Log safely: plugin logger is initialized in Awake
				if (TumbleAttackStunPlugin.Logger != null)
				{
					TumbleAttackStunPlugin.Logger.LogError($"TumbleAttackStun patch error: {ex}");
				}
			}
		}
	}
}