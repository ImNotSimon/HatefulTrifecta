//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200001C RID: 28
public class Wretch : MonoBehaviour
{
    public bool isEnraged { get; private set; }

    private Coroutine gasolineCoroutine;

    private int blessings;

    private bool blessed;

    [HideInInspector]
    public List<GameObject> blessingGlows = new List<GameObject>();

    public GameObject blessingGlow;

    private List<Flammable> burners;

    private void Start()
    {
        this.nma = base.GetComponent<NavMeshAgent>();
        this.anim = base.GetComponent<Animator>();
        this.eid = base.GetComponent<EnemyIdentifier>();
        this.mach = base.GetComponent<Machine>();
        this.maxHealth = this.mach.health;
        this.rb = base.GetComponent<Rigidbody>();
        this.anim.SetFloat("Speed", 1f);
        bool flag = this.eid.difficultyOverride >= 0;
        if (flag)
        {
            this.difficulty = this.eid.difficultyOverride;
        }
        else
        {
            this.difficulty = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
        }
        this.SlowUpdate();
        if (this.difficulty > 3)
        {
            gasolineCoroutine = StartCoroutine(InstantiateGasolineRoutine());
        }
    }

    private IEnumerator InstantiateGasolineRoutine()
    {
        while (true)
        {
            if (gasoline != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out hitInfo, 120f, LayerMaskDefaults.Get(LMD.Environment)))
                {
                    Instantiate(gasoline, hitInfo.point, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SlowUpdate()
    {
        Invoke("SlowUpdate", 0.25f);
        if (eid.target != null && !inAction && nma.enabled && nma.isOnNavMesh && Physics.Raycast(eid.target.position, Vector3.down, out var hitInfo, 120f, LayerMaskDefaults.Get(LMD.Environment)))
        {
            nma.SetDestination(hitInfo.point);
        }
    }
    private void Update()
    {
        bool flag = this.difficulty > 3 && this.currentEnrageEffect == null && this.mach.health < this.maxHealth / 2f;
        if (flag)
        {
            this.Enrage();
        }
        else
        {
            bool flag2 = this.mach.health < this.maxHealth / 2f;
            if (flag2)
            {
                this.HalfHealthThing();
            }
        }
        Vector3 vector = ((this.eid.target == null) ? (base.transform.position + base.transform.forward) : new Vector3(this.eid.target.position.x, base.transform.position.y, this.eid.target.position.z));
        bool flag3 = !this.inAction && this.eid.target != null;
        if (flag3)
        {
            bool flag4 = Vector3.Distance(base.transform.position, vector) < 5f;
            if (flag4)
            {
                this.Swing();
            }
        }
        else
        {
            bool flag5 = this.moving;
            if (flag5)
            {
                this.rb.MovePosition(base.transform.position + base.transform.forward * Time.deltaTime * 15f);
            }
        }
        this.anim.SetBool("Walking", !this.inAction && this.nma.velocity.magnitude > 1.5f);
        if (mach.health <= 0)
        {
            StopCoroutine(gasolineCoroutine);
            gasolineCoroutine = null;
        }
    }

    // Token: 0x060000A6 RID: 166 RVA: 0x0000717C File Offset: 0x0000537C
    public void HalfHealthThing()
    {
        this.weakpoint.SetActive(true);
        this.mach.health = 99999f;
        this.eid.health = 99999f;
        this.HateBless(true);
        if (difficulty == 3) {
            this.anim.SetFloat("Speed", 1.55f);
            this.nma.speed = 12.5f;
        }
    }

    // Token: 0x060000A7 RID: 167 RVA: 0x000071AC File Offset: 0x000053AC
    public void Enrage()
    {
        bool flag = !this.eid.dead && !this.isEnraged;
        if (flag)
        {
            this.isEnraged = true;
            bool flag2 = this.ensims == null || this.ensims.Length == 0;
            if (flag2)
            {
                this.ensims = base.GetComponentsInChildren<EnemySimplifier>();
            }
            EnemySimplifier[] array = this.ensims;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].enraged = true;
            }
            this.anim.SetFloat("Speed", 2.2f);
            this.HalfHealthThing();
            this.mach.health = 99999f;
            this.eid.health = 99999f;
            this.nma.speed = 16f;
            this.currentEnrageEffect = Object.Instantiate<GameObject>(this.enrageEffect, this.EnrageSpot);
        }
    }

    // Token: 0x060000A8 RID: 168 RVA: 0x00007296 File Offset: 0x00005496
    public void UnEnrage()
    {
        Object.Destroy(this.currentEnrageEffect);
    }

    // Token: 0x060000A9 RID: 169 RVA: 0x000072A8 File Offset: 0x000054A8
    private void Swing()
    {
        this.inAction = true;
        this.nma.enabled = false;
        this.anim.Play("Swing", -1, 0f);
        bool flag = this.eid.target != null;
        if (flag)
        {
            base.transform.LookAt(new Vector3(this.eid.target.position.x, base.transform.position.y, this.eid.target.position.z));
        }
    }

    public void HateBless(bool ignorePrevious = false)
    {
        if (!ignorePrevious)
        {
            blessings++;
            if (blessings > 1)
            {
                return;
            }
        }
        if (!ignorePrevious && blessed)
        {
            return;
        }
        blessed = true;
        EnemyIdentifierIdentifier[] componentsInChildren = GetComponentsInChildren<EnemyIdentifierIdentifier>();
        foreach (EnemyIdentifierIdentifier enemyIdentifierIdentifier in componentsInChildren)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(blessingGlow, enemyIdentifierIdentifier.transform.position, enemyIdentifierIdentifier.transform.rotation);
            Collider component = enemyIdentifierIdentifier.GetComponent<Collider>();
            if ((bool)component)
            {
                gameObject.transform.localScale = component.bounds.size;
            }
            gameObject.transform.SetParent(enemyIdentifierIdentifier.transform, worldPositionStays: true);
            blessingGlows.Add(gameObject);
        }
        if (burners == null || burners.Count <= 0)
        {
            return;
        }
        foreach (Flammable burner in burners)
        {
            burner.PutOut(getWet: false);
        }
        burners.Clear();
    }

    // Token: 0x060000AA RID: 170 RVA: 0x00007340 File Offset: 0x00005540
    private void DamageStart()
    {
        bool flag = !this.isEnraged;
        if (flag)
        {
            Object.Instantiate<GameObject>(this.flash, this.parryTransform);
            this.mach.parryable = true;
        }
        else
        {
            bool isEnraged = this.isEnraged;
            if (isEnraged)
            {
                Object.Instantiate<GameObject>(this.blueflash, this.parryTransform);
                this.mach.parryable = false;
            }
        }
        this.sc.DamageStart();
        this.moving = true;
    }

    // Token: 0x060000AB RID: 171 RVA: 0x000073BA File Offset: 0x000055BA
    private void DamageStop()
    {
        this.mach.parryable = false;
        this.sc.DamageStop();
        this.moving = false;
    }

    // Token: 0x060000AC RID: 172 RVA: 0x000073DC File Offset: 0x000055DC
    private void StopAction()
    {
        this.inAction = false;
        this.mach.parryable = false;
        bool onGround = this.mach.gc.onGround;
        if (onGround)
        {
            this.nma.enabled = true;
        }
    }

    // Token: 0x040000C7 RID: 199
    private NavMeshAgent nma;

    // Token: 0x040000C8 RID: 200
    [SerializeField]
    private SwingCheck2 sc;

    // Token: 0x040000C9 RID: 201
    [SerializeField]
    private GameObject weakpoint;

    // Token: 0x040000CA RID: 202
    private Animator anim;

    // Token: 0x040000CB RID: 203
    public Transform EnrageSpot;

    // Token: 0x040000CC RID: 204
    public GameObject flash;

    // Token: 0x040000CD RID: 205
    public GameObject blueflash;

    // Token: 0x040000CE RID: 206
    public Transform parryTransform;

    // Token: 0x040000CF RID: 207
    private EnemyIdentifier eid;

    // Token: 0x040000D0 RID: 208
    private Machine mach;

    // Token: 0x040000D1 RID: 209
    private Rigidbody rb;

    // Token: 0x040000D2 RID: 210
    private bool inAction;

    // Token: 0x040000D3 RID: 211
    private bool enraged;

    // Token: 0x040000D4 RID: 212
    private int difficulty;

    // Token: 0x040000D5 RID: 213
    private EnemySimplifier[] ensims;

    // Token: 0x040000D7 RID: 215
    public GameObject enrageEffect;

    public GameObject gasoline;

    // Token: 0x040000D8 RID: 216
    private GameObject currentEnrageEffect;

    // Token: 0x040000D9 RID: 217
    private bool moving;

    // Token: 0x040000DA RID: 218
    private float maxHealth;
}
