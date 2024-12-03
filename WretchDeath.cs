using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class WretchDeath : MonoBehaviour
{
	// Token: 0x060000AE RID: 174 RVA: 0x00007429 File Offset: 0x00005629
	private void Start()
	{
		this.parentEID.health = 0f;
		this.parentMachine.health = 0f;
		this.parent.UnEnrage();
	}

	// Token: 0x040000DB RID: 219
	public Wretch parent;

	// Token: 0x040000DC RID: 220
	public EnemyIdentifier parentEID;

	// Token: 0x040000DD RID: 221
	public Machine parentMachine;
}
