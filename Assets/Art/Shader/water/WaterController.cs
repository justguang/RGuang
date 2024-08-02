using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.IEffect
{
    /// <summary>
    /// 水特效
    /// </summary>
    public class WaterController : MonoBehaviour
    {
        [Header("水特效的启用状态"), SerializeField] private bool m_enableWater = false;
        [Header("水特效的贴图"), SerializeField] private Texture m_waterMainTex;

        [Header("水流速"), SerializeField] private float m_waterSpeed = 2.0f;
        [Header("水折射率"), SerializeField] private float m_waterRefraction = 0.50f;
        [Header("水中折射光密度"), SerializeField] private float m_waterRefractedLightDensity = 0.30f;

        [Header("水高度"), SerializeField, Range(0.0f, 1.0f)] private float m_waterHeight = 0.60f;
        [Header("水颜色"), SerializeField] private Color m_waterColor = Color.white;
        [Header("水浪大小"), SerializeField] private float m_waterWaveScale = 0.30f;

        [Header("浪花启用状态"), SerializeField] private bool m_enableWaterFoam = false;
        [Header("浪花颜色"), SerializeField] private Color m_waterFoamColor = Color.white;

        [Header("整体特效材质透明度"), SerializeField, Range(0.0f, 1.0f)] private float m_waterAlpha = 1.0f;


        #region Shader PropertyID
        private int m_waterMainTexID = Shader.PropertyToID("_MainTex");

        private int m_waterSpeedID = Shader.PropertyToID("_WaterSpeed");
        private int m_waterRefractionID = Shader.PropertyToID("_Refraction");
        private int m_waterRefractedLightDensityID = Shader.PropertyToID("_RefractedLightDensity");

        private int m_waterHeightID = Shader.PropertyToID("_WaterHeight");
        private int m_waterColorID = Shader.PropertyToID("_WaterColor");
        private int m_waterWaveScaleID = Shader.PropertyToID("_WaveScale");

        private int m_enableWaterFoamID = Shader.PropertyToID("_EnableFoam");
        private int m_waterFoamID = Shader.PropertyToID("_FoamColor");

        private int m_waterAlphaID = Shader.PropertyToID("_WaterAlpha");
        #endregion

        #region 影响的目标【按顺序查找】
        private Image m_img;
        private SpriteRenderer m_spriteRender;
        private MeshRenderer m_render;
        #endregion


        [Space(20.0f)]
        //水特效材质
        [SerializeField] private Material m_waterMat;
        //存储目标默认材质
        [SerializeField] private Material m_defaultMat;
        //描边材质用的shader名
        private string m_shaderName = "Shader Graphs/水";

        #region Unity Func
        private void Awake()
        {
            m_img = transform.GetComponent<Image>();
            m_spriteRender = transform.GetComponent<SpriteRenderer>();
            m_render = transform.GetComponent<MeshRenderer>();

            InstanceRes();
        }

        private void OnDestroy()
        {
            SwitchMaterial2Default();
            DisposeRes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetEnableWater(m_enableWater);
            SetWaterMainTex(m_waterMainTex);
            SetWaterSpeed(m_waterSpeed);
            SetWaterRefraction(m_waterRefraction);
            SetWaterRefractedLightDensity(m_waterRefractedLightDensity);
            SetWaterHeight(m_waterHeight);
            SetWaterColor(m_waterColor);
            SetWaterWaveScale(m_waterWaveScale);
            SetEnableWaterFoam(m_enableWaterFoam);
            SetWaterFoamColor(m_waterFoamColor);
            SetWaterAlpha(m_waterAlpha);
        }
#endif
        #endregion

        #region Getter
        /// <summary>
        /// 获取水特效的启用状态【true启用】
        /// </summary>
        /// <returns></returns>
        public bool GetEnableWater() => m_enableWater;
        /// <summary>
        /// 获取水特效的贴图
        /// </summary>
        /// <returns></returns>
        public Texture GetWaterMainTex() => m_waterMainTex;
        /// <summary>
        /// 获取水流速
        /// </summary>
        /// <returns></returns>
        public float GetWaterSpeed() => m_waterSpeed;
        /// <summary>
        /// 获取水的折射率
        /// </summary>
        /// <returns></returns>
        public float GetWaterRefraction() => m_waterRefraction;
        /// <summary>
        /// 获取水折射光的密度
        /// </summary>
        /// <returns></returns>
        public float GetWaterRefractedLightDensity() => m_waterRefractedLightDensity;
        /// <summary>
        /// 获取水高度
        /// </summary>
        /// <returns></returns>
        public float GetWaterHeight() => m_waterHeight;
        /// <summary>
        /// 获取水颜色
        /// </summary>
        /// <returns></returns>
        public Color GetWaterColor() => m_waterColor;
        /// <summary>
        /// 获取水浪大小
        /// </summary>
        /// <returns></returns>
        public float GetWaterWaveScale() => m_waterWaveScale;
        /// <summary>
        /// 获取浪花启用状态【true启用】
        /// </summary>
        /// <returns></returns>
        public bool GetEnableWaterFoam() => m_enableWaterFoam;
        /// <summary>
        /// 获取浪花颜色
        /// </summary>
        /// <returns></returns>
        public Color GetWaterFoamColor() => m_waterFoamColor;
        /// <summary>
        /// 获取整体水特效的透明度
        /// </summary>
        /// <returns></returns>
        public float GetWaterAlpha() => m_waterAlpha;
        #endregion

        #region Setter
        /// <summary>
        /// 设置水特效启用状态【true启用】
        /// </summary>
        /// <param name="enableWater"></param>
        public void SetEnableWater(bool enableWater)
        {
            m_enableWater = enableWater;
            if (m_enableWater)
            {
                SwitchMaterial2Water();
            }
            else
            {
                SwitchMaterial2Default();
            }
        }
        /// <summary>
        /// 设置水特效的贴图
        /// </summary>
        /// <param name="waterMainTex"></param>
        public void SetWaterMainTex(Texture waterMainTex)
        {
            m_waterMainTex = waterMainTex;
            RefreshWaterMainTex();
        }
        /// <summary>
        /// 设置水流速
        /// </summary>
        /// <param name="waterSpeed"></param>
        public void SetWaterSpeed(float waterSpeed)
        {
            m_waterSpeed = UnityEngine.Mathf.Max(waterSpeed, 0.0f);
            RefreshWaterSpeed();
        }
        /// <summary>
        /// 设置水的折射率
        /// </summary>
        /// <param name="waterRefraction"></param>
        public void SetWaterRefraction(float waterRefraction)
        {
            m_waterRefraction = UnityEngine.Mathf.Max(waterRefraction, 0.0f);
            RefreshWaterRefraction();
        }
        /// <summary>
        /// 设置水折射的光密度
        /// </summary>
        /// <param name="waterRefractedLightDensity"></param>
        public void SetWaterRefractedLightDensity(float waterRefractedLightDensity)
        {
            m_waterRefractedLightDensity = UnityEngine.Mathf.Max(waterRefractedLightDensity, 0.0f);
            RefreshWaterRefractedLightDensity();
        }
        /// <summary>
        /// 设置水高度
        /// </summary>
        /// <param name="waterHeight"></param>
        public void SetWaterHeight(float waterHeight)
        {
            m_waterHeight = UnityEngine.Mathf.Clamp(waterHeight, 0.0f, 1.0f);
            RefreshWaterHeight();
        }
        /// <summary>
        /// 设置水的颜色
        /// </summary>
        /// <param name="waterColor"></param>
        public void SetWaterColor(Color waterColor)
        {
            m_waterColor = waterColor;
            RefreshWaterColor();
        }
        /// <summary>
        /// 设置水浪大小
        /// </summary>
        /// <param name="waterWaveScale"></param>
        public void SetWaterWaveScale(float waterWaveScale)
        {
            m_waterWaveScale = UnityEngine.Mathf.Max(waterWaveScale, 0.0f);
            RefreshWaterWaveScale();
        }
        /// <summary>
        /// 设置水浪花的启用状态【true启用】
        /// </summary>
        /// <param name="enableWaterFoam"></param>
        public void SetEnableWaterFoam(bool enableWaterFoam)
        {
            m_enableWaterFoam = enableWaterFoam;
            RefreshEnableWaterFoam();
        }
        /// <summary>
        /// 设置水浪花的颜色
        /// </summary>
        /// <param name="waterFoamColor"></param>
        public void SetWaterFoamColor(Color waterFoamColor)
        {
            m_waterFoamColor = waterFoamColor;
            RefreshWaterFoamColor();
        }
        /// <summary>
        /// 设置整体水特效的透明度
        /// </summary>
        /// <param name="waterAlpha"></param>
        public void SetWaterAlpha(float waterAlpha)
        {
            m_waterAlpha = UnityEngine.Mathf.Clamp01(waterAlpha);
            RefreshWaterAlpha();
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
        private void SwitchMaterial2Water()
        {
            if (m_img != null)
            {
                m_img.material = m_waterMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_waterMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_waterMat;
            }
        }

        #region 刷新水特效材质数据
        private void RefreshWaterMainTex()
        {
            if (m_waterMat) m_waterMat.SetTexture(m_waterMainTexID, GetWaterMainTex());
        }
        private void RefreshWaterSpeed()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterSpeedID, GetWaterSpeed());
        }
        private void RefreshWaterRefraction()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterRefractionID, GetWaterRefraction());
        }
        private void RefreshWaterRefractedLightDensity()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterRefractedLightDensityID, GetWaterRefractedLightDensity());
        }
        private void RefreshWaterHeight()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterHeightID, GetWaterHeight());
        }
        private void RefreshWaterColor()
        {
            if (m_waterMat) m_waterMat.SetColor(m_waterColorID, GetWaterColor());
        }
        private void RefreshWaterWaveScale()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterWaveScaleID, GetWaterWaveScale());
        }
        private void RefreshEnableWaterFoam()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_enableWaterFoamID, GetEnableWaterFoam() ? 1.0f : 0.0f);
        }
        private void RefreshWaterFoamColor()
        {
            if (m_waterMat) m_waterMat.SetColor(m_waterFoamID, GetWaterFoamColor());
        }
        private void RefreshWaterAlpha()
        {
            if (m_waterMat) m_waterMat.SetFloat(m_waterAlphaID, GetWaterAlpha());
        }
        #endregion

        #region Res Mgr
        private void InstanceRes()
        {
            DisposeRes();
            m_waterMat = Instantiate(new Material(Shader.Find(m_shaderName)));
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


            RefreshWaterMainTex();
            RefreshWaterSpeed();
            RefreshWaterRefraction();
            RefreshWaterRefractedLightDensity();
            RefreshWaterHeight();
            RefreshWaterColor();
            RefreshWaterWaveScale();
            RefreshEnableWaterFoam();
            RefreshWaterFoamColor();
            RefreshWaterAlpha();
        }
        private void DisposeRes()
        {
            if (m_waterMat) DestroyImmediate(m_waterMat, true);
            m_waterMat = null;
        }
        #endregion

    }

}