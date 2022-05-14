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
		int settingHash = GetSettingHash();
		if (settingHash != 0 && settingHash != bufferedHash)
		{
			MakeMesh(sprite, x, y, meshType);
			Image component = GetComponent<Image>();
			if ((bool)component)
			{
				component.useSpriteMesh = false;
				component.useSpriteMesh = true;
			}
			bufferedHash = settingHash;
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
		bool flag = meshType == MeshType.CenterPoint;
		int num = x + 1;
		int num2 = y + 1;
		int num3 = num * num2;
		Vector2[] array;
		ushort[] array2;
		if (flag)
		{
			array = new Vector2[num3 + x * y];
			array2 = new ushort[x * y * 12];
		}
		else
		{
			array = new Vector2[num3];
			array2 = new ushort[x * y * 6];
		}
		for (int i = 0; i < num; i++)
		{
			float num4 = (float)i / (float)x;
			for (int j = 0; j < num2; j++)
			{
				float num5 = (float)j / (float)y;
				array[num * j + i] = new Vector2(num4, num5);
			}
		}
		if (flag)
		{
			for (int k = 0; k < x; k++)
			{
				float num6 = ((float)k + 0.5f) / (float)x;
				for (int l = 0; l < y; l++)
				{
					float num7 = ((float)l + 0.5f) / (float)y;
					array[l * x + k + num3] = new Vector2(num6, num7);
				}
			}
			for (int m = 0; m < x; m++)
			{
				for (int n = 0; n < y; n++)
				{
					int num8 = 12 * (n * x + m);
					int num9 = n * num + m;
					ushort num10 = (ushort)(n * x + m + num3);
					ushort[] array3 = array2;
					int num11 = num8 + 11;
					ushort num12;
					array2[num8] = (num12 = (ushort)num9);
					array3[num11] = num12;
					ushort[] array4 = array2;
					int num13 = num8 + 3;
					array2[num8 + 2] = (num12 = (ushort)(num9 + 1));
					array4[num13] = num12;
					ushort[] array5 = array2;
					int num14 = num8 + 6;
					array2[num8 + 5] = (num12 = (ushort)(num9 + num + 1));
					array5[num14] = num12;
					ushort[] array6 = array2;
					int num15 = num8 + 9;
					array2[num8 + 8] = (num12 = (ushort)(num9 + num));
					array6[num15] = num12;
					ushort[] array7 = array2;
					int num16 = num8 + 1;
					ushort[] array8 = array2;
					int num17 = num8 + 4;
					ushort[] array9 = array2;
					int num18 = num8 + 7;
					array2[num8 + 10] = (num12 = num10);
					array9[num18] = (num12 = num12);
					array8[num17] = (num12 = num12);
					array7[num16] = num12;
				}
			}
		}
		else if (meshtype == MeshType.forward)
		{
			for (int num19 = 0; num19 < x; num19++)
			{
				for (int num20 = 0; num20 < y; num20++)
				{
					int num21 = 6 * (num20 * x + num19);
					int num22 = num20 * num + num19;
					ushort[] array10 = array2;
					int num23 = num21 + 5;
					ushort num12;
					array2[num21 + 1] = (num12 = (ushort)num22);
					array10[num23] = num12;
					array2[num21] = (ushort)(num22 + 1);
					ushort[] array11 = array2;
					int num24 = num21 + 4;
					array2[num21 + 2] = (num12 = (ushort)(num22 + num + 1));
					array11[num24] = num12;
					array2[num21 + 3] = (ushort)(num22 + num);
				}
			}
		}
		else if (meshType == MeshType.backward)
		{
			for (int num25 = 0; num25 < x; num25++)
			{
				for (int num26 = 0; num26 < y; num26++)
				{
					int num27 = 6 * (num26 * x + num25);
					int num28 = num26 * num + num25;
					array2[num27] = (ushort)num28;
					ushort[] array12 = array2;
					int num29 = num27 + 4;
					ushort num12;
					array2[num27 + 2] = (num12 = (ushort)(num28 + 1));
					array12[num29] = num12;
					array2[num27 + 3] = (ushort)(num28 + num + 1);
					ushort[] array13 = array2;
					int num30 = num27 + 5;
					array2[num27 + 1] = (num12 = (ushort)(num28 + num));
					array13[num30] = num12;
				}
			}
		}
		sprite.OverrideGeometry(array, array2);
	}
}
