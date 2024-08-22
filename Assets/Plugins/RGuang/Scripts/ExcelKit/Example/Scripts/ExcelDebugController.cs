using System;
using UnityEngine;
using UnityEngine.UI;

namespace RGuang.Kit.Example
{
    public class ExcelDebugController : MonoBehaviour
    {
        [SerializeField] private Text m_txt;
        [SerializeField] private ConfigItems m_ConfigSO;
        void Start()
        {
            m_txt = transform.GetComponent<Text>();
            if (!m_txt) return;

            if (m_ConfigSO != null)
            {
                if (m_ConfigSO.ItemLst_1 != null)
                {
                    m_ConfigSO.ItemLst_1.ForEach(i => m_txt.text = m_txt.text + i.ToString());
                }

                if (m_ConfigSO.ItemLst_2 != null)
                {
                    m_txt.text = m_txt.text + "\n------------------------------------------------------\n";
                    m_ConfigSO.ItemLst_2.ForEach(i => m_txt.text = m_txt.text + i.ToString());
                }
            }

        }

    }

}
