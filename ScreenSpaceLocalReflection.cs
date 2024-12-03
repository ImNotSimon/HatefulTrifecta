using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class ScreenSpaceLocalReflection : MonoBehaviour
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000011 RID: 17 RVA: 0x00002458 File Offset: 0x00000658
	private int width
	{
		get
		{
			return (int)((float)base.GetComponent<Camera>().pixelWidth * this.resolution);
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000012 RID: 18 RVA: 0x00002480 File Offset: 0x00000680
	private int height
	{
		get
		{
			return (int)((float)base.GetComponent<Camera>().pixelHeight * this.resolution);
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x000024A8 File Offset: 0x000006A8
	private Mesh CreateQuad()
	{
		return new Mesh
		{
			name = "Quad",
			vertices = new Vector3[]
			{
				new Vector3(1f, 1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(-1f, -1f, 0f),
				new Vector3(1f, -1f, 0f)
			},
			triangles = new int[] { 0, 1, 2, 2, 3, 0 }
		};
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002560 File Offset: 0x00000760
	private void ReleaseTexture(ref RenderTexture texture)
	{
		bool flag = texture != null;
		if (flag)
		{
			texture.Release();
			texture = null;
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002588 File Offset: 0x00000788
	private void UpdateTexture(ref RenderTexture texture, RenderTextureFormat format)
	{
		bool flag = texture != null && (texture.width != this.width || texture.height != this.height);
		if (flag)
		{
			this.ReleaseTexture(ref texture);
		}
		bool flag2 = texture == null || !texture.IsCreated();
		if (flag2)
		{
			texture = new RenderTexture(this.width, this.height, 0, format);
			texture.filterMode = FilterMode.Bilinear;
			texture.useMipMap = false;
			texture.enableRandomWrite = true;
			texture.Create();
			Graphics.SetRenderTarget(texture);
			GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
		}
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002654 File Offset: 0x00000854
	private void ReleaseTextures()
	{
		for (int i = 0; i < 2; i++)
		{
			this.ReleaseTexture(ref this.accumulationTextures_[i]);
		}
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002688 File Offset: 0x00000888
	private void UpdateAccumulationTexture()
	{
		for (int i = 0; i < 2; i++)
		{
			this.UpdateTexture(ref this.accumulationTextures_[i], RenderTextureFormat.ARGB32);
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000026BC File Offset: 0x000008BC
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		bool flag = this.shader == null;
		if (!flag)
		{
			bool flag2 = this.material_ == null;
			if (flag2)
			{
				this.material_ = new Material(this.shader);
			}
			bool flag3 = this.screenQuad_ == null;
			if (flag3)
			{
				this.screenQuad_ = this.CreateQuad();
			}
			this.UpdateAccumulationTexture();
			this.material_.SetVector("_Params1", new Vector4(this.raytraceMaxLength, this.raytraceMaxThickness, this.reflectionEnhancer, this.accumulationBlendRatio));
			Camera component = base.GetComponent<Camera>();
			Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(component.projectionMatrix, false);
			Matrix4x4 matrix4x = gpuprojectionMatrix * worldToCameraMatrix;
			this.material_.SetMatrix("_ViewProj", matrix4x);
			this.material_.SetMatrix("_InvViewProj", matrix4x.inverse);
			RenderTexture temporary = RenderTexture.GetTemporary(this.width, this.height, 0, RenderTextureFormat.ARGB32);
			temporary.filterMode = FilterMode.Bilinear;
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.width, this.height, 0, RenderTextureFormat.ARGB32);
			temporary2.filterMode = FilterMode.Bilinear;
			RenderTexture temporary3 = RenderTexture.GetTemporary(this.width, this.height, 0, RenderTextureFormat.ARGB32);
			temporary3.filterMode = FilterMode.Bilinear;
			switch (this.quality)
			{
			case ScreenSpaceLocalReflection.Quality.High:
				this.material_.EnableKeyword("QUALITY_HIGH");
				this.material_.DisableKeyword("QUALITY_MIDDLE");
				this.material_.DisableKeyword("QUALITY_LOW");
				break;
			case ScreenSpaceLocalReflection.Quality.Middle:
				this.material_.DisableKeyword("QUALITY_HIGH");
				this.material_.EnableKeyword("QUALITY_MIDDLE");
				this.material_.DisableKeyword("QUALITY_LOW");
				break;
			case ScreenSpaceLocalReflection.Quality.Low:
				this.material_.DisableKeyword("QUALITY_HIGH");
				this.material_.DisableKeyword("QUALITY_MIDDLE");
				this.material_.EnableKeyword("QUALITY_LOW");
				break;
			}
			Graphics.Blit(src, temporary, this.material_, 0);
			this.material_.SetTexture("_ReflectionTexture", temporary);
			bool flag4 = this.blurNum > 0U;
			if (flag4)
			{
				Graphics.SetRenderTarget(temporary2);
				this.material_.SetVector("_BlurParams", new Vector4(this.blurOffset.x, 0f, this.blurNum, 0f));
				this.material_.SetPass(1);
				Graphics.DrawMeshNow(this.screenQuad_, Matrix4x4.identity);
				this.material_.SetTexture("_ReflectionTexture", temporary2);
				Graphics.SetRenderTarget(temporary3);
				this.material_.SetVector("_BlurParams", new Vector4(0f, this.blurOffset.y, this.blurNum, 0f));
				this.material_.SetPass(1);
				Graphics.DrawMeshNow(this.screenQuad_, Matrix4x4.identity);
				this.material_.SetTexture("_ReflectionTexture", temporary3);
			}
			bool flag5 = this.preViewProj_ == Matrix4x4.identity;
			if (flag5)
			{
				this.preViewProj_ = matrix4x;
			}
			Graphics.SetRenderTarget(this.accumulationTextures_[0]);
			this.material_.SetMatrix("_PreViewProj", this.preViewProj_);
			this.material_.SetTexture("_PreAccumulationTexture", this.accumulationTextures_[1]);
			this.material_.SetPass(2);
			Graphics.DrawMeshNow(this.screenQuad_, Matrix4x4.identity);
			this.material_.SetTexture("_AccumulationTexture", this.accumulationTextures_[0]);
			RenderTexture renderTexture = this.accumulationTextures_[1];
			this.accumulationTextures_[1] = this.accumulationTextures_[0];
			this.accumulationTextures_[0] = renderTexture;
			this.preViewProj_ = matrix4x;
			bool flag6 = this.useSmoothness;
			if (flag6)
			{
				this.material_.EnableKeyword("USE_SMOOTHNESS");
				this.material_.SetTexture("_ReflectionTexture", this.accumulationTextures_[1]);
				Graphics.SetRenderTarget(temporary2);
				this.material_.SetVector("_BlurParams", new Vector4(this.blurOffset.x, 0f, (float)this.maxSmoothness, 0f));
				this.material_.SetPass(1);
				Graphics.DrawMeshNow(this.screenQuad_, Matrix4x4.identity);
				Graphics.SetRenderTarget(temporary3);
				this.material_.SetTexture("_ReflectionTexture", temporary2);
				this.material_.SetVector("_BlurParams", new Vector4(0f, this.blurOffset.y, (float)this.maxSmoothness, 0f));
				this.material_.SetPass(1);
				Graphics.DrawMeshNow(this.screenQuad_, Matrix4x4.identity);
				this.material_.SetTexture("_SmoothnessTexture", temporary3);
			}
			else
			{
				this.material_.DisableKeyword("USE_SMOOTHNESS");
			}
			Graphics.SetRenderTarget(dst);
			Graphics.Blit(src, dst, this.material_, 3);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
		}
	}

	// Token: 0x04000011 RID: 17
	[SerializeField]
	private Shader shader;

	// Token: 0x04000012 RID: 18
	[Header("Quality")]
	[SerializeField]
	private ScreenSpaceLocalReflection.Quality quality = ScreenSpaceLocalReflection.Quality.Middle;

	// Token: 0x04000013 RID: 19
	[Range(0f, 1f)]
	[SerializeField]
	private float resolution = 0.5f;

	// Token: 0x04000014 RID: 20
	[Header("Raytrace")]
	[SerializeField]
	private float raytraceMaxLength = 2f;

	// Token: 0x04000015 RID: 21
	[SerializeField]
	private float raytraceMaxThickness = 0.2f;

	// Token: 0x04000016 RID: 22
	[Header("Blur")]
	[SerializeField]
	private Vector2 blurOffset = new Vector2(1f, 1f);

	// Token: 0x04000017 RID: 23
	[Range(0f, 10f)]
	[SerializeField]
	private uint blurNum = 3U;

	// Token: 0x04000018 RID: 24
	[Header("Reflection")]
	[Range(0f, 5f)]
	[SerializeField]
	private float reflectionEnhancer = 1f;

	// Token: 0x04000019 RID: 25
	[Header("Smoothness")]
	[SerializeField]
	private bool useSmoothness = false;

	// Token: 0x0400001A RID: 26
	[Range(3f, 10f)]
	[SerializeField]
	private int maxSmoothness = 5;

	// Token: 0x0400001B RID: 27
	[Header("Accumulation")]
	[Range(0f, 1f)]
	[SerializeField]
	private float accumulationBlendRatio = 0.1f;

	// Token: 0x0400001C RID: 28
	private Material material_;

	// Token: 0x0400001D RID: 29
	private Mesh screenQuad_;

	// Token: 0x0400001E RID: 30
	private RenderTexture[] accumulationTextures_ = new RenderTexture[2];

	// Token: 0x0400001F RID: 31
	private Matrix4x4 preViewProj_ = Matrix4x4.identity;

	// Token: 0x02000033 RID: 51
	private enum Quality
	{
		// Token: 0x04000156 RID: 342
		High,
		// Token: 0x04000157 RID: 343
		Middle,
		// Token: 0x04000158 RID: 344
		Low
	}
}
