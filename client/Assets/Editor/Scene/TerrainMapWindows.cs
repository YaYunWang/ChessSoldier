using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
/// <summary>
/// terrain editor
/// wangyy
/// </summary>
public class TerrainMapWindows : EditorWindow
{
	[MenuItem("Window/Terrain/Import Raw")]
	public static void ShowTerrainMapWindows()
	{
		EditorWindow.GetWindow<TerrainMapWindows> ();
	}

	private GameObject m_terrain_gameobject = null;//
	private int m_texture_width = 1024;
	private int m_texture_height = 1024;
	private MyDepth m_depth = MyDepth.Bit8;
	private MyByteOrder m_byte_order = MyByteOrder.Windows;
	private Vector3 m_terrain_size = new Vector3 (2000f, 600f, 2000f);

	private bool m_changer_data = false;
	private bool m_flip_vertically = false;
	void OnGUI()
	{
		m_terrain_gameobject = EditorGUILayout.ObjectField ("Terrain", m_terrain_gameobject, typeof(UnityEngine.Object), true) as GameObject;
		m_texture_width = EditorGUILayout.IntField ("Texture Width", m_texture_width);
		m_texture_height = EditorGUILayout.IntField ("Texture Height", m_texture_height);
		m_depth = (MyDepth)EditorGUILayout.EnumPopup ("Depth", m_depth);
		m_byte_order = (MyByteOrder)EditorGUILayout.EnumPopup ("ByteOrder", m_byte_order);
		m_terrain_size = EditorGUILayout.Vector3Field ("TerrainSize", m_terrain_size);

		m_changer_data = EditorGUILayout.Toggle ("ChangeTerrainSize", m_changer_data);
		m_flip_vertically = EditorGUILayout.Toggle ("FlipVertically", m_flip_vertically);

		if (GUILayout.Button ("import raw")) 
		{
			string text = EditorUtility.OpenFilePanel("Import Raw Heightmap", string.Empty, "raw");
			if (text == string.Empty) {
				return;
			}
			byte[] array = GetRawData(text);
			if (array == null || array.Length <= 0) {
				Debug.LogError ("texture is null");
				return;
			}


			List<Terrain> terrain_list = GetTerrains ();
			if (terrain_list == null || terrain_list.Count <= 0) {
				Debug.LogError ("Terrain is null");
				return;
			}
			if (m_changer_data) {
				InitTerrainDataSize (terrain_list);
			}

			for (int idx = 0; idx < terrain_list.Count; idx++) {
				TerrainData terrainData = terrain_list [idx].terrainData;
				if (terrainData == null)
					continue;
				int index_x = 0;
				int index_z = 0;
				bool flg = GetTerrainIndex (terrain_list, idx, out index_x, out index_z);
				if (!flg) {
					continue;
				}

				int heightmapWidth = terrainData.heightmapWidth;
				int heightmapHeight = terrainData.heightmapHeight;
				float[,] array2 = new float[heightmapHeight, heightmapWidth];
				if (this.m_depth == MyDepth.Bit16)
				{
					float num = 1.52587891E-05f;
					for (int i = 0; i < heightmapHeight; i++)
					{
						for (int j = 0; j < heightmapWidth; j++)
						{
							int num2 = Mathf.Clamp(j + index_x * heightmapWidth, 0, this.m_texture_width - 1) + Mathf.Clamp(i + index_z * heightmapHeight, 0, this.m_texture_height - 1) * this.m_texture_width;
							if (this.m_byte_order == MyByteOrder.Mac == BitConverter.IsLittleEndian)
							{
								byte b = array[num2 * 2];
								array[num2 * 2] = array[num2 * 2 + 1];
								array[num2 * 2 + 1] = b;
							}
							ushort num3 = BitConverter.ToUInt16(array, num2 * 2);
							float num4 = (float)num3 * num;
							int num5 = (!this.m_flip_vertically) ? i : (heightmapHeight - 1 - i);
							array2[num5, j] = num4;
						}
					}
				}
				else
				{
					float num6 = 0.00390625f;
					for (int k = 0; k < heightmapHeight; k++)
					{
						for (int l = 0; l < heightmapWidth; l++)
						{
							int num7 = Mathf.Clamp(l + index_x * heightmapWidth, 0, this.m_texture_width - 1) + Mathf.Clamp(k + index_z * heightmapHeight, 0, this.m_texture_height - 1) * this.m_texture_width;
							byte b2 = array[num7];
							float num8 = (float)b2 * num6;
							int num9 = (!this.m_flip_vertically) ? k : (heightmapHeight - 1 - k);
							array2[num9, l] = num8;
						}
					}
				}
				terrainData.SetHeights(0, 0, array2);

			}

			FlushHeightmap (terrain_list);
		}
	}

	private void FlushHeightmap(List<Terrain> list)
	{
		for (int idx = 0; idx < list.Count; idx++) {
			list [idx].Flush ();
		}
	}

	private bool GetTerrainIndex(List<Terrain> list, int index, out int index_x, out int index_z)
	{
		index_x = 0;
		index_z = 0;
		if (list == null || list.Count <= 0 || index < 0 || index >= list.Count) {
			return false;
		}

		Vector3 frist_pos = list [0].transform.localPosition;

		Vector3 index_pos = list [index].transform.localPosition;
		Vector3 tmp_pos = index_pos - frist_pos;
		index_x = (int)(tmp_pos.x / m_terrain_size.x);
		index_z = (int)(tmp_pos.z / m_terrain_size.z);
		if (index_x < 0) {
			index_x = 0;
		}
		if (index_z < 0) {
			index_z = 0;
		}

		return true;
	}

	private void InitTerrainDataSize(List<Terrain> terrain_list)
	{
		for (int idx = 0; idx < terrain_list.Count; idx++) {
			terrain_list [idx].terrainData.size = m_terrain_size;
		}
	}

	private byte[] GetRawData(string path)
	{
		byte[] array;
		using (BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
		{
			array = binaryReader.ReadBytes(this.m_texture_width * this.m_texture_height * (int)this.m_depth);
			binaryReader.Close();
		}
		return array;
	}
	private List<Terrain> GetTerrains()
	{
		if (m_terrain_gameobject == null) 
		{
			return null;
		}
		Terrain[] terrain = m_terrain_gameobject.transform.GetComponentsInChildren<Terrain> ();
		List<Terrain> list = new List<Terrain> (terrain); 
		list.Sort (SortTerrains);
		return list;
	}

	private static int SortTerrains(Terrain terrain1, Terrain terrain2)
	{
		if ((terrain1 == null) || (terrain2 == null))
		{
			return 0;
		}
		Transform t1 = terrain1.transform;
		Transform t2 = terrain2.transform;

		if (t1.position.z > t2.position.z) {
			return 1;
		} else if (t1.position.z < t2.position.z) {
			return -1;
		} else if (t1.position.x > t2.position.x) {
			return 1;
		} else if(t1.position.x < t2.position.x){
			return -1;
		}
		return 0;
	}

	private enum MyDepth
	{
		Bit8 = 1,
		Bit16
	}

	private enum MyByteOrder
	{
		Mac = 1,
		Windows
	}
}
