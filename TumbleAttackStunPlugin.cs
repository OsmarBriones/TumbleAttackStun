using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace TumbleAttackStun
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	public class TumbleAttackStunPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "osmarbriones.TumbleAttackStun";
		public const string PluginName = "TumbleAttackStun";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

		internal static ConfigEntry<float> StunSecondsConfig { get; private set; }

		internal static TumbleAttackStunPlugin Instance { get; private set; }

		private Harmony harmony;

		private void Awake()
		{
			Instance = this;
			Logger = base.Logger;
			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");

			// Bind config: stun time in seconds (float) with allowed range 0 - 20
			StunSecondsConfig = Config.Bind(
				"General",
				"StunSeconds",
				3f,
				new ConfigDescription(
					"Stun duration (seconds) applied to enemies hit by a tumble attack. Range: 0 - 20",
					new AcceptableValueRange<float>(0f, 20f)
				)
			);

			Logger.LogInfo($"Configured {nameof(StunSecondsConfig)} = {StunSecondsConfig.Value}");

			harmony = new Harmony(PluginGuid);
			harmony.PatchAll();
		}

		private void OnDestroy()
		{
			harmony?.UnpatchSelf();
		}

		/// <summary>
		/// Reloads the plugin config from disk and logs the loaded stun time.
		/// Call from Harmony patches (e.g. on level start).
		/// </summary>
		internal static void ReloadConfig()
		{
			try
			{
				Instance?.Config.Reload();
				Logger?.LogInfo($"TumbleAttackStun config reloaded. StunTime={StunSecondsConfig?.Value}");
			}
			catch (Exception ex)
			{
				Logger?.LogError($"TumbleAttackStun ReloadConfig error: {ex}");
			}
		}
	}
}
