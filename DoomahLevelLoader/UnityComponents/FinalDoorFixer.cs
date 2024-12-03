//using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DoomahLevelLoader.UnityComponents
{
	// Token: 0x0200002D RID: 45
	public class FinalDoorFixer : MonoBehaviour
	{
		// Token: 0x060000FC RID: 252 RVA: 0x0000A36C File Offset: 0x0000856C
		private void OnEnable()
		{
			this.Activate();
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000A378 File Offset: 0x00008578
		public void Activate()
		{
			bool flag = this.oneTime && this._activated;
			if (!flag)
			{
				this._activated = true;
				GameObject gameObject = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Levels/Special Rooms/FinalRoom.prefab").WaitForCompletion();
				bool flag2 = gameObject == null;
				if (flag2)
				{
					Debug.LogWarning("Tried to load asset, but it does not exist");
					base.enabled = false;
				}
				else
				{
					this.instantiatedObject = Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation, base.transform);
					bool flag3 = this.moveToParent;
					if (flag3)
					{
						this.instantiatedObject.transform.SetParent(base.transform.parent, true);
					}
					Debug.Log("FinalDoorFixer: Final door game object loaded successfully.");
					Transform transform = this.instantiatedObject.transform.Find("FinalDoor");
					FinalDoor finalDoor = ((transform != null) ? transform.GetComponent<FinalDoor>() : null);
					bool flag4 = finalDoor != null;
					if (flag4)
					{
						this.FD = finalDoor;
						Debug.Log("FinalDoorFixer: Final door component found successfully.");
					}
					else
					{
						Debug.LogWarning("FinalDoorFixer: Final door component not found.");
					}
					this.PostInstantiate(this.instantiatedObject);
				}
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000A49B File Offset: 0x0000869B
		protected virtual void PostInstantiate(GameObject instantiatedObject)
		{
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000A4A0 File Offset: 0x000086A0
		private void OnTriggerEnter(Collider other)
		{
			bool flag = !this.isOpened && other.CompareTag("Player") && this.FD != null;
			if (flag)
			{
				this.FD.Open();
				this.isOpened = true;
			}
		}

		// Token: 0x0400013B RID: 315
		public bool oneTime = true;

		// Token: 0x0400013C RID: 316
		public bool moveToParent = true;

		// Token: 0x0400013D RID: 317
		public BoxCollider OpenTrigger;

		// Token: 0x0400013E RID: 318
		private FinalDoor FD;

		// Token: 0x0400013F RID: 319
		private GameObject instantiatedObject;

		// Token: 0x04000140 RID: 320
		private bool isOpened = false;

		// Token: 0x04000141 RID: 321
		private bool _activated = false;
	}
}
