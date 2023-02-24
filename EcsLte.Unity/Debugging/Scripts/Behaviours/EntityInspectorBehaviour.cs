using EcsLte;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Scripts.Behaviours
{
    [ExecuteInEditMode]
    public class EntityInspectorBehaviour : MonoBehaviour
    {
        public Color Color1 = Color.cyan;
        public Color Color2 = Color.grey;

        public EcsContext Context { get; set; }
        public Entity Entity { get; set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}