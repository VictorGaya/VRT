using System;
using System.Linq;
using UnityEngine;
using _Project.Ray_Tracer.Scripts.RT_Scene;
using UnityEngine.Rendering.ShaderGraph;

namespace _Project.Ray_Tracer.Scripts.RT_Texture
{
    [RequireComponent(typeof(RTMesh))]
    public class RTTexture : MonoBehaviour
    {
        private RTMesh mesh;
        private GameObject uvView;
        // private Material uvMaterial;
        // private Shader uvShader;
        // private Shader vfeShader;

        public enum Mode{Vertex, Face, Edge}
        [SerializeField]
        private Mode viewMode = Mode.Vertex;
        public Mode ViewMode{
            get{return viewMode;}
            set{viewMode = value;}
        }

        [SerializeField]
        private bool showUV = false;
        public bool ShowUV{
            get{return showUV;}
            set{showUV = value;}
        }

        private void Awake()
        {
            mesh = GetComponent<RTMesh>();
            AddUVObject();
            Debug.Log("Add shader");
            AddShader("Custom/UvShader");
        }
        private void Start()
        {
            ViewMode = Mode.Vertex;
            // ShowUV = true;
        }
        private void onEnable()
        {
            uvView.SetActive(true);
        }

        private void onDisable()
        {
            uvView.SetActive(false);
        }

        private void AddUVObject()
        {
            uvView = new GameObject(mesh.name + "_UV", typeof(MeshFilter), typeof(MeshRenderer));
            uvView.gameObject.layer = LayerMask.NameToLayer("UVCamera");
            uvView.transform.parent = mesh.transform;
            uvView.GetComponent<MeshFilter>().mesh = mesh.GetComponent<MeshFilter>().mesh;
            uvView.GetComponent<MeshRenderer>().materials = new Material[0];
            // uvView.SetActive(false);
        }

        private void AddShader(string name)
        {
            var materials = uvView.GetComponent<MeshRenderer>().materials.ToList();
            materials.Add(new Material(Shader.Find(name)));
            uvView.GetComponent<MeshRenderer>().materials = materials.ToArray();
        }

        // private void AddShader(string name)
        // {
        //     uvShader = Shader.Find(name);
        //     uvMaterial = new Material(uvShader);
        //     uvMaterial.mainTexture = mesh.GetComponent<MeshRenderer>().material.mainTexture;
        //     uvView.GetComponent<MeshRenderer>().material = uvMaterial;
        // }

        // private void UpdateViewMode()
        // {
        //     foreach (string mode in System.Enum.GetNames(typeof(Mode))){
        //         if (mode == viewMode.ToString()){
        //             uvMaterial.EnableKeyword(mode.ToUpper());
        //         } else {
        //             uvMaterial.DisableKeyword(mode.ToUpper());
        //         }
        //     }
        // }
    }
}