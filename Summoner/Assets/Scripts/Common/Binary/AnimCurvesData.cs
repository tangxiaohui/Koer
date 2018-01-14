using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class AnimCurvesData : MonoBehaviour
    {
        public static string DefautResourcesAnimationCurvePath = "Prefabs/AnimCurves/";
        public static string DefautStreamingAssetsAnimationCurvePath = "AnimCurves/";
        public static string CurveExtName = "_animationCurve";
        public string[] curveNames;
        public AnimationCurve[] curveDatas;
        public string[] curveCameraNames;
        public AnimationCurve[] curveCameraDatas;

        private List<string> nameList;

        public string[] curveYNames;
        public AnimationCurve[] curveYDatas;
        public string[] curveYCameraNames;
        public AnimationCurve[] curveYCameraDatas;

        private List<string> nameYList;

        void OnStart()
        {
            nameList = new List<string>(curveNames);
            nameYList = new List<string>(curveYNames);
        }
        //以KEY值获取动画曲线
        public AnimationCurve ZCurve(string key)
        {
            if (nameList == null)
            {
                nameList = new List<string>(curveNames);
            }
            if (nameList.Contains(key))
            {
                return curveDatas[nameList.IndexOf(key)];
            }
            return null;

        }
        //以KEY值获取Y轴动画曲线
        public AnimationCurve YCurve(string key)
        {
            if (nameYList == null)
            {
                nameYList = new List<string>(curveYNames);
            }
            if (nameYList.Contains(key))
            {
                return curveYDatas[nameYList.IndexOf(key)];
            }
            return null;
        }

#if UNITY_EDITOR
        private  static AnimationCurve BinarySerializer_ReadAnimationCurve(BinaryReader reader)
        {
            var keyFrameLength = Common.BinarySerializer.Read_Int32(reader);
            var keyFrames = new Keyframe[keyFrameLength];
            for(int i = 0; i < keyFrameLength; ++i)
            {
                keyFrames[i].time           = Common.BinarySerializer.Read_Single(reader);
                keyFrames[i].value          = Common.BinarySerializer.Read_Single(reader);
                keyFrames[i].inTangent      = Common.BinarySerializer.Read_Single(reader);
                keyFrames[i].outTangent     = Common.BinarySerializer.Read_Single(reader);
                keyFrames[i].tangentMode    = Common.BinarySerializer.Read_Int32(reader);
            }
            return new AnimationCurve(keyFrames);
        }
        public void ImportBytes(string fileName)
        {
            var path = "Assets/StreamingAssets/AnimCurves/" + fileName + CurveExtName + ".bytes";

            try 
            {
                using (var stream = File.OpenRead(path))
                {
                    if(stream != null)
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        if (reader != null)
                        {
                            curveNames  = null;
                            curveDatas  = null;
                            curveYNames = null;
                            curveYDatas = null;
                            curveCameraDatas = null;
                            curveYCameraDatas = null;

                            var zanimationCurveCount = Common.BinarySerializer.Read_Int32(reader);
                            if(zanimationCurveCount > 0)
                            {
                                curveNames = new string[zanimationCurveCount];
                                curveDatas = new AnimationCurve[zanimationCurveCount];
                                curveCameraDatas = new AnimationCurve[zanimationCurveCount];
                                for(int i = 0; i < zanimationCurveCount; ++i)
                                {
                                    curveNames[i] = Common.BinarySerializer.Read_String(reader);
                                    curveDatas[i] = BinarySerializer_ReadAnimationCurve(reader);
                                    curveCameraDatas[i] = BinarySerializer_ReadAnimationCurve(reader);
                                }
                            }
                            var yanimationCurveCount = Common.BinarySerializer.Read_Int32(reader);
                            if(yanimationCurveCount > 0)
                            {
                                curveYNames = new string[yanimationCurveCount];
                                curveYDatas = new AnimationCurve[yanimationCurveCount];
                                curveYCameraDatas = new AnimationCurve[yanimationCurveCount];
                                for(int i = 0; i < yanimationCurveCount; ++i)
                                {
                                    curveYNames[i] = Common.BinarySerializer.Read_String(reader);
                                    curveYDatas[i] = BinarySerializer_ReadAnimationCurve(reader);
                                    curveYCameraDatas[i] = BinarySerializer_ReadAnimationCurve(reader);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(path);
                UnityEngine.Debug.LogException(e);
            }
        }
        public void ExportBytes(string fileName)
        {
            var path = "Assets/StreamingAssets/AnimCurves/" + fileName + CurveExtName + ".bytes";

            try 
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(fs);

                if(curveDatas != null && curveNames != null && curveDatas.Length == curveNames.Length)
                {
                    var count = curveDatas.Length;
                    BinarySerializer.Write_Int32(writer, count);
                    for(int i = 0; i < count; ++i)
                    {
                        BinarySerializer.Write_String(writer, curveNames[i]);
                        var animationCurveValue = curveDatas[i];
                        if(animationCurveValue != null)
                        {
                            var keys = animationCurveValue.keys;
                            BinarySerializer.Write_Int32(writer, keys.Length);
                            Array.ForEach(keys, 
                                keyFrame =>
                                {
                                    BinarySerializer.Write_Single(writer, keyFrame.time);
                                    BinarySerializer.Write_Single(writer, keyFrame.value);
                                    BinarySerializer.Write_Single(writer, keyFrame.inTangent);
                                    BinarySerializer.Write_Single(writer, keyFrame.outTangent);
                                    BinarySerializer.Write_Int32(writer, keyFrame.tangentMode);
                                }
                            );
                        }
                        var animationCurveCameraValue = curveCameraDatas[i];
                        if(animationCurveCameraValue != null)
                        {
                            var keys = animationCurveCameraValue.keys;
                            BinarySerializer.Write_Int32(writer, keys.Length);
                            Array.ForEach(keys, 
                                keyFrame =>
                                {
                                    BinarySerializer.Write_Single(writer, keyFrame.time);
                                    BinarySerializer.Write_Single(writer, keyFrame.value);
                                    BinarySerializer.Write_Single(writer, keyFrame.inTangent);
                                    BinarySerializer.Write_Single(writer, keyFrame.outTangent);
                                    BinarySerializer.Write_Int32(writer, keyFrame.tangentMode);
                                }
                            );
                        }
                    }
                }

                if(curveYDatas != null && curveYNames != null && curveYDatas.Length == curveYNames.Length)
                {
                    var count = curveYDatas.Length;
                    BinarySerializer.Write_Int32(writer, count);
                    for(int i = 0; i < count; ++i)
                    {
                        BinarySerializer.Write_String(writer, curveYNames[i]);
                        var animationCurveYValue = curveYDatas[i];
                        if(animationCurveYValue != null)
                        {
                            var keys = animationCurveYValue.keys;
                            BinarySerializer.Write_Int32(writer, keys.Length);
                            Array.ForEach(keys, 
                                keyFrame =>
                                {
                                    BinarySerializer.Write_Single(writer, keyFrame.time);
                                    BinarySerializer.Write_Single(writer, keyFrame.value);
                                    BinarySerializer.Write_Single(writer, keyFrame.inTangent);
                                    BinarySerializer.Write_Single(writer, keyFrame.outTangent);
                                    BinarySerializer.Write_Int32(writer, keyFrame.tangentMode);
                                }
                            );
                        }
                        var animationCurveCameraYValue = curveYCameraDatas[i];
                        if(animationCurveCameraYValue != null)
                        {
                            var keys = animationCurveCameraYValue.keys;
                            BinarySerializer.Write_Int32(writer, keys.Length);
                            Array.ForEach(keys, 
                                keyFrame =>
                                {
                                    BinarySerializer.Write_Single(writer, keyFrame.time);
                                    BinarySerializer.Write_Single(writer, keyFrame.value);
                                    BinarySerializer.Write_Single(writer, keyFrame.inTangent);
                                    BinarySerializer.Write_Single(writer, keyFrame.outTangent);
                                    BinarySerializer.Write_Int32(writer, keyFrame.tangentMode);
                                }
                            );
                        }
                    }
                }

                fs.Flush();
                fs.Close();
                fs = null;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }

        }
#endif
    }
}