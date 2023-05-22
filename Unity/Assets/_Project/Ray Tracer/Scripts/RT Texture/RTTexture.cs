using System;
using System.Linq;
using UnityEngine;
using _Project.Ray_Tracer.Scripts.RT_Scene;

namespace _Project.Ray_Tracer.Scripts.RT_Texture
{
    [RequireComponent(typeof(RTMesh))]
    public class RTTexture : MonoBehaviour
    {
        private RTMesh mesh;
        private GameObject uvView;
        private Material uvMaterial;
        private Shader uvShader;
        private Shader vfeShader;

        public enum Mode{Vertex, Face, Edge}
        [SerializeField]
        private Mode viewMode = Mode.Vertex;
        public Mode ViewMode{
            get{return viewMode;}
            set{viewMode = value; UpdateMaterialProperties();}
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
            AddView();
            AddMatShader();
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

        private void AddView()
        {
            uvView = new GameObject(mesh.name + "_UV", typeof(MeshFilter), typeof(MeshRenderer));
            uvView.gameObject.layer = LayerMask.NameToLayer("UVCamera");
            uvView.transform.parent = mesh.transform;
            uvView.GetComponent<MeshFilter>().mesh = mesh.GetComponent<MeshFilter>().mesh;
            // uvView.SetActive(false);
        }

        private void AddMatShader()
        {
            uvShader = Shader.Find("Custom/UvShader");
            uvMaterial = new Material(uvShader);
            uvMaterial.mainTexture = mesh.GetComponent<MeshRenderer>().material.mainTexture;
            uvView.GetComponent<MeshRenderer>().material = uvMaterial;
        }

        private void UpdateMaterialProperties()
        {
            foreach (string mode in System.Enum.GetNames(typeof(Mode))){
                if (mode == viewMode.ToString()){
                    uvMaterial.EnableKeyword(mode.ToUpper());
                } else {
                    uvMaterial.DisableKeyword(mode.ToUpper());
                }
            }
            }
    }
}