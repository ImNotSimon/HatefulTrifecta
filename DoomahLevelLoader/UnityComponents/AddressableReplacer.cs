//using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DoomahLevelLoader.UnityComponents
{
	// Token: 0x02000029 RID: 41
	public class AddressableReplacer : MonoBehaviour
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x00009EAB File Offset: 0x000080AB
		private void OnEnable()
		{
			this.Activate();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00009EB8 File Offset: 0x000080B8
		public void Activate()
		{
			bool flag = this.oneTime && this._activated;
			if (!flag)
			{
				this._activated = true;
				GameObject gameObject = Addressables.LoadAssetAsync<GameObject>(this.targetAddress).WaitForCompletion();
				bool flag2 = gameObject == null;
				if (flag2)
				{
					Debug.LogWarning("Tried to load asset at address " + this.targetAddress + ", but it does not exist");
					base.enabled = false;
				}
				else
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation, base.transform);
					this.eid = gameObject2.GetComponent<EnemyIdentifier>();
					bool flag3 = this.eid == null && gameObject2.transform.childCount > 0;
					if (flag3)
					{
						this.eid = gameObject2.transform.GetChild(0).GetComponent<EnemyIdentifier>();
					}
					bool flag4 = this.moveToParent;
					if (flag4)
					{
						gameObject2.transform.SetParent(base.transform.parent, true);
					}
					this.PostInstantiate(gameObject2);
					bool flag5 = this.eid != null && this.IsBoss;
					if (flag5)
					{
						BossHealthBar bossHealthBar = this.eid.gameObject.AddComponent<BossHealthBar>();
						bool flag6 = !string.IsNullOrEmpty(this.BossName);
						if (flag6)
						{
							bossHealthBar.bossName = this.BossName;
						}
					}
					bool flag7 = this.eid != null && this.IsSanded;
					if (flag7)
					{
						this.eid.Sandify(false);
					}
					bool flag8 = this.eid != null && this.IsPuppet;
					if (flag8)
					{
						this.eid.PuppetSpawn();
						this.eid.puppet = true;
					}
					bool flag9 = this.eid != null && this.IsRadient;
					if (flag9)
					{
						this.eid.radianceTier = this.RadienceTier;
						this.eid.healthBuffModifier = this.HealthTier;
						this.eid.speedBuffModifier = this.SpeedTier;
						this.eid.damageBuffModifier = this.DamageTier;
						this.eid.BuffAll();
					}
					bool flag10 = this.destroyThis;
					if (flag10)
					{
						Object.Destroy(base.gameObject);
						base.gameObject.SetActive(false);
					}
					base.enabled = false;
				}
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000A11D File Offset: 0x0000831D
		protected virtual void PostInstantiate(GameObject instantiatedObject)
		{
		}

		// Token: 0x0400012A RID: 298
		public string targetAddress;

		// Token: 0x0400012B RID: 299
		public bool oneTime = true;

		// Token: 0x0400012C RID: 300
		public bool moveToParent = true;

		// Token: 0x0400012D RID: 301
		public bool destroyThis = true;

		// Token: 0x0400012E RID: 302
		public bool IsBoss = false;

		// Token: 0x0400012F RID: 303
		public string BossName;

		// Token: 0x04000130 RID: 304
		public bool IsSanded = false;

		// Token: 0x04000131 RID: 305
		public bool IsPuppet = false;

		// Token: 0x04000132 RID: 306
		public bool IsRadient = false;

		// Token: 0x04000133 RID: 307
		public float RadienceTier;

		// Token: 0x04000134 RID: 308
		public float DamageTier;

		// Token: 0x04000135 RID: 309
		public float SpeedTier;

		// Token: 0x04000136 RID: 310
		public float HealthTier;

		// Token: 0x04000137 RID: 311
		internal EnemyIdentifier eid;

		// Token: 0x04000138 RID: 312
		private bool _activated = false;
	}
}
