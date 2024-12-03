using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200000D RID: 13
public class CustomButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x0600002F RID: 47 RVA: 0x00003258 File Offset: 0x00001458
	// (remove) Token: 0x06000030 RID: 48 RVA: 0x00003290 File Offset: 0x00001490
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action onClick;

	// Token: 0x06000031 RID: 49 RVA: 0x000032C5 File Offset: 0x000014C5
	private void Start()
	{
		this.buttonImage = base.GetComponent<Image>();
		this.buttonImage.color = this.normalColor;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x000032E6 File Offset: 0x000014E6
	public void AddOnClickListener(Action listener)
	{
		this.onClick += listener;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000032F1 File Offset: 0x000014F1
	public void RemoveOnClickListener(Action listener)
	{
		this.onClick -= listener;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000032FC File Offset: 0x000014FC
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.buttonImage.color = this.hoverColor;
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00003311 File Offset: 0x00001511
	public void OnPointerExit(PointerEventData eventData)
	{
		this.buttonImage.color = this.normalColor;
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003326 File Offset: 0x00001526
	public void OnPointerDown(PointerEventData eventData)
	{
		this.buttonImage.color = this.clickColor;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x0000333B File Offset: 0x0000153B
	public void OnPointerUp(PointerEventData eventData)
	{
		this.buttonImage.color = this.hoverColor;
		this.InvokeOnClick();
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00003357 File Offset: 0x00001557
	private void InvokeOnClick()
	{
		Action action = this.onClick;
		if (action != null)
		{
			action();
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x0000336C File Offset: 0x0000156C
	private void Update()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		if (mouseButtonDown)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit);
			if (flag)
			{
				bool flag2 = raycastHit.collider == base.GetComponent<Collider>();
				if (flag2)
				{
					this.InvokeOnClick();
				}
			}
		}
	}

	// Token: 0x04000032 RID: 50
	public Color normalColor = Color.white;

	// Token: 0x04000033 RID: 51
	public Color hoverColor = Color.gray;

	// Token: 0x04000034 RID: 52
	public Color clickColor = Color.red;

	// Token: 0x04000035 RID: 53
	private Image buttonImage;
}
