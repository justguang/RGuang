using UnityEngine;
using System.Collections.Generic;
using RGuang.Attribute;

namespace CardGame
{

    public class LogoController : MonoBehaviour
    {
        [SerializeField] private Animator _ani;

        private void Start()
        {
            _ani = transform.GetComponent<Animator>();

        }

        public void OnLogoCompleted()
        {
            _ani.enabled = false;
        }


    }

}


