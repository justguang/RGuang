using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 受击特效
/// Shader = [Shader Graphs/2D受击特效]
/// </summary>
public class HitEffectController : MonoBehaviour
{
    [Header("从当前效果缓动到目标效果的时长[秒]")]
    [SerializeField] private float m_duration = 0.150f;
    [Header("效果颜色")]
    [SerializeField] private Color m_color = Color.white;

    private int m_hitEffectAmountID = Shader.PropertyToID("_HitEffectAmount");
    private int m_hitEffectColorID = Shader.PropertyToID("_HitEffectColor");

    #region 影响的目标【按顺序查找】
    //目标
    private Image m_img;
    private SpriteRenderer m_spriteRender;
    #endregion

    //目标受击的材质
    [SerializeField] private Material m_hitEffectMat;
    //目标默认材质
    [SerializeField] private Material m_defaultMat;
    //材质使用的shader名
    private string m_shaderName = "Shader Graphs/2D受击特效";

    //效果动画进度
    private float m_lerpAmount;

    #region Unity Fun
    private void Awake()
    {
        m_img = transform.GetComponent<Image>();
        m_spriteRender = transform.GetComponent<SpriteRenderer>();

        InstenceRes();

        RefreshColor();
    }

    private void OnDestroy()
    {
        SwitchMaterial2Default();
        DisposeRes();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetDuration(m_duration);
        SetColor(m_color);
        RefreshColor();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.A))
    //    {
    //        Debug.Log($"[{this.gameObject}] hitEffectController Test => update:【KeyCode.A】");
    //        PlayHitEffect(null);
    //    }
    //    else if (Input.GetKeyUp(KeyCode.S))
    //    {
    //        Debug.Log($"[{this.gameObject}] hitEffectController Test => update:【KeyCode.S】");
    //        PlayUnHitEffect(null);
    //    }
    //    else if (Input.GetKeyUp(KeyCode.D))
    //    {
    //        Debug.Log($"[{this.gameObject}] hitEffectController Test => update:【KeyCode.D】");
    //        PlayHitEffect(() => PlayUnHitEffect(SwitchMaterial2Default));
    //    }
    //}
#endif
    #endregion


    #region Public Func
    /// <summary>
    /// 播放受击特效
    /// </summary>
    /// <param name="onFinishCalback">播放完毕后的回调</param>
    public void PlayHitEffect(Action onFinishCalback)
    {
        SetterLerpValue(0.0f);
        SwitchMaterial2HitEffect();
        DOTween.To(GetterLerpValue, SetterLerpValue, 1.0f, GetDuration()).SetEase(Ease.OutExpo).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCalback?.Invoke());
    }
    /// <summary>
    /// 播放反向受击特效
    /// </summary>
    /// <param name="onFinishCalback">播放完毕后的回调</param>
    public void PlayUnHitEffect(Action onFinishCalback)
    {
        SetterLerpValue(1.0f);
        SwitchMaterial2HitEffect();
        DOTween.To(GetterLerpValue, SetterLerpValue, 0.0f, GetDuration()).SetEase(Ease.InExpo).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCalback?.Invoke());
    }
    /// <summary>
    /// 缓动到目标效果
    /// </summary>
    /// <param name="targetValue">目标值【0~1】（0为正常值，1为反应值）</param>
    /// <param name="onFinishCallback">缓动完成后的回调</param>
    public void DoTarget(float targetValue, Action onFinishCallback)
    {
        targetValue = UnityEngine.Mathf.Clamp01(targetValue);
        SwitchMaterial2HitEffect();
        DOTween.To(GetterLerpValue, SetterLerpValue, targetValue, GetDuration()).SetEase(Ease.InOutExpo).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCallback?.Invoke());
    }
    /// <summary>
    /// 这是动画时长【秒】
    /// </summary>
    /// <param name="newDuration">时长【0~max】秒</param>
    public void SetDuration(float newDuration)
    {
        m_duration = UnityEngine.Mathf.Max(newDuration, 0.0f);
    }
    /// <summary>
    /// 获取动画时长【秒】
    /// </summary>
    /// <returns></returns>
    public float GetDuration() => m_duration;
    /// <summary>
    /// 设置效果颜色
    /// </summary>
    /// <param name="newColor"></param>
    public void SetColor(Color newColor)
    {
        m_color = newColor;
    }
    /// <summary>
    /// 获取效果颜色
    /// </summary>
    /// <returns></returns>
    public Color GetColor() => m_color;
    /// <summary>
    /// 切换默认材质
    /// </summary>
    public void SwitchMaterial2Default()
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
    /// <summary>
    /// 切换受击材质
    /// </summary>
    public void SwitchMaterial2HitEffect()
    {
        if (m_img != null)
        {
            m_img.material = m_hitEffectMat;
        }
        else if (m_spriteRender != null)
        {
            m_spriteRender.material = m_hitEffectMat;
        }
    }
    #endregion


    private void RefreshColor()
    {
        if (m_hitEffectMat) m_hitEffectMat.SetColor(m_hitEffectColorID, GetColor());
    }
    private void OnLerpUpdate()
    {
        if (m_hitEffectMat) m_hitEffectMat.SetFloat(m_hitEffectAmountID, GetterLerpValue());
    }
    private float GetterLerpValue() => m_lerpAmount;
    private void SetterLerpValue(float newValue) => m_lerpAmount = UnityEngine.Mathf.Clamp01(newValue);

    #region Res Mgr
    private void InstenceRes()
    {
        DisposeRes();
        m_hitEffectMat = Instantiate(new Material(Shader.Find(m_shaderName)));
        if (m_img != null)
        {
            m_defaultMat = m_img.material;
        }
        else if (m_spriteRender != null)
        {
            m_defaultMat = m_spriteRender.material;
        }
    }
    private void DisposeRes()
    {
        if (m_hitEffectMat) DestroyImmediate(m_hitEffectMat, true);
        m_hitEffectMat = null;
    }
    #endregion

}

