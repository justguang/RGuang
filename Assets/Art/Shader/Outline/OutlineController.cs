using System;
using UnityEngine;
using UnityEngine.UI;

namespace RGuang.IEffect
{
    /// <summary>
    /// 描边效果
    /// </summary>
    public class OutlineController : MonoBehaviour
    {
        [Header("描边效果的启用状态"), SerializeField] private bool m_enableOutline = false;
        [Header("描边效果的贴图"), SerializeField] private Texture m_outlineMainTex;
        [Header("描边颜色"), SerializeField] private Color m_outlineColor = Color.black;
        [Header("描边Left大小"), SerializeField, Range(0.0f, 0.10f)] private float m_outlineLeft = 0.0f;
        [Header("描边Right大小"), SerializeField, Range(0.0f, 0.10f)] private float m_outlineRight = 0.0f;
        [Header("描边Top大小"), SerializeField, Range(0.0f, 0.10f)] private float m_outlineTop = 0.0f;
        [Header("描边Bottom大小"), SerializeField, Range(0.0f, 0.10f)] private float m_outlineBottom = 0.0f;

        #region Shader PropertyID
        private int m_outlineMainTexID = Shader.PropertyToID("_MainTex");
        private int m_outlineColorID = Shader.PropertyToID("_OutlineColor");
        private int m_outlineLeftID = Shader.PropertyToID("_OutlineLeft");
        private int m_outlineRightID = Shader.PropertyToID("_OutlineRight");
        private int m_outlineTopID = Shader.PropertyToID("_OutlineTop");
        private int m_outlineBottomID = Shader.PropertyToID("_OutlineBottom");
        #endregion

        #region 影响的目标【按顺序查找】
        private Image m_img;
        private SpriteRenderer m_spriteRender;
        private MeshRenderer m_render;
        #endregion


        [Space(20.0f)]
        //描边材质
        [SerializeField] private Material m_outlineMat;
        //存储目标默认材质
        [SerializeField] private Material m_defaultMat;
        //描边材质用的shader名
        private string m_shaderName = "Shader Graphs/描边";

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
            SetEnableOutline(m_enableOutline);
            SetOutlineColor(m_outlineColor);
            SetOutlineLeft(m_outlineLeft);
            SetOutlineRight(m_outlineRight);
            SetOutlineTop(m_outlineTop);
            SetOutlineBottom(m_outlineBottom);
            SetOutlineMainTex(m_outlineMainTex);
        }
#endif
        #endregion


        #region Public Func
        /// <summary>
        /// 获取描边效果的启用状态【true启用】
        /// </summary>
        /// <returns></returns>
        public bool GetEnableOutline() => m_enableOutline;
        public void SetEnableOutline(bool enableOutline)
        {
            m_enableOutline = enableOutline;
            if (m_enableOutline)
            {
                SwitchMaterial2Outline();
            }
            else
            {
                SwitchMaterial2Default();
            }
        }
        public Color GetOutlineColor() => m_outlineColor;
        public void SetOutlineColor(Color outlineColor)
        {
            m_outlineColor = outlineColor;
            RefreshOutlineColor();
        }
        public float GetOutlineLeft() => m_outlineLeft;
        public void SetOutlineLeft(float outlineLeft)
        {
            m_outlineLeft = UnityEngine.Mathf.Clamp(outlineLeft, 0.0f, 0.10f);
            RefreshOutlineScale();
        }
        public float GetOutlineRight() => m_outlineRight;
        public void SetOutlineRight(float outlineRight)
        {
            m_outlineRight = UnityEngine.Mathf.Clamp(outlineRight, 0.0f, 0.10f);
            RefreshOutlineScale();
        }
        public float GetOutlineTop() => m_outlineTop;
        public void SetOutlineTop(float outlineTop)
        {
            m_outlineTop = UnityEngine.Mathf.Clamp(outlineTop, 0.0f, 0.10f);
            RefreshOutlineScale();
        }
        public float GetOutlineBottom() => m_outlineBottom;
        public void SetOutlineBottom(float outlineBottom)
        {
            m_outlineBottom = UnityEngine.Mathf.Clamp(outlineBottom, 0.0f, 0.10f);
            RefreshOutlineScale();
        }
        public Texture GetOutlineMainTex() => m_outlineMainTex;
        public void SetOutlineMainTex(Texture outlineMainTex)
        {
            m_outlineMainTex = outlineMainTex;
            RefreshOutlineMainTex();
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
        private void SwitchMaterial2Outline()
        {
            if (m_img != null)
            {
                m_img.material = m_outlineMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_outlineMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_outlineMat;
            }
        }

        private void RefreshOutlineScale()
        {
            if (m_outlineMat)
            {
                m_outlineMat.SetFloat(m_outlineLeftID, GetOutlineLeft());
                m_outlineMat.SetFloat(m_outlineRightID, GetOutlineRight());
                m_outlineMat.SetFloat(m_outlineTopID, GetOutlineTop());
                m_outlineMat.SetFloat(m_outlineBottomID, GetOutlineBottom());
            }
        }
        private void RefreshOutlineColor()
        {
            if (m_outlineMat) m_outlineMat.SetColor(m_outlineColorID, GetOutlineColor());
        }
        private void RefreshOutlineMainTex()
        {
            if (m_outlineMat) m_outlineMat.SetTexture(m_outlineMainTexID, GetOutlineMainTex());
        }
        #region Res Mgr
        private void InstanceRes()
        {
            DisposeRes();
            m_outlineMat = Instantiate(new Material(Shader.Find(m_shaderName)));
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

            RefreshOutlineColor();
            RefreshOutlineScale();
        }
        private void DisposeRes()
        {
            if (m_outlineMat) DestroyImmediate(m_outlineMat, true);
            m_outlineMat = null;
        }
        #endregion

    }

}
