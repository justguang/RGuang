using System;
using UnityEngine;

namespace RGuang.IEffect
{
    public class WaterTriggerHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask m_waterMask;
        [SerializeField] private GameObject m_splashParticles;

        private EdgeCollider2D m_edgeColl;
        private InteractableWater m_water;

        private void Awake()
        {
            m_edgeColl = GetComponent<EdgeCollider2D>();
            m_water = GetComponent<InteractableWater>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //return;
            //检测目标碰撞在m_waterMask范围内
            if ((m_waterMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
                if (rb != null)
                {
                    //粒子
                    Vector2 localPos = gameObject.transform.localPosition;
                    Vector2 hitObjectPos = collision.transform.position;
                    Bounds hitObjectBounds = collision.bounds;

                    Vector3 spawnPos = Vector3.zero;
                    if (collision.transform.position.y >= m_edgeColl.points[1].y + m_edgeColl.offset.y + localPos.y)
                    {
                        //hit from above
                        spawnPos = hitObjectPos - new Vector2(0.0f, hitObjectBounds.extents.y);
                    }
                    else
                    {
                        //hit from below
                        spawnPos = hitObjectPos + new Vector2(0.0f, hitObjectBounds.extents.y);
                    }

                    Instantiate(m_splashParticles, spawnPos, Quaternion.identity);


                    //clamp splash point to a　Max velocity 
                    int multipler = 1;
                    if (rb.velocity.y < 0)
                    {
                        multipler = -1;
                    }
                    else
                    {
                        multipler = 1;
                    }

                    float vel = rb.velocity.y * m_water.ForceMultiplier;
                    vel = Mathf.Clamp(Mathf.Abs(vel), 0.0f, m_water.MaxForce);
                    vel *= multipler;

                    m_water.Splash(collision, vel);
                }
            }
        }



    }


}


