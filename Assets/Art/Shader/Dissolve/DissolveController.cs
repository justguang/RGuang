using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 溶解特效
/// Shader = [Shader Graphs/溶解]
/// </summary>
public class DissolveController : MonoBehaviour
{
    [Header("溶解时长[秒]"), SerializeField] private float m_duration = 1.5f;
    [Header("溶解噪点"), SerializeField] private float m_dissolvePoint = 70.0f;
    [Header("单个溶解点大小"), SerializeField, Range(0.0f, 1.0f)] private float m_dissolveScale = 0.010f;
    [Header("溶解出的颜色"), SerializeField] private Color m_dissolveColor = Color.gray;

    #region Shader PropertyID
    //溶解进度【0~1】
    private int m_dissolveValueID = Shader.PropertyToID("_DissolveValue");

    //溶解噪点
    private int m_noiseScaleID = Shader.PropertyToID("_NoiseScale");

    //溶解面积，单个溶解点大小【0~1】
    private int m_dissolveScaleID = Shader.PropertyToID("_DissolveScale");

    //溶解出的颜色
    private int m_dissolveColorID = Shader.PropertyToID("_Color");
    #endregion

    private float m_dissolveValue;//当前溶解进度

    #region 影响的目标【按顺序查找】
    private Image m_img;
    private SpriteRenderer m_spriteRender;
    #endregion

    //溶解材质
    [SerializeField] private Material m_dissolveMat;
    //存储目标对象默认材质
    [SerializeField] private Material m_defaultMat;
    //材质用的shader名
    private string m_shaderName = "Shader Graphs/溶解";


    #region Unity Func
    private void Awake()
    {
        m_img = transform.GetComponent<Image>();
        m_spriteRender = transform.GetComponent<SpriteRenderer>();

        InstanceRes();

        ResetDissolveValue();
        RefreshNoiseScale();
        RefreshDissolveScale();
        RefreshColor();
    }
    private void OnDestroy()
    {
        SwitchMaterial2Defaul();
        DisposeRes();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        SetDissolveDuration(m_duration);
        SetDissolvePoint(m_dissolvePoint);
        SetDissolveScale(m_dissolveScale);
        SetDissolveColor(m_dissolveColor);

        RefreshNoiseScale();
        RefreshDissolveScale();
        RefreshColor();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.A))
    //    {
    //        Debug.Log($"[{this.gameObject}] dissolveController Test => update:【KeyCode.A】");
    //        PlayDissolve(null);
    //    }
    //    else if (Input.GetKeyUp(KeyCode.S))
    //    {
    //        Debug.Log($"[{this.gameObject}] dissolveController Test => update:【KeyCode.S】");
    //        PlayUnDissolve(null);
    //    }
    //    else if (Input.GetKeyUp(KeyCode.D))
    //    {
    //        Debug.Log($"[{this.gameObject}] dissolveController Test => update:【KeyCode.D】");
    //        PlayDissolve(() => PlayUnDissolve(SwitchMaterial2Defaul));
    //    }
    //}
#endif
    #endregion

    #region Public Func
    /// <summary>
    /// 溶解
    /// </summary>
    /// <param name="onFinishCallback">溶解完成后的回调</param>
    public void PlayDissolve(Action onFinishCallback)
    {
        SetterLerpValue(1.0f);
        SwitchMaterial2Dissolve();
        DOTween.To(GetterLerpValue, SetterLerpValue, 0.0f, GetDissolveDuration()).SetEase(Ease.InOutQuad).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCallback?.Invoke());
    }
    /// <summary>
    /// 反向溶解
    /// </summary>
    /// <param name="onFinishCallback">反向溶解完成后的回调</param>
    public void PlayUnDissolve(Action onFinishCallback)
    {
        SetterLerpValue(0.0f);
        SwitchMaterial2Dissolve();
        DOTween.To(GetterLerpValue, SetterLerpValue, 1.0f, GetDissolveDuration()).SetEase(Ease.InOutQuad).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCallback?.Invoke());
    }
    /// <summary>
    /// 切换可溶解材质
    /// </summary>
    public void SwitchMaterial2Dissolve()
    {
        if (m_img != null)
        {
            m_img.material = m_dissolveMat;
        }
        else if (m_spriteRender != null)
        {
            m_spriteRender.material = m_dissolveMat;
        }
    }
    /// <summary>
    /// 切换正常材质
    /// </summary>
    public void SwitchMaterial2Defaul()
    {
        if (m_img != null)
        {
            m_img.material = m_defaultMat;
        }
        else if (m_spriteRender != null)
        {
            m_spriteRender.material = m_defaultMat;
        }
    }
    #region Public Getter
    /// <summary>
    /// 获取溶解出的颜色
    /// </summary>
    /// <returns></returns>
    public Color GetDissolveColor() => m_dissolveColor;
    /// <summary>
    /// 获取单个溶解点的大小
    /// </summary>
    /// <returns></returns>
    public float GetDissolveScale() => m_dissolveScale;
    /// <summary>
    /// 获取溶解噪点
    /// </summary>
    /// <returns></returns>
    public float GetDissolvePoint() => m_dissolvePoint;
    /// <summary>
    /// 获取溶解时长【秒】
    /// </summary>
    /// <returns></returns>
    public float GetDissolveDuration() => m_duration;
    #endregion

    #region Public Setter
    /// <summary>
    /// 设置溶解出的颜色
    /// </summary>
    /// <param name="newDissolveColor"></param>
    public void SetDissolveColor(Color newDissolveColor)
    {
        m_dissolveColor = newDissolveColor;
    }
    /// <summary>
    /// 设置单个溶解点的大小
    /// </summary>
    /// <param name="newDissolveScale">大小【0~1】</param>
    public void SetDissolveScale(float newDissolveScale)
    {
        m_dissolveScale = UnityEngine.Mathf.Clamp01(newDissolveScale);
    }
    /// <summary>
    /// 设置溶解噪点数
    /// </summary>
    /// <param name="newDissolvePoint">噪点数【0~max】</param>
    public void SetDissolvePoint(float newDissolvePoint)
    {
        m_dissolvePoint = UnityEngine.Mathf.Max(newDissolvePoint, 0.0f);
    }
    /// <summary>
    /// 设置溶解总时长【秒】
    /// </summary>
    /// <param name="newDissolveDuration">时长【0~max】</param>
    public void SetDissolveDuration(float newDissolveDuration) => m_duration = UnityEngine.Mathf.Max(newDissolveDuration, 0);
    #endregion
    #endregion


    /// <summary>
    /// 刷新颜色
    /// </summary>
    private void RefreshColor()
    {
        if (m_dissolveMat) m_dissolveMat.SetColor(m_dissolveColorID, GetDissolveColor());
    }
    /// <summary>
    /// 刷新噪点
    /// </summary>
    private void RefreshNoiseScale()
    {
        if (m_dissolveMat) m_dissolveMat.SetFloat(m_noiseScaleID, GetDissolvePoint());
    }
    /// <summary>
    /// 刷新单个溶解点的大小
    /// </summary>
    private void RefreshDissolveScale()
    {
        if (m_dissolveMat) m_dissolveMat.SetFloat(m_dissolveScaleID, GetDissolveScale());
    }
    /// <summary>
    /// 重置溶解进度
    /// </summary>
    private void ResetDissolveValue()
    {
        SetterLerpValue(1.0f);
        OnLerpUpdate();
    }
    private void OnLerpUpdate()
    {
        if (m_dissolveMat) m_dissolveMat.SetFloat(m_dissolveValueID, GetterLerpValue());
    }
    private float GetterLerpValue() => m_dissolveValue;
    private void SetterLerpValue(float newValue) => m_dissolveValue = UnityEngine.Mathf.Clamp01(newValue);


    #region Res Mgr
    private void DisposeRes()
    {
        if (m_dissolveMat) DestroyImmediate(m_dissolveMat, true);
        m_dissolveMat = null;
    }
    private void InstanceRes()
    {
        DisposeRes();
        m_dissolveMat = Instantiate(new Material(Shader.Find(m_shaderName)));

        if (m_img != null)
        {
            m_defaultMat = m_img.material;
        }
        else if (m_spriteRender != null)
        {
            m_defaultMat = m_spriteRender.material;
        }
    }
    #endregion


}
