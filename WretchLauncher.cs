//using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class WretchLauncher : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x00007464 File Offset: 0x00005664
	private void Start()
	{
		this.eid = base.GetComponentInParent<EnemyIdentifier>();
		this.anim = base.GetComponent<Animator>();
		this.mach = base.GetComponent<Machine>();
		this.cooldown = this.firstFireDelay;
		bool flag = this.eid.difficultyOverride >= 0;
		if (flag)
		{
			this.difficulty = this.eid.difficultyOverride;
		}
		else
		{
			this.difficulty = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
		}
		bool flag2 = this.difficulty == 1;
		if (flag2)
		{
			this.difficultySpeedModifier = 0.8f;
		}
		else
		{
			bool flag3 = this.difficulty == 0;
			if (flag3)
			{
				this.difficultySpeedModifier = 0.6f;
			}
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x0000751A File Offset: 0x0000571A
	public void InstantiateDeathFX()
	{
		Object.Instantiate<GameObject>(this.DeathFX, base.transform);
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00007530 File Offset: 0x00005730
	private void Update()
	{
		this.cooldown = Mathf.MoveTowards(this.cooldown, 0f, Time.deltaTime * this.eid.totalSpeedModifier * this.difficultySpeedModifier);
		bool flag = this.cooldown == 0f && this.eid.target != null;
		if (flag)
		{
			this.cooldown = this.firingDelay;
			this.ShootHoming();
			UltrakillEvent ultrakillEvent = this.onFire;
			if (ultrakillEvent != null)
			{
				ultrakillEvent.Invoke();
			}
		}
		bool flag2 = this.mach.health <= 0f || this.eid.health <= 0f;
		if (flag2)
		{
			this.parentMachine.health = 0f;
			this.parentEID.health = 0f;
			this.parent.UnEnrage();
			Object.Instantiate<GameObject>(this.boom, base.transform);
		}
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00007624 File Offset: 0x00005824
	public void ShootHoming()
	{
		bool flag = this.eid.target != null;
		if (flag)
		{
			Projectile projectile = Object.Instantiate<Projectile>(this.mortar, this.shootPoint.position, this.shootPoint.rotation);
			projectile.target = this.eid.target;
			projectile.GetComponent<Rigidbody>().velocity = this.shootPoint.forward * this.projectileForce;
			projectile.damage *= this.eid.totalDamageModifier;
			projectile.safeEnemyType = this.eid.enemyType;
			projectile.turningSpeedMultiplier *= this.difficultySpeedModifier;
			projectile.gameObject.SetActive(true);
			bool flag2 = this.anim;
			if (flag2)
			{
				this.anim.Play("Shoot", 0, 0f);
			}
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00007710 File Offset: 0x00005910
	public void ChangeFiringDelay(float target)
	{
		this.firingDelay = target;
		bool flag = this.cooldown > this.firingDelay;
		if (flag)
		{
			this.cooldown = this.firingDelay;
		}
	}

	// Token: 0x040000DE RID: 222
	private EnemyIdentifier eid;

	// Token: 0x040000DF RID: 223
	public Transform shootPoint;

	// Token: 0x040000E0 RID: 224
	public Projectile mortar;

	// Token: 0x040000E1 RID: 225
	public Wretch parent;

	// Token: 0x040000E2 RID: 226
	public Machine parentMachine;

	// Token: 0x040000E3 RID: 227
	public EnemyIdentifier parentEID;

	// Token: 0x040000E4 RID: 228
	public GameObject boom;

	// Token: 0x040000E5 RID: 229
	public GameObject DeathFX;

	// Token: 0x040000E6 RID: 230
	private Machine mach;

	// Token: 0x040000E7 RID: 231
	private float cooldown = 1f;

	// Token: 0x040000E8 RID: 232
	public float firingDelay;

	// Token: 0x040000E9 RID: 233
	public float firstFireDelay = 1f;

	// Token: 0x040000EA RID: 234
	public float projectileForce;

	// Token: 0x040000EB RID: 235
	public UltrakillEvent onFire;

	// Token: 0x040000EC RID: 236
	private Animator anim;

	// Token: 0x040000ED RID: 237
	private int difficulty;

	// Token: 0x040000EE RID: 238
	private float difficultySpeedModifier = 1f;
}
