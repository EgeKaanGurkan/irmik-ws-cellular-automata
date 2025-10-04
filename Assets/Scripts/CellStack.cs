using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : MonoBehaviour
{

        public CellLayer layerPrefab;
        public Cell cellPrefab;

        public int columnCount ;
        public int rowCount ;
        public int layerCount;

        public CellLayer[] layers;


        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            InitializeCells();
        }


        /// <summary>
        /// 
        /// </summary>
        public void InitializeCells()
        {
            layers = new CellLayer[layerCount];

            // instantiate layers
            for (int i = 0; i < layerCount; i++)
            {
                CellLayer copy = Instantiate(layerPrefab, transform);
                copy.transform.localPosition = new Vector3(0.0f, i, 0.0f);

                // create cell layer
                copy.Initialize(cellPrefab, rowCount, columnCount);
                layers[i] = copy;
            }

            // center at the world origin
            transform.localPosition = new Vector3(columnCount, layerCount, rowCount) * -0.5f;
        }
}

