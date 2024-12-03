using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace HatefulLevelScript
{
	public static class Utils
	{
		public static Font gameFont
		{
			get
			{
				bool flag = Utils._gameFont == null;
				if (flag)
				{
					Utils._gameFont = Utils.LoadObject<Font>("Assets/Fonts/VCR_OSD_MONO_1.001.ttf");
				}
				return Utils._gameFont;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00009AF0 File Offset: 0x00007CF0
		public static Sprite levelPanel
		{
			get
			{
				bool flag = Utils._levelPanel == null;
				if (flag)
				{
					Utils._levelPanel = Utils.LoadObject<Sprite>("Assets/Textures/UI/meter.png");
				}
				return Utils._levelPanel;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00009B28 File Offset: 0x00007D28
		public static Sprite hellmapArrow
		{
			get
			{
				bool flag = Utils._hellmapArrow == null;
				if (flag)
				{
					Utils._hellmapArrow = Utils.LoadObject<Sprite>("Assets/Textures/UI/arrow.png");
				}
				return Utils._hellmapArrow;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00009B60 File Offset: 0x00007D60
		public static Material metalDec20
		{
			get
			{
				bool flag = Utils._metalDec20 == null;
				if (flag)
				{
					Utils._metalDec20 = Utils.LoadObject<Material>("Assets/Materials/Environment/Metal/Metal Decoration 20.mat");
				}
				return Utils._metalDec20;
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00009B98 File Offset: 0x00007D98
		public static T LoadObject<T>(string path)
		{
			bool flag = Utils.resourceMap == null;
			if (flag)
			{
				Addressables.InitializeAsync().WaitForCompletion();
				Utils.resourceMap = Addressables.ResourceLocators.First<IResourceLocator>() as ResourceLocationMap;
			}
			Debug.Log("Loading " + path);
			KeyValuePair<object, IList<IResourceLocation>> keyValuePair;
			try
			{
				keyValuePair = Utils.resourceMap.Locations.Where((KeyValuePair<object, IList<IResourceLocation>> pair) => pair.Key as string == path).First<KeyValuePair<object, IList<IResourceLocation>>>();
			}
			catch (Exception)
			{
				return default(T);
			}
			return Addressables.LoadAssetAsync<T>(keyValuePair.Value.First<IResourceLocation>()).WaitForCompletion();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00009C60 File Offset: 0x00007E60
		public static void SetPlayerWorldRotation(Quaternion newRotation)
		{
			MonoSingleton<CameraController>.Instance.transform.rotation = newRotation;
			float x = MonoSingleton<CameraController>.Instance.transform.localEulerAngles.x;
			float num = x;
			bool flag = x <= 90f && x >= 0f;
			if (flag)
			{
				num = -x;
			}
			else
			{
				bool flag2 = x >= 270f && x <= 360f;
				if (flag2)
				{
					num = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(360f, 270f, x));
				}
			}
			float y = MonoSingleton<CameraController>.Instance.transform.rotation.eulerAngles.y;
			MonoSingleton<CameraController>.Instance.rotationX = num;
			MonoSingleton<CameraController>.Instance.rotationY = y;
		}

		// Token: 0x04000121 RID: 289
		private static Font _gameFont;

		// Token: 0x04000122 RID: 290
		private static Sprite _levelPanel;

		// Token: 0x04000123 RID: 291
		private static Sprite _hellmapArrow;

		// Token: 0x04000124 RID: 292
		private static Material _metalDec20;

		// Token: 0x04000125 RID: 293
		public static ResourceLocationMap resourceMap;
	}
}
