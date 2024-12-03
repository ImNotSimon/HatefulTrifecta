using System;
using HarmonyLib;
using UnityEngine;

namespace HatefulLevelLoader
{
	// Token: 0x0200001F RID: 31
	[HarmonyPatch(typeof(Material))]
	internal static class MaterialPatches
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00007770 File Offset: 0x00005970
		public static void Process(Material material)
		{
			bool flag = material.shader == null;
			if (!flag)
			{
				Shader shader;
				bool flag2 = !ShaderManager.shaderDictionary.TryGetValue(material.shader.name, out shader);
				if (!flag2)
				{
					bool flag3 = material.shader == shader;
					if (!flag3)
					{
						material.shader = shader;
					}
				}
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000077CA File Offset: 0x000059CA
		[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(Shader) })]
		[HarmonyPostfix]
		public static void CtorPatch1(Material __instance)
		{
			MaterialPatches.Process(__instance);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000077D4 File Offset: 0x000059D4
		[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(Material) })]
		[HarmonyPostfix]
		public static void CtorPatch2(Material __instance)
		{
			MaterialPatches.Process(__instance);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000077DE File Offset: 0x000059DE
		[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(string) })]
		[HarmonyPostfix]
		public static void CtorPatch3(Material __instance)
		{
			MaterialPatches.Process(__instance);
		}
	}
}
