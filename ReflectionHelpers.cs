using System;
using System.Reflection;
using TumbleAttackStun;
using UnityEngine;

namespace TumbleAttackStun
{
	internal static class ReflectionHelpers
	{
		// PlayerTumble.isTumbling
		private static readonly Type PlayerTumbleType = typeof(PlayerTumble);
		private static readonly PropertyInfo PT_IsTumbling_Property;
		private static readonly FieldInfo PT_IsTumbling_Field;

		// Enemy.HasStateStunned and Enemy.StateStunned
		private static readonly Type EnemyType = typeof(Enemy);
		private static readonly PropertyInfo Enemy_HasStateStunned_Property;
		private static readonly FieldInfo Enemy_HasStateStunned_Field;
		private static readonly PropertyInfo Enemy_StateStunned_Property;
		private static readonly FieldInfo Enemy_StateStunned_Field;

		// EnemyStateStunned.Set(float)
		private static readonly MethodInfo StateStunned_Set_Method;

		static ReflectionHelpers()
		{
			BindingFlags allInst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			// PlayerTumble.isTumbling (try property then field)
			PT_IsTumbling_Property = PlayerTumbleType.GetProperty("isTumbling", allInst);
			PT_IsTumbling_Field = PT_IsTumbling_Property == null ? PlayerTumbleType.GetField("isTumbling", allInst) : null;

			// Enemy.HasStateStunned
			Enemy_HasStateStunned_Property = EnemyType.GetProperty("HasStateStunned", allInst);
			Enemy_HasStateStunned_Field = Enemy_HasStateStunned_Property == null ? EnemyType.GetField("HasStateStunned", allInst) : null;

			// Enemy.StateStunned
			Enemy_StateStunned_Property = EnemyType.GetProperty("StateStunned", allInst);
			Enemy_StateStunned_Field = Enemy_StateStunned_Property == null ? EnemyType.GetField("StateStunned", allInst) : null;

			// If we can find a StateStunned instance type, look for Set(float)
			Type stateStunnedType = null;
			if (Enemy_StateStunned_Property != null)
			{
				stateStunnedType = Enemy_StateStunned_Property.PropertyType;
			}
			else if (Enemy_StateStunned_Field != null)
			{
				stateStunnedType = Enemy_StateStunned_Field.FieldType;
			}
			// fallback: try to locate by name
			if (stateStunnedType == null)
			{
				stateStunnedType = Type.GetType("EnemyStateStunned") ?? Type.GetType("EnemyStateStunned, Assembly-CSharp");
			}
			if (stateStunnedType != null)
			{
				StateStunned_Set_Method = stateStunnedType.GetMethod("Set", allInst, null, new Type[] { typeof(float) }, null);
			}
		}

		public static bool TryGetIsTumbling(PlayerTumble pt)
		{
			if (pt == null) return false;
			try
			{
				if (PT_IsTumbling_Property != null)
				{
					object val = PT_IsTumbling_Property.GetValue(pt);
					if (val is bool b) return b;
				}
				else if (PT_IsTumbling_Field != null)
				{
					object val = PT_IsTumbling_Field.GetValue(pt);
					if (val is bool b) return b;
				}
			}
			catch (Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogWarning($"ReflectionHelpers.TryGetIsTumbling error: {ex.Message}");
			}
			return false;
		}

		public static bool TryGetHasStateStunned(Enemy enemy, out bool value)
		{
			value = false;
			if (enemy == null) return false;
			try
			{
				if (Enemy_HasStateStunned_Property != null)
				{
					object val = Enemy_HasStateStunned_Property.GetValue(enemy);
					if (val is bool b) { value = b; return true; }
				}
				else if (Enemy_HasStateStunned_Field != null)
				{
					object val = Enemy_HasStateStunned_Field.GetValue(enemy);
					if (val is bool b) { value = b; return true; }
				}
				else
				{
					// Fallback: if there is a StateStunned instance, treat that as true
					object state = GetStateStunned(enemy);
					value = state != null;
					return true;
				}
			}
			catch (Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogWarning($"ReflectionHelpers.TryGetHasStateStunned error: {ex.Message}");
			}
			return false;
		}

		public static object GetStateStunned(Enemy enemy)
		{
			if (enemy == null) return null;
			try
			{
				if (Enemy_StateStunned_Property != null)
				{
					return Enemy_StateStunned_Property.GetValue(enemy);
				}
				if (Enemy_StateStunned_Field != null)
				{
					return Enemy_StateStunned_Field.GetValue(enemy);
				}
				// Last resort: try GetComponent<EnemyStateStunned>()
				var comp = enemy.GetComponent("EnemyStateStunned");
				return comp;
			}
			catch (Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogWarning($"ReflectionHelpers.GetStateStunned error: {ex.Message}");
				return null;
			}
		}

		public static bool TrySetStateStunned(object stateStunnedInstance, float time)
		{
			if (stateStunnedInstance == null) return false;
			try
			{
				if (StateStunned_Set_Method != null)
				{
					StateStunned_Set_Method.Invoke(stateStunnedInstance, new object[] { time });
					return true;
				}
				// fallback: try dynamic invocation by method name
				var mi = stateStunnedInstance.GetType().GetMethod("Set", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(float) }, null);
				if (mi != null)
				{
					mi.Invoke(stateStunnedInstance, new object[] { time });
					return true;
				}
			}
			catch (Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogWarning($"ReflectionHelpers.TrySetStateStunned error: {ex.Message}");
			}
			return false;
		}

		public static bool TrySetEnemyStun(Enemy enemy, float time)
		{
			if (enemy == null) return false;
			try
			{
				var state = GetStateStunned(enemy);
				if (state == null) return false;
				return TrySetStateStunned(state, time);
			}
			catch (Exception ex)
			{
				TumbleAttackStunPlugin.Logger?.LogWarning($"ReflectionHelpers.TrySetEnemyStun error: {ex.Message}");
				return false;
			}
		}
	}
}