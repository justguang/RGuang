using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RGuang.IEffect
{
    /// <summary>
    /// 像素化特效
    /// Shader = [Shader Graphs/像素化]
    /// </summary>
    public class PixelController : MonoBehaviour
    {
        [Header("像素化启用状态"), SerializeField] private bool m_enablePixel = false;
        [Header("像素化效果贴图"), SerializeField] private Texture m_pixelMainTex;
        [Header("像素化过程时长[秒]"), SerializeField] private float m_duration = 1.0f;
        [Header("像素化时的行数"), SerializeField] private int m_pixelRows = 50;
        [Header("像素化时的列数"), SerializeField] private int m_pixelColumns = 50;


        #region Shader PropertyID
        private int m_pixelMainTexID = Shader.PropertyToID("_MainTex");
        //像素化时行数
        private int m_pixelAmountRowsID = Shader.PropertyToID("_PixelAmountRows");
        //像素化时列数
        private int m_pixelAmountColumnsID = Shader.PropertyToID("_PixelAmountColumns");
        #endregion

        #region 影响的目标【按顺序查找】
        //影响的目标
        private Image m_img;
        private SpriteRenderer m_spriteRender;
        private MeshRenderer m_render;
        #endregion

        [Space(20.0f)]
        //目标像素化材质
        [SerializeField] private Material m_pixelMat;
        //目标默认材质
        [SerializeField] private Material m_defaultMat;
        //像素化shader名
        private string m_shaderName = "Shader Graphs/像素化";

        #region Unity Fun
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
            SetEnablePixel(m_enablePixel);
            SetDuration(m_duration);
            SetPixelRows(m_pixelRows);
            SetPixelColumns(m_pixelColumns);
            SetPixelMainTex(m_pixelMainTex);
        }
        //private void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.A))
        //    {
        //        Debug.Log($"[{this.gameObject}] pixelController Test => update:【KeyCode.A】");
        //        SetEnablePixel(!m_enablePixel);
        //    }
        //    else if (Input.GetKeyUp(KeyCode.S))
        //    {
        //        Debug.Log($"[{this.gameObject}] pixelController Test => update:【KeyCode.S】");
        //        PlayPixel(null);
        //    }
        //    else if (Input.GetKeyUp(KeyCode.D))
        //    {
        //        Debug.Log($"[{this.gameObject}] pixelController Test => update:【KeyCode.D】");
        //        PlayPixel(() => SetEnablePixel(false));
        //    }

        //}
#endif
        #endregion

        #region Public Func
        /// <summary>
        /// 播放动画像素化
        /// </summary>
        /// <param name="onFinishCallback">播放动画完毕后回调</param>
        public void PlayPixel(Action onFinishCallback)
        {
            int endRows = GetPixelRows();
            int endColumns = GetPixelColumns();
            SetPixelRows(2000);
            SetPixelColumns(2000);

            SetEnablePixel(true);

            DOTween.To(GetPixelRows, SetPixelRows, endRows, GetDuartion() - 0.1f).SetEase(Ease.InOutQuad).OnUpdate(RefreshPixelRows);
            DOTween.To(GetPixelColumns, SetPixelColumns, endColumns, GetDuartion()).SetEase(Ease.InOutQuad).OnUpdate(RefreshPixelColumns).OnComplete(() => onFinishCallback?.Invoke());

        }

        /// <summary>
        /// 设置像素化启用状态【true启用，false禁用】
        /// </summary>
        /// <param name="newEnable"></param>
        public void SetEnablePixel(bool newEnable)
        {
            m_enablePixel = newEnable;
            if (m_enablePixel)
            {
                SwitchMaterial2Pixel();
            }
            else
            {
                SwitchMaterial2Default();
            }
        }
        /// <summary>
        /// 设置像素化时长【秒】
        /// </summary>
        /// <param name="newDuration"></param>
        public void SetDuration(float newDuration)
        {
            m_duration = UnityEngine.Mathf.Max(newDuration, 0.0f);
        }
        /// <summary>
        /// 设置像素行数
        /// </summary>
        /// <param name="newRows"></param>
        public void SetPixelRows(int newRows)
        {
            m_pixelRows = UnityEngine.Mathf.Max(newRows, 1);
            RefreshPixelRows();
        }
        /// <summary>
        /// 设置像素列数
        /// </summary>
        /// <param name="newColumns"></param>
        public void SetPixelColumns(int newColumns)
        {
            m_pixelColumns = UnityEngine.Mathf.Max(newColumns, 1);
            RefreshPixelColumns();
        }
        /// <summary>
        /// 设置像素化特效的贴图
        /// </summary>
        /// <param name="pixelMainTex"></param>
        public void SetPixelMainTex(Texture pixelMainTex)
        {
            m_pixelMainTex = pixelMainTex;
            RefreshPixelMainTex();
        }

        /// <summary>
        /// 获取像素化启用状态
        /// </summary>
        /// <returns></returns>
        public bool GetEnablePixel() => m_enablePixel;
        /// <summary>
        /// 获取像素化动画时长【秒】
        /// </summary>
        /// <returns></returns>
        public float GetDuartion() => m_duration;
        /// <summary>
        /// 获取像素行数
        /// </summary>
        /// <returns></returns>
        public int GetPixelRows() => m_pixelRows;
        /// <summary>
        /// 获取像素列数
        /// </summary>
        /// <returns></returns>
        public int GetPixelColumns() => m_pixelColumns;
        /// <summary>
        /// 获取像素化特效的贴图
        /// </summary>
        /// <returns></returns>
        public Texture GetPixelMainTex() => m_pixelMainTex;
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
        private void SwitchMaterial2Pixel()
        {
            if (m_img != null)
            {
                m_img.material = m_pixelMat;
            }
            else if (m_spriteRender != null)
            {
                m_spriteRender.material = m_pixelMat;
            }
            else if (m_render != null)
            {
                m_render.material = m_pixelMat;
            }
        }


        private void RefreshPixelRows()
        {
            if (m_pixelMat) m_pixelMat.SetFloat(m_pixelAmountRowsID, GetPixelRows());
        }
        private void RefreshPixelColumns()
        {
            if (m_pixelMat) m_pixelMat.SetFloat(m_pixelAmountColumnsID, GetPixelColumns());
        }
        private void RefreshPixelMainTex()
        {
            if (m_pixelMat) m_pixelMat.SetTexture(m_pixelMainTexID, GetPixelMainTex());
        }

        #region Res Mgr
        private void InstancRes()
        {
            DisposeRes();
            m_pixelMat = Instantiate(new Material(Shader.Find(m_shaderName)));
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


            RefreshPixelRows();
            RefreshPixelColumns();
            RefreshPixelMainTex();
        }
        private void DisposeRes()
        {
            if (m_pixelMat) DestroyImmediate(m_pixelMat, true);
            m_pixelMat = null;
        }
        #endregion

    }

}
