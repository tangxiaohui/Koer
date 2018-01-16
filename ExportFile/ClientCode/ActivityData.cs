using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class ActivityData : DataBase 
{ 
	#region load and get funtion
	private static Dictionary<int, Dictionary<int,ActivityData>> m_DicDatas = null;
	public static Dictionary<int, Dictionary<int,ActivityData>> DicDatas
	{
		get
		{
			Load();
			return m_DicDatas;
		}
	}
	
	private static Dictionary<int, List<ActivityData>> m_DicDatasList = null;
	public static Dictionary<int, List<ActivityData>> DicDatasList
	{
		get
		{
			Load();
			return m_DicDatasList;
		}
	}
	
	
	
	private static List<ActivityData> m_Datas = null;
	public static List<ActivityData> Datas
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
			Stream fs = OpenData("ActivityData.bin");
			if (fs != null)
			{
				BinaryReader br = new BinaryReader(fs);
				ushort dataNum = br.ReadUInt16();
				m_DicDatas = new Dictionary<int, Dictionary<int,ActivityData>>();
				m_Datas = new List<ActivityData>(dataNum + 1);
				m_DicDatasList = new Dictionary<int, List<ActivityData>>(dataNum + 1);
				for (int i = 0; i < dataNum; ++i)
				{
					ActivityData data = new ActivityData();
					data.Load(br);
					#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					/*if (m_DicDatas.ContainsKey(data.iActivityID_KEYID1))
					{
						UDebug.Assert(false, "ActivityData encountered duplicate keys, " + data.iActivityID_KEYID1);
					}*/
					#endif
					if(!m_DicDatas.ContainsKey(data.iActivityID_KEYID1))
					{
						m_DicDatas[data.iActivityID_KEYID1] = new Dictionary<int,ActivityData>();
					}
					
					if(!m_DicDatasList.ContainsKey(data.iActivityID_KEYID1))
					{
						m_DicDatasList[data.iActivityID_KEYID1] = new List<ActivityData>();
					}
					
					m_DicDatas[data.iActivityID_KEYID1][data.iActivityPosition_KEYID2] = data;
					m_DicDatasList[data.iActivityID_KEYID1].Add(data);
					m_Datas.Add(data);
				}
				br.Close();
				br = null;
				fs.Close();
				fs = null;
			}
		}
	}

	public static List<ActivityData> Get(int iActivityID_KEYID1)
	{
		Load();
		List<ActivityData> data = null;
		if(m_DicDatasList.TryGetValue(iActivityID_KEYID1, out data))
		{
			return data;
		}
		return null;
	}
	
	public static ActivityData Get(int iActivityID_KEYID1,int iActivityPosition_KEYID2)
	{
		Load();
		Dictionary<int,ActivityData> dataVaule = null;
		if(m_DicDatas.TryGetValue(iActivityID_KEYID1,out dataVaule))
		{
			ActivityData data = null;
			if(dataVaule.TryGetValue(iActivityPosition_KEYID2,out data))
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
	
	
#region member and property
       private  int  m_ID;
       public  int  ID  { get { return  m_ID ; } } 
       private  int  m_iActivityID_KEYID1;
       public  int  iActivityID_KEYID1  { get { return  m_iActivityID_KEYID1 ; } } 
       private  int  m_iActivityPosition_KEYID2;
       public  int  iActivityPosition_KEYID2  { get { return  m_iActivityPosition_KEYID2 ; } } 
       private  int  m_iActivityTypeForSevenTest;
       public  int  iActivityTypeForSevenTest  { get { return  m_iActivityTypeForSevenTest ; } } 
       private  int  m_iActivityType;
       public  int  iActivityType  { get { return  m_iActivityType ; } } 
       private  int  m_iDayNum;
       public  int  iDayNum  { get { return  m_iDayNum ; } } 
       private  string  m_iActivityName;
       public  string  iActivityName  { get { return  m_iActivityName ; } } 
       private  string  m_iActivityNameForEditor;
       public  string  iActivityNameForEditor  { get { return  m_iActivityNameForEditor ; } } 
       private  string  m_iChageGiftTips;
       public  string  iChageGiftTips  { get { return  m_iChageGiftTips ; } } 
       private  string  m_iChageGiftTipsForEditor;
       public  string  iChageGiftTipsForEditor  { get { return  m_iChageGiftTipsForEditor ; } } 
       private  int  m_iConditionType;
       public  int  iConditionType  { get { return  m_iConditionType ; } } 
       private  int  m_iRecordType;
       public  int  iRecordType  { get { return  m_iRecordType ; } } 
       private  int  m_iCompareType;
       public  int  iCompareType  { get { return  m_iCompareType ; } } 
       private  List <Vec3Vec>  m_iActivityConditionList;
       public  List <Vec3Vec>  iActivityConditionList  { get { return  m_iActivityConditionList ; } } 
       private  List <Vec3Vec>  m_vecList;
       public  List <Vec3Vec>  vecList  { get { return  m_vecList ; } } 
       private  int  m_iRefreshFlag;
       public  int  iRefreshFlag  { get { return  m_iRefreshFlag ; } } 
       private  int  m_iActivityTimeType;
       public  int  iActivityTimeType  { get { return  m_iActivityTimeType ; } } 
       private  string  m_strActivityShowTime;
       public  string  strActivityShowTime  { get { return  m_strActivityShowTime ; } } 
       private  string  m_strActivitiStartTime;
       public  string  strActivitiStartTime  { get { return  m_strActivitiStartTime ; } } 
       private  string  m_strActivitiEndTime;
       public  string  strActivitiEndTime  { get { return  m_strActivitiEndTime ; } } 
       private  string  m_strActivityDisappearTime;
       public  string  strActivityDisappearTime  { get { return  m_strActivityDisappearTime ; } } 
#endregion

#region load funtion
public override void Load(BinaryReader pStream)
{
        m_ID=  pStream.ReadInt32 ();
        m_iActivityID_KEYID1=  pStream.ReadInt32 ();
        m_iActivityPosition_KEYID2=  pStream.ReadInt32 ();
        m_iActivityTypeForSevenTest=  pStream.ReadInt32 ();
        m_iActivityType=  pStream.ReadInt32 ();
        m_iDayNum=  pStream.ReadInt32 ();
        m_iActivityName= TextManager.Instance.GetString(pStream.ReadInt32 ());
        m_iActivityNameForEditor=  ReadUTFString (pStream);
        m_iChageGiftTips= TextManager.Instance.GetString(pStream.ReadInt32 ());
        m_iChageGiftTipsForEditor=  ReadUTFString (pStream);
        m_iConditionType=  pStream.ReadInt32 ();
        m_iRecordType=  pStream.ReadInt32 ();
        m_iCompareType=  pStream.ReadInt32 ();
 ReadVec3VecList (pStream,ref        m_iActivityConditionList);
 ReadVec3VecList (pStream,ref        m_vecList);
        m_iRefreshFlag=  pStream.ReadInt32 ();
        m_iActivityTimeType=  pStream.ReadInt32 ();
        m_strActivityShowTime=  ReadUTFString (pStream);
        m_strActivitiStartTime=  ReadUTFString (pStream);
        m_strActivitiEndTime=  ReadUTFString (pStream);
        m_strActivityDisappearTime=  ReadUTFString (pStream);
}
#endregion

} 
