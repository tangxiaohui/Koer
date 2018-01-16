using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class Cards : DataBase 
{ 
	#region load and get funtion
	private static Dictionary<int, Cards> m_DicDatas = null;
	public static Dictionary<int, Cards> DicDatas
	{
		get
		{
			Load();
			return m_DicDatas;
		}
	}
	private static List<Cards> m_Datas = null;
	public static List<Cards> Datas
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
			Stream fs = OpenData("Cards.bin");
			if (fs != null)
			{
				BinaryReader br = new BinaryReader(fs);
				ushort dataNum = br.ReadUInt16();
				m_DicDatas = new Dictionary<int, Cards>(dataNum + 1);
				m_Datas = new List<Cards>(dataNum + 1);
				for (int i = 0; i < dataNum; ++i)
				{
					Cards data = new Cards();
					data.Load(br);

					if (m_DicDatas.ContainsKey(data.ID))
					{
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

	public static Cards Get(int ID)
	{
		Load();
		Cards data = null;
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
       private  int  m_CardID;
       public  int  CardID  { get { return  m_CardID ; } } 
       private  string  m_Name;
       public  string  Name  { get { return  m_Name ; } } 
       private  string  m_NameForEditor;
       public  string  NameForEditor  { get { return  m_NameForEditor ; } } 
       private  int  m_Attributed;
       public  int  Attributed  { get { return  m_Attributed ; } } 
       private  int  m_Rare;
       public  int  Rare  { get { return  m_Rare ; } } 
       private  int  m_Soul;
       public  int  Soul  { get { return  m_Soul ; } } 
       private  int  m_Power;
       public  int  Power  { get { return  m_Power ; } } 
       private  int  m_Hp;
       public  int  Hp  { get { return  m_Hp ; } } 
       private  int  m_Ac;
       public  int  Ac  { get { return  m_Ac ; } } 
       private  int  m_Atk;
       public  int  Atk  { get { return  m_Atk ; } } 
       private  int  m_Str;
       public  int  Str  { get { return  m_Str ; } } 
       private  int  m_Agi;
       public  int  Agi  { get { return  m_Agi ; } } 
       private  int  m_Int;
       public  int  Int  { get { return  m_Int ; } } 
       private  int  m_Luck;
       public  int  Luck  { get { return  m_Luck ; } } 
       private  int  m_Focus;
       public  int  Focus  { get { return  m_Focus ; } } 
#endregion

#region load funtion
public override void Load(BinaryReader pStream)
{
        m_ID=  pStream.ReadInt32 ();
        m_CardID=  pStream.ReadInt32 ();
        //m_Name= TextManager.Instance.GetString(pStream.ReadInt32 ());
        m_NameForEditor=  ReadUTFString (pStream);
        m_Attributed=  pStream.ReadInt32 ();
        m_Rare=  pStream.ReadInt32 ();
        m_Soul=  pStream.ReadInt32 ();
        m_Power=  pStream.ReadInt32 ();
        m_Hp=  pStream.ReadInt32 ();
        m_Ac=  pStream.ReadInt32 ();
        m_Atk=  pStream.ReadInt32 ();
        m_Str=  pStream.ReadInt32 ();
        m_Agi=  pStream.ReadInt32 ();
        m_Int=  pStream.ReadInt32 ();
        m_Luck=  pStream.ReadInt32 ();
        m_Focus=  pStream.ReadInt32 ();
}
#endregion

} 
