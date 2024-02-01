using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    class Model
    {
        private string modelName;

        public virtual void Init(string name) 
        {
            modelName = name;
        }

        public virtual void UnInit() 
        {
        }

        public string GetModelName() {  return modelName; }
    }

}

