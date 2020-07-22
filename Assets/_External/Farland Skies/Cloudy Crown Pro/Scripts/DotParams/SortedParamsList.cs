using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams
{
    [Serializable]
    public abstract class SortedParamsList<T> where T : DotParam
    {
        public T[] Params = new T[0];
        protected DotParamsList<T> SortedParams;

        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public void Init()
        {
            SortedParams = new DotParamsList<T>(Params.Length);
            foreach (var param in Params) SortedParams[param.Time] = param;
        }

        public void Update()
        {
            if (SortedParams == null)
            {
                SortedParams = new DotParamsList<T>(Params.Length);
            }
            else
            {
                SortedParams.Clear();
            }

            
            foreach (var param in Params) SortedParams[param.Time] = param;
        }
    }
}