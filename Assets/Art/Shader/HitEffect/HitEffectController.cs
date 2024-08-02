using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CardGame.IEffect
{
    /// <summary>
    /// 受击特效
    /// Shader = [Shader Graphs/2D受击特效]
    /// </summary>
    public class HitEffectController : MonoBehaviour
    {
        [Header("启用受击特效的状态"), SerializeField] private bool m_enableHitEfect = false;
        [Header("受击贴图"), SerializeField] private Texture m_hitEffectMainTex;
        [Header("从当前效果缓动到目标效果的时长[秒]")]
        [SerializeField] private float m_duration = 0.150f;
        [Header("效果颜色")]
        [SerializeField] private Color m_color = Color.white;

        private int m_hitEffectMainTexID = Shader.PropertyToID("_MainTex");
        private int m_hitEffectAmountID = Shader.PropertyToID("_HitEffectAmount");
        private int m_hitEffectColorID = Shader.PropertyToID("_HitEffectColor");

        #region 影响的目标【按顺序查找】
        //目标
        private Image m_img;
        private SpriteRenderer m_spriteRender;
        private MeshRenderer m_render;
        #endregion

        [Space(20.0f)]
        //目标受击的材质
        [SerializeField] private Material m_hitEffectMat;
        //目标默认材质
        [SerializeField] private Material m_defaultMat;
        //材质使用的shader名
        private string m_shaderName = "Shader Graphs/受击特效";

        //效果动画进度
        private float m_lerpAmount;

        #region Unity Fun
        private void Awake()
        {
            m_img = transform.GetComponent<Image>();
            m_spriteRender = transform.GetComponent<SpriteRenderer>();
            m_render = transform.GetComponent<MeshRenderer>();

            InstenceRes();
        }

        private void OnDestroy()
        {
            SwitchMaterial2Default();
            DisposeRes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetEnableHitEffect(m_enableHitEfect);
            SetHitEffecDuration(m_duration);
            SetColor(m_color);
            SetHitEffectMainTex(m_hitEffectMainTex);
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
        //        PlayHitEffect(() => PlayUnHitEffect(() => SetEnableHitEffect(false)));
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
            SetEnableHitEffect(true);
            DOTween.To(GetterLerpValue, SetterLerpValue, 1.0f, GetDuration()).SetEase(Ease.OutExpo).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCalback?.Invoke());
        }
        /// <summary>
        /// 播放反向受击特效
        /// </summary>
        /// <param name="onFinishCalback">播放完毕后的回调</param>
        public void PlayUnHitEffect(Action onFinishCalback)
        {
            SetterLerpValue(1.0f);
            SetEnableHitEffect(true);
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
            if (GetEnableHitEffect() == false) SetEnableHitEffect(true);
            DOTween.To(GetterLerpValue, SetterLerpValue, targetValue, GetDuration()).SetEase(Ease.InOutExpo).OnUpdate(OnLerpUpdate).OnComplete(() => onFinishCallback?.Invoke());
        }

        public bool GetEnableHitEffect() => m_enableHitEfect;
        public void SetEnableHitEffect(bool enableHitEffect)
        {
            m_enableHitEfect = enableHitEffect;
            if (m_enableHitEfect)
            {
                SwitchMaterial2HitEffect();
            }
            else
            {
                SwitchMaterial2Default();
            }
        }
        public float GetDuration() => m_duration;
        public void SetHitEffecDuration(float newDuration)
        {
            m_duration = UnityEngine.Mathf.Max(newDuration, 0.0f);
        }
        public Color GetColor() => m_color;
        public void SetColor(Color newColor)
        {
            m_color = newColor;
            RefreshHitEffectColor();
        }
        public Texture GetHitEffectMainTex() => m_hitEffectMainTex;
        public void SetHitEffectMainTex(Texture hitEffectMainTex)
        {
            m_hitEffectMainTex = hitEffectMainTex;
            RefreshHitEffectMainTex();
        }
        #endregion


        private void SwitchMaterial2Default()
        {
            if (m_img != null)
            {
                m_img.material = m_defaultMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_defaultMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_defaultMat;
            }
        }
        private void SwitchMaterial2HitEffect()
        {
            if (m_img != null)
            {
                m_img.material = m_hitEffectMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_hitEffectMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_hitEffectMat;
            }
        }
        private void RefreshHitEffectColor()
        {
            if (m_hitEffectMat) m_hitEffectMat.SetColor(m_hitEffectColorID, GetColor());
        }
        private void RefreshHitEffectMainTex()
        {
            if (m_hitEffectMat) m_hitEffectMat.SetTexture(m_hitEffectMainTexID, GetHitEffectMainTex());
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
            else if (m_render != null)
            {
                m_defaultMat = m_render.material;
            }

            RefreshHitEffectMainTex();
            RefreshHitEffectColor();
        }
        private void DisposeRes()
        {
            if (m_hitEffectMat) DestroyImmediate(m_hitEffectMat, true);
            m_hitEffectMat = null;
        }
        #endregion

    }

}