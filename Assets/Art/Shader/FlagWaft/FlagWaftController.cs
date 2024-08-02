using UnityEngine;
using UnityEngine.UI;

namespace RGuang.IEffect
{
    /// <summary>
    /// 旗帜飘扬特效
    /// </summary>
    public class FlagWaftController : MonoBehaviour
    {
        [Header("随风飘荡启用状态"), SerializeField] private bool m_enableFlagWaft = false;
        [Header("飘荡效果的贴图"), SerializeField] private Texture m_flagWaftMainTex;
        [Header("风速"), SerializeField] private float m_windSpeed = 0.50f;
        [Header("风大小"), SerializeField] private float m_windScale = 0.50f;
        [Header("风向"), SerializeField] private Vector2 m_windDir = new Vector2(1.0f, 0.0f);

        #region Shader PropertyID
        private int m_flagWaftMainTexID = Shader.PropertyToID("_MainTex");
        private int m_windSpeedID = Shader.PropertyToID("_WindSpeed");
        private int m_windScaleID = Shader.PropertyToID("_WindScale");
        private int m_windDirID = Shader.PropertyToID("_WindDirection");
        #endregion


        #region 影响的目标【按顺序查找】
        //影响的目标
        private Image m_img;
        private SpriteRenderer m_spriteRender;
        private MeshRenderer m_render;
        #endregion


        [Space(20.0f)]
        //目标飘扬材质
        [SerializeField] private Material m_flagWaftMat;
        //目标默认材质
        [SerializeField] private Material m_defaultMat;
        //像素化shader名
        private string m_shaderName = "Shader Graphs/旗帜飘扬";


        #region Unity Func
        private void Awake()
        {
            m_img = transform.GetComponent<Image>();
            m_spriteRender = transform.GetComponent<SpriteRenderer>();
            m_render = transform.GetComponent<MeshRenderer>();
            InstancRes();
        }

        private void OnDestroy()
        {
            SwitchMaterial2Default();
            DisposeRes();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetEnableFlagWaft(m_enableFlagWaft);
            SetFlagWaftMainTex(m_flagWaftMainTex);
            SetWindSpeed(m_windSpeed);
            SetWindScale(m_windScale);
            SetWindDirection(m_windDir);
        }
        //private void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.A))
        //    {
        //        Debug.Log($"[{this.gameObject}] flagWaftController Test => update:【KeyCode.A】");
        //        SetEnableFlagWaft(!m_enableFlagWaft);
        //    }
        //    else if (Input.GetKeyUp(KeyCode.S))
        //    {
        //        Debug.Log($"[{this.gameObject}] flagWaftController Test => update:【KeyCode.S】");
        //    }
        //    else if (Input.GetKeyUp(KeyCode.D))
        //    {
        //        Debug.Log($"[{this.gameObject}] flagWaftController Test => update:【KeyCode.D】");
        //    }

        //}
#endif

        #endregion



        #region Public Getter
        /// <summary>
        /// 获取飘扬的启用状态【true启用】
        /// </summary>
        /// <returns></returns>
        public bool GetEnableFlagWaft() => m_enableFlagWaft;
        /// <summary>
        /// 获取飘扬特效的贴图
        /// </summary>
        /// <returns></returns>
        public Texture GetFlagWaftMainTex() => m_flagWaftMainTex;
        /// <summary>
        /// 获取飘扬时的风速
        /// </summary>
        /// <returns></returns>
        public float GetWindSpeed() => m_windSpeed;
        /// <summary>
        /// 获取飘扬时的风大小
        /// </summary>
        /// <returns></returns>
        public float GetWindScale() => m_windScale;
        /// <summary>
        /// 获取飘扬时的风向
        /// </summary>
        /// <returns></returns>
        public Vector2 GetWindDirection() => m_windDir;
        #endregion

        #region Public Setter
        /// <summary>
        /// 设置旗帜飘扬特效启用状态【true启用】
        /// </summary>
        /// <param name="enableFlagWaft"></param>
        public void SetEnableFlagWaft(bool enableFlagWaft)
        {
            m_enableFlagWaft = enableFlagWaft;
            if (m_enableFlagWaft)
            {
                SwitchMaterial2FlagWaft();
            }
            else
            {
                SwitchMaterial2Default();
            }
        }
        /// <summary>
        /// 设置飘扬特效的贴图
        /// </summary>
        /// <param name="flagWaftMainTex"></param>
        public void SetFlagWaftMainTex(Texture flagWaftMainTex)
        {
            m_flagWaftMainTex = flagWaftMainTex;
            RefreshFlagWaftMainTex();
        }
        /// <summary>
        /// 设置飘扬时的风速
        /// </summary>
        /// <param name="windSpeed"></param>
        public void SetWindSpeed(float windSpeed)
        {
            m_windSpeed = UnityEngine.Mathf.Max(windSpeed, 0.0f);
            RefreshWindSpeed();
        }
        /// <summary>
        /// 设置飘扬时的风大小
        /// </summary>
        /// <param name="windScale"></param>
        public void SetWindScale(float windScale)
        {
            m_windScale = UnityEngine.Mathf.Max(windScale, 0.0f);
            RefreshWindScale();
        }
        /// <summary>
        /// 设置飘扬时的风向
        /// </summary>
        /// <param name="windDirection"></param>
        public void SetWindDirection(Vector2 windDirection)
        {
            m_windDir = windDirection;
            RefreshWindDirection();
        }
        #endregion


        private void RefreshFlagWaftMainTex()
        {
            if (m_flagWaftMat) m_flagWaftMat.SetTexture(m_flagWaftMainTexID, GetFlagWaftMainTex());
        }
        private void RefreshWindSpeed()
        {
            if (m_flagWaftMat) m_flagWaftMat.SetFloat(m_windSpeedID, GetWindSpeed());
        }
        private void RefreshWindScale()
        {
            if (m_flagWaftMat) m_flagWaftMat.SetFloat(m_windScaleID, GetWindScale());
        }
        private void RefreshWindDirection()
        {
            if (m_flagWaftMat) m_flagWaftMat.SetVector(m_windDirID, GetWindDirection());
        }

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
        private void SwitchMaterial2FlagWaft()
        {
            if (m_img != null)
            {
                m_img.material = m_flagWaftMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_flagWaftMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_flagWaftMat;
            }
        }


        #region Res Mgr
        private void InstancRes()
        {
            DisposeRes();
            m_flagWaftMat = Instantiate(new Material(Shader.Find(m_shaderName)));
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


        }
        private void DisposeRes()
        {
            if (m_flagWaftMat) DestroyImmediate(m_flagWaftMat, true);
            m_flagWaftMat = null;
        }
        #endregion

    }

}
