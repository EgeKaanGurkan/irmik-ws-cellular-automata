using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class Cell : MonoBehaviour
    {
        public MeshRenderer renderer;

        // Additional custom per-cell attributes
        public int state = 0;
        // ...
        // ...
        // ...


        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            renderer = GetComponent<MeshRenderer>();
            state = 0; // set dead by default
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            renderer.enabled = (state == 1);
        }
}
