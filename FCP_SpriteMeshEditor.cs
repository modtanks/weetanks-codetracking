using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FCP_SpriteMeshEditor : MonoBehaviour
{
	public enum MeshType
	{
		CenterPoint,
		forward,
		backward
	}

	public int x;

	public int y;

	public MeshType meshType;

	public Sprite sprite;

	private int bufferedHash;

	private void Update()
	{
		int hash = GetSettingHash();
		if (hash != 0 && hash != bufferedHash)
		{
			MakeMesh(sprite, x, y, meshType);
			Image im = GetComponent<Image>();
			if ((bool)im)
			{
				im.useSpriteMesh = false;
				im.useSpriteMesh = true;
			}
			bufferedHash = hash;
		}
	}

	private int GetSettingHash()
	{
		if (sprite == null || x <= 0 || y <= 0)
		{
			return 0;
		}
		return sprite.GetHashCode() * (x ^ 0x88) * (y ^ 0x53E) * (int)((meshType + 1) ^ (MeshType)99999);
	}

	private void MakeMesh(Sprite sprite, int x, int y, MeshType meshtype)
	{
		bool centerPoints = meshType == MeshType.CenterPoint;
		int px = x + 1;
		int py = y + 1;
		int t = px * py;
		Vector2[] verts;
		ushort[] faces;
		if (centerPoints)
		{
			verts = new Vector2[t + x * y];
			faces = new ushort[x * y * 12];
		}
		else
		{
			verts = new Vector2[t];
			faces = new ushort[x * y * 6];
		}
		for (int m = 0; m < px; m++)
		{
			float xi = (float)m / (float)x;
			for (int n = 0; n < py; n++)
			{
				float yi = (float)n / (float)y;
				verts[px * n + m] = new Vector2(xi, yi);
			}
		}
		if (centerPoints)
		{
			for (int l = 0; l < x; l++)
			{
				float xi2 = ((float)l + 0.5f) / (float)x;
				for (int j4 = 0; j4 < y; j4++)
				{
					float yi2 = ((float)j4 + 0.5f) / (float)y;
					verts[j4 * x + l + t] = new Vector2(xi2, yi2);
				}
			}
			for (int k = 0; k < x; k++)
			{
				for (int j5 = 0; j5 < y; j5++)
				{
					int f3 = 12 * (j5 * x + k);
					int s3 = j5 * px + k;
					ushort ns = (ushort)(j5 * x + k + t);
					ushort[] array = faces;
					int num = f3 + 11;
					ushort num2;
					faces[f3] = (num2 = (ushort)s3);
					array[num] = num2;
					ushort[] array2 = faces;
					int num3 = f3 + 3;
					faces[f3 + 2] = (num2 = (ushort)(s3 + 1));
					array2[num3] = num2;
					ushort[] array3 = faces;
					int num4 = f3 + 6;
					faces[f3 + 5] = (num2 = (ushort)(s3 + px + 1));
					array3[num4] = num2;
					ushort[] array4 = faces;
					int num5 = f3 + 9;
					faces[f3 + 8] = (num2 = (ushort)(s3 + px));
					array4[num5] = num2;
					ushort[] array5 = faces;
					int num6 = f3 + 1;
					ushort[] array6 = faces;
					int num7 = f3 + 4;
					ushort[] array7 = faces;
					int num8 = f3 + 7;
					faces[f3 + 10] = (num2 = ns);
					array7[num8] = (num2 = num2);
					array6[num7] = (num2 = num2);
					array5[num6] = num2;
				}
			}
		}
		else if (meshtype == MeshType.forward)
		{
			for (int j = 0; j < x; j++)
			{
				for (int j3 = 0; j3 < y; j3++)
				{
					int f2 = 6 * (j3 * x + j);
					int s2 = j3 * px + j;
					ushort[] array8 = faces;
					int num9 = f2 + 5;
					ushort num2;
					faces[f2 + 1] = (num2 = (ushort)s2);
					array8[num9] = num2;
					faces[f2] = (ushort)(s2 + 1);
					ushort[] array9 = faces;
					int num10 = f2 + 4;
					faces[f2 + 2] = (num2 = (ushort)(s2 + px + 1));
					array9[num10] = num2;
					faces[f2 + 3] = (ushort)(s2 + px);
				}
			}
		}
		else if (meshType == MeshType.backward)
		{
			for (int i = 0; i < x; i++)
			{
				for (int j2 = 0; j2 < y; j2++)
				{
					int f = 6 * (j2 * x + i);
					int s = j2 * px + i;
					faces[f] = (ushort)s;
					ushort[] array10 = faces;
					int num11 = f + 4;
					ushort num2;
					faces[f + 2] = (num2 = (ushort)(s + 1));
					array10[num11] = num2;
					faces[f + 3] = (ushort)(s + px + 1);
					ushort[] array11 = faces;
					int num12 = f + 5;
					faces[f + 1] = (num2 = (ushort)(s + px));
					array11[num12] = num2;
				}
			}
		}
		sprite.OverrideGeometry(verts, faces);
	}
}
