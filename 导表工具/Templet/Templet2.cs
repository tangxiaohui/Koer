using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class Templet : DataBase 
{ 
	#region load and get funtion
	private static Dictionary<TemKey, Dictionary<Tem2Key,Templet>> m_DicDatas = null;
	public static Dictionary<TemKey, Dictionary<Tem2Key,Templet>> DicDatas
	{
		get
		{
			Load();
			return m_DicDatas;
		}
	}
	
	private static Dictionary<TemKey, List<Templet>> m_DicDatasList = null;
	public static Dictionary<TemKey, List<Templet>> DicDatasList
	{
		get
		{
			Load();
			return m_DicDatasList;
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
		if (m_DicDatas == null || m_Datas == null || m_DicDatasList == null)
		{
			Stream fs = OpenData("Templet.bin");
			if (fs != null)
			{
				BinaryReader br = new BinaryReader(fs);
				ushort dataNum = br.ReadUInt16();
				m_DicDatas = new Dictionary<TemKey, Dictionary<Tem2Key,Templet>>();
				m_Datas = new List<Templet>(dataNum + 1);
				m_DicDatasList = new Dictionary<TemKey, List<Templet>>(dataNum + 1);
				for (int i = 0; i < dataNum; ++i)
				{
					Templet data = new Templet();
					data.Load(br);
					#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					/*if (m_DicDatas.ContainsKey(data.MainKey))
					{
						UDebug.Assert(false, "Templet encountered duplicate keys, " + data.MainKey);
					}*/
					#endif
					if(!m_DicDatas.ContainsKey(data.MainKey))
					{
						m_DicDatas[data.MainKey] = new Dictionary<Tem2Key,Templet>();
					}
					
					if(!m_DicDatasList.ContainsKey(data.MainKey))
					{
						m_DicDatasList[data.MainKey] = new List<Templet>();
					}
					
					m_DicDatas[data.MainKey][data.Main2Key] = data;
					m_DicDatasList[data.MainKey].Add(data);
					m_Datas.Add(data);
				}
				br.Close();
				br = null;
				fs.Close();
				fs = null;
			}
		}
	}

	public static List<Templet> Get(TemKey MainKey)
	{
		Load();
		List<Templet> data = null;
		if(m_DicDatasList.TryGetValue(MainKey, out data))
		{
			return data;
		}
		return null;
	}
	
	public static Templet Get(TemKey MainKey,Tem2Key Main2Key)
	{
		Load();
		Dictionary<Tem2Key,Templet> dataVaule = null;
		if(m_DicDatas.TryGetValue(MainKey,out dataVaule))
		{
			Templet data = null;
			if(dataVaule.TryGetValue(Main2Key,out data))
			{
				return data;
			}
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
		
		if(m_DicDatasList != null)
		{
			m_DicDatasList.Clear();
			m_DicDatasList = null;
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
