using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityDepthCamera
{
    [ExecuteInEditMode]
    public class UnityDepthCamera : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Camera colorCamera = null;
        [SerializeField] private Camera depthCamera = null;
        [SerializeField] private RenderTexture colorRender = null;
        [SerializeField] private RenderTexture depthRender = null;

        public Texture2D ColorTexture { private set; get; }
        public Texture2D DepthTexture { private set; get; }

        // Start is called before the first frame update
        void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            StartCoroutine(readRenderTextureCoroutine());
        }

        // Update is called once per frame
        void Update()
        {
            if (colorCamera != null)
            {
                colorCamera.nearClipPlane = mainCamera.nearClipPlane;
                colorCamera.farClipPlane = mainCamera.farClipPlane;
                colorCamera.fieldOfView = mainCamera.fieldOfView;
            }
            if (depthCamera != null)
            {
                depthCamera.nearClipPlane = mainCamera.nearClipPlane;
                depthCamera.farClipPlane = mainCamera.farClipPlane;
                depthCamera.fieldOfView = mainCamera.fieldOfView;
            }
        }

        private IEnumerator readRenderTextureCoroutine()
        {
            ColorTexture = new Texture2D(colorRender.width, colorRender.height, TextureFormat.RGB24, false);
            DepthTexture = new Texture2D(depthRender.width, depthRender.height, TextureFormat.RGB24, false);
            while (true)
            {
                yield return new WaitForEndOfFrame();
                RenderTexture.active = colorRender;
                ColorTexture.ReadPixels(new Rect(0, 0, colorRender.width, colorRender.height), 0, 0, false);
                ColorTexture.Apply();

                RenderTexture.active = depthRender;
                DepthTexture.ReadPixels(new Rect(0, 0, depthRender.width, depthRender.height), 0, 0, false);
                DepthTexture.Apply();

                RenderTexture.active = null;
            }
        }

        private void capturePNG(Texture2D texture, string fileName, bool isInvert = false)
        {
            if (isInvert)
            {
                var data = texture.GetRawTextureData();
                var bufData = new byte[data.Length];
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        var source = (y * texture.width + x) * 3;
                        var target = ((texture.height - y - 1) * texture.width + x) * 3;
                        bufData[target + 0] = data[source + 0];
                        bufData[target + 1] = data[source + 1];
                        bufData[target + 2] = data[source + 2];
                    }
                }
                texture.LoadRawTextureData(bufData);
                texture.Apply();
            }
            var path = Application.dataPath + "/../" + fileName + ".png";
            Debug.Log("save " + fileName + " : " + path);
            File.WriteAllBytes(path, texture.EncodeToPNG());
        }

        [ContextMenu("CapturePNG")]
        private void editorCapturePNG()
        {
            capturePNG(ColorTexture, "color");
            capturePNG(DepthTexture, "depth", true);
        }
    }
}
