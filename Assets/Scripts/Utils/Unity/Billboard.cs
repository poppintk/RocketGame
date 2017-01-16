using System;
using System.Collections.Generic;
using UnityEngine;

namespace TX
{
    public class Billboard : BaseBehaviour
    {
        private void LateUpdate()
        {
            Vector3 target = Camera.main.transform.position;
            transform.LookAt(target, Vector3.up);
        }
    }
}
