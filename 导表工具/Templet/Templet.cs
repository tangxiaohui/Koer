using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class Templet : DataBase 
{ 
	#region load and get funtion
	private static Dictionary<TemKey, Templet> m_DicDatas = null;
	public static Dictionary<TemKey, Templet> DicDatas
	{
		get
		{
			Load();
			return m_DicDatas;
		}
	}
	private static List<Templet> m_Datas = null;
	public static List<Templet> Datas
	{
		get
		{
			Load();
			return m_Datas;
		}
	}
	public static int Count
	{
		get
		{
			Load();
			return m_Datas.Count;
		}
	}
	public static void Load()
	{
		if (m_DicDatas == null || m_Datas == null)
		{
			Stream fs = OpenData("Templet.bin");
			if (fs != null)
			{
				BinaryReader br = new BinaryReader(fs);
				ushort dataNum = br.ReadUInt16();
				m_DicDatas = new Dictionary<TemKey, Templet>(dataNum + 1);
				m_Datas = new List<Templet>(dataNum + 1);
				for (int i = 0; i < dataNum; ++i)
				{
					Templet data = new Templet();
					data.Load(br);

					if (m_DicDatas.ContainsKey(data.MainKey))
					{
                        Debug.LogError("fuck you mate, ID:" + data.ID + " already exists in Templet!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        continue;
                    }

                    m_DicDatas.Add(data.MainKey, data);
					m_Datas.Add(data);
				}
				br.Close();
				br = null;
				fs.Close();
				fs = null;
			}
		}
	}

	public static Templet Get(TemKey MainKey)
	{
		Load();
		Templet data = null;
		if(m_DicDatas.TryGetValue(MainKey, out data))
		{
			return data;
		}
		return null;
	}
	
	public static void Reload()
	{
		Unload();
		Load();
	}
	
	public static void Unload()
	{
		bool bGC = false;
		if(m_DicDatas != null)
		{
			m_DicDatas.Clear();
			m_DicDatas = null;
			bGC = true;
		}
				
		if(m_Datas != null)
		{
			m_Datas.Clear();
			m_Datas = null;
			bGC = true;
		}
		
		if(bGC)
		{
			System.GC.Collect();
		}
	}

	#endregion
	
	ReadContent
} 
