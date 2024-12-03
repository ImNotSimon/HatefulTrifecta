using System;
using System.Collections;
using UnityEngine;

namespace HatefulLevelScript
{
	// Token: 0x02000026 RID: 38
	public static class UnityUtils
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00009D29 File Offset: 0x00007F29
		public static IEnumerable GetComponentsInChildrenRecursive<T>(Transform parent) where T : Component
		{
			foreach (object obj in parent)
			{
				Transform child = (Transform)obj;
				T comp;
				bool flag = child.TryGetComponent<T>(out comp);
				if (flag)
				{
					yield return comp;
				}
				foreach (object obj2 in UnityUtils.GetComponentsInChildrenRecursive<T>(child))
				{
					T childComp = (T)((object)obj2);
					yield return childComp;
					childComp = default(T);
				}
				IEnumerator enumerator2 = null;
				comp = default(T);
				child = null;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00009D3C File Offset: 0x00007F3C
		public static T GetComponentInChildrenRecursive<T>(Transform parent) where T : Component
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				T t;
				bool flag = transform.TryGetComponent<T>(out t);
				if (flag)
				{
					return t;
				}
				T componentInChildrenRecursive = UnityUtils.GetComponentInChildrenRecursive<T>(transform);
				bool flag2 = componentInChildrenRecursive != null;
				if (flag2)
				{
					return componentInChildrenRecursive;
				}
			}
			return default(T);
		}
	}
}
