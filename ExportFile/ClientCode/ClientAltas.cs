using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class ClientAltas : DataBase 
{ 
	#region load and get funtion
	private static Dictionary<int, ClientAltas> m_DicDatas = null;
	public static Dictionary<int, ClientAltas> DicDatas
	{
		get
		{
			Load();
			return m_DicDatas;
		}
	}
	private static List<ClientAltas> m_Datas = null;
	public static List<ClientAltas> Datas
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
			Stream fs = OpenData("ClientAltas.bin");
			if (fs != null)
			{
				BinaryReader br = new BinaryReader(fs);
				ushort dataNum = br.ReadUInt16();
				m_DicDatas = new Dictionary<int, ClientAltas>(dataNum + 1);
				m_Datas = new List<ClientAltas>(dataNum + 1);
				for (int i = 0; i < dataNum; ++i)
				{
					ClientAltas data = new ClientAltas();
					data.Load(br);

					if (m_DicDatas.ContainsKey(data.ID))
					{
                        Debug.LogError("fuck you mate, ID:" + data.ID + " already exists in ClientAltas!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        continue;
                    }

                    m_DicDatas.Add(data.ID, data);
					m_Datas.Add(data);
				}
				br.Close();
				br = null;
				fs.Close();
				fs = null;
			}
		}
	}

	public static ClientAltas Get(int ID)
	{
		Load();
		ClientAltas data = null;
		if(m_DicDatas.TryGetValue(ID, out data))
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
	
	
#region member and property
       private  int  m_ID;
       public  int  ID  { get { return  m_ID ; } } 
       private  string  m_name;
       public  string  name  { get { return  m_name ; } } 
       private  string  m_remark;
       public  string  remark  { get { return  m_remark ; } } 
#endregion

#region load funtion
public override void Load(BinaryReader pStream)
{
        m_ID=  pStream.ReadInt32 ();
        m_name=  ReadUTFString (pStream);
        m_remark=  ReadUTFString (pStream);
}
#endregion

} 
