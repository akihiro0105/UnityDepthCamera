using UnityEngine;
using System.Collections;

namespace UnityDepthCamera
{
    [ExecuteInEditMode]
    public class PostProcessDepthGrayscale : MonoBehaviour
    {
        private Material mat;

        void Start()
        {
            mat = new Material(Shader.Find("Custom/DepthGrayscale"));
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, mat);
        }
    }
}
