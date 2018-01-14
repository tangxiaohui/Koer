using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common 
{
public class Root
{
    static bool _hasInitialized = false;
    static GameObject _node = null;
    static Transform _root = null;
    static RootCoroutine _coroutine = null;
    static RootCoroutine[] _loadingCoroutines = null;
    const int loadingCoroutineCount = 5;

    public static bool hasInitialized {
        get {
            return _hasInitialized;
        }
    }

    public static Transform root {
        get {
#if UNITY_EDITOR
            CheckInitialized();
#endif
            return _root;
        }
    }

    public static GameObject node {
        get {
#if UNITY_EDITOR
            CheckInitialized();
#endif
            return _node;
        }
    }

    static void CheckInitialized() 
    {
        if ( !_hasInitialized )
        {
            Initialize();
        }
    }

    public static void UnInitialize()
    {
        if(_node != null)
        {
            if(_loadingCoroutines != null)
            {
                for(int i = 0; i < _loadingCoroutines.Length; ++i)
                {
                    _loadingCoroutines[i].StopAllCoroutines();
                }
                _loadingCoroutines = null;
            }
            if(_coroutine != null)
            {
                _coroutine.StopAllCoroutines();
                _coroutine = null;
            }
            GameObject.DestroyImmediate(_node);
            _node = null;
            _hasInitialized = false;
        } 
    }
    public static void Initialize() 
    {
        if ( _node == null ) 
        {
            _hasInitialized = true;
            _node = new GameObject( "[ProgramRoot]" );
            _coroutine = _node.AddComponent<RootCoroutine>();
            _coroutine.hideFlags |= HideFlags.HideInInspector;
            _loadingCoroutines = new RootCoroutine[loadingCoroutineCount];
            for ( int i = 0; i < loadingCoroutineCount; ++i ) 
            {
                _loadingCoroutines[i] = _node.AddComponent<RootCoroutine>();
                _loadingCoroutines[i].hideFlags |= HideFlags.HideInInspector;
            }
            _root = _node.transform;
            UnityLogFile.Instance.SetParent(_root);

            InitRenderFeature();
            GameObject.DontDestroyOnLoad(_node);
        }
    }

    public static RootCoroutine[] loadingCoros {
        get {
            return _loadingCoroutines;
        }
    }

    public static RootCoroutine coro {
        get {
#if UNITY_EDITOR
            CheckInitialized();
#endif
            return _coroutine;
        }
    }

    #region render feature
    private const int SupportFeaturePostProcess = 0;
    private const int SupportFeatureDynamicShadow = 1;
    private const int SupportFeatureMaxTextureSize2048 = 2;
    private const int SupportFeatureRenderTexture = 3;
    private const int SupportFeatureRenderTarget30 = 4;
    private const int SupportFeatureHighPerformance = 5;
    private const int SupportFeatureNum = 6;

    public static BitArray m_feature = new BitArray(SupportFeatureNum);
    public static BitArray feature
    {
        get { return m_feature; }
    }
    public static bool supportFeatureDynamiShadow { get { return m_feature[SupportFeatureDynamicShadow]; } }
    public static bool supportFeaturePostProcess { get { return m_feature[SupportFeaturePostProcess]; } }
    public static bool supportFeatureHighPerformance { get { return m_feature[SupportFeatureHighPerformance]; } }

    private static void InitRenderFeature()
    {
        
        m_feature[SupportFeaturePostProcess]        = SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures;
        m_feature[SupportFeatureDynamicShadow]      = SystemInfo.supportsRenderTextures && 
                                                      SystemInfo.graphicsShaderLevel >= 30 &&
                                                      (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8));
        m_feature[SupportFeatureMaxTextureSize2048] = SystemInfo.maxTextureSize >= 2048;
        m_feature[SupportFeatureRenderTexture]      = SystemInfo.supportsRenderTextures;
        m_feature[SupportFeatureRenderTarget30]     = SystemInfo.graphicsShaderLevel >= 30;
        m_feature[SupportFeatureHighPerformance]    = SystemInfo.systemMemorySize > 1024 ;

#if UNITY_IPHONE
        var aspectRatio = (float)Screen.width / Screen.height;
        m_feature[SupportFeatureHighPerformance]    = SystemInfo.systemMemorySize > 768 && aspectRatio > 1.5f;
#endif

#if UNITY_ANDROID
        var pattern = @"\bAdreno.*[3-9][3-9][0-9]|\bAdreno.*[5-9][0-9][0-9]|\bMali-T8[6-9][0-9]\s+MP[6-9]|\bMali.*MP8|.*MP12";  //GPU匹配 是否是高端
        m_feature[SupportFeatureHighPerformance]  &= Regex.IsMatch(SystemInfo.graphicsDeviceName, pattern);
#endif
        m_feature[SupportFeatureDynamicShadow] &=  m_feature[SupportFeatureHighPerformance];
#if _DEBUG_INFO
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.deviceModel: [{0}]",SystemInfo.deviceModel));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.deviceName: [{0}]", SystemInfo.deviceName));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsDeviceName: [{0}]", SystemInfo.graphicsDeviceName));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsDeviceID: [{0}]", SystemInfo.graphicsDeviceID));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsDeviceVendor: [{0}]", SystemInfo.graphicsDeviceVendor));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsDeviceVendorID: [{0}]", SystemInfo.graphicsDeviceVendorID));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsDeviceVersion: [{0}]", SystemInfo.graphicsDeviceVersion));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsMemorySize: [{0}]", SystemInfo.graphicsMemorySize));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.graphicsShaderLevel: [{0}]", SystemInfo.graphicsShaderLevel));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.maxTextureSize: [{0}]", SystemInfo.maxTextureSize));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.npotSupport: [{0}]", SystemInfo.npotSupport.ToString()));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.operatingSystem: [{0}]", SystemInfo.operatingSystem));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.processorType: [{0}]", SystemInfo.processorType));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.supportedRenderTargetCount: [{0}]", SystemInfo.supportedRenderTargetCount));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.supportsImageEffects: [{0}]", SystemInfo.supportsImageEffects));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.supportsRenderTextures: [{0}]", SystemInfo.supportsRenderTextures));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.SupportsRenderTextureFormat Depth: [{0}]", SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.supportsShadows: [{0}]", SystemInfo.supportsShadows));
        UnityEngine.Debug.LogWarning(String.Format("SystemInfo.systemMemorySize: [{0}]", SystemInfo.systemMemorySize));
#endif

    }
    #endregion
    #region start coroutine function and run callback function
    public static void PostCall( Action function, float delayTime = 0 ) {
        coro.StartCoroutine( _PostCall( function, delayTime ) );
    }

    public static void PostCall<T>( Action<T> function, T arg, float delayTime = 0 ) {
        coro.StartCoroutine( _PostCall<T>( function, arg, delayTime ) );
    }

    public static void PostCall<T1, T2>( Action<T1, T2> function, T1 arg1, T2 arg2, float delayTime = 0 ) {
        coro.StartCoroutine( _PostCall<T1, T2>( function, arg1, arg2, delayTime ) );
    }

    public static void PostCall<T1, T2, T3>( Action<T1, T2, T3> function, T1 arg1, T2 arg2, T3 arg3, float delayTime = 0 ) {
        coro.StartCoroutine( _PostCall<T1, T2, T3>( function, arg1, arg2, arg3, delayTime ) );
    }

    public static void PostCall<T1, T2, T3, T4>( Action<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, float delayTime = 0 ) {
        coro.StartCoroutine( _PostCall<T1, T2, T3, T4>( function, arg1, arg2, arg3, arg4, delayTime ) );
    }

    static IEnumerator _PostCall( Action function, float delayTime ) {
        yield return new WaitForSeconds( delayTime );
        function.DynamicInvoke();
    }

    static IEnumerator _PostCall<T>( Action<T> function, T arg, float delayTime ) {
        yield return new WaitForSeconds( delayTime );
        function.DynamicInvoke( arg );
    }

    static IEnumerator _PostCall<T1, T2>( Action<T1, T2> function, T1 arg1, T2 arg2, float delayTime ) {
        yield return new WaitForSeconds( delayTime );
        function.DynamicInvoke( arg1, arg2 );
    }

    static IEnumerator _PostCall<T1, T2, T3>( Action<T1, T2, T3> function, T1 arg1, T2 arg2, T3 arg3, float delayTime ) {
        yield return new WaitForSeconds( delayTime );
        function.DynamicInvoke( arg1, arg2, arg3 );
    }

    static IEnumerator _PostCall<T1, T2, T3, T4>( Action<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, float delayTime ) {
        yield return new WaitForSeconds( delayTime );
        function.DynamicInvoke( arg1, arg2, arg3, arg4 );
    }


    static public void DoDelegateAction( Action action, Action<Action> action1 ) {
        if ( action != null ) {
            action();
        }
    }

    public static void RunAction( System.Action action ) {
        if ( action != null ) {
            action();
            action = null;
        }
    }

    public static void RunAction<T>( System.Action<T> action, T arg ) {
        if ( action != null ) {
            action( arg );
            action = null;
        }
    }

    public static void RunAction<T1, T2>( System.Action<T1, T2> action, T1 arg1, T2 arg2 ) {
        if ( action != null ) {
            action( arg1, arg2 );
            action = null;
        }
    }

    public static void RunAction<T1, T2, T3>( System.Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3 ) {
        if ( action != null ) {
            action( arg1, arg2, arg3 );
            action = null;
        }
    }
    #endregion runaction
    
}

[ExecuteInEditMode]
public class RootCoroutine : MonoBehaviour
{

}

}
//EOF
