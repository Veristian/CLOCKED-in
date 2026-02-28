using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PictureTaker : MonoBehaviour
{
    public Image pictureRenderer;  // UI Image
    public Texture2D picture;      // runtime copy of last picture
    private string savedFileName = "savedImage.png";

    void Start()
    {
        pictureRenderer.sprite = null;
        pictureRenderer.gameObject.SetActive(false);

        // Load previously saved picture on start
        LoadSavedPicture();
    }

    public void GetPicture()
    {
        // Don't attempt to use the camera if it is already open
        if (NativeCamera.IsCameraBusy())
            return;

        // Take a picture with the camera
        TakePicture(512);
    }

    private void TakePicture(int maxSize)
    {
        picture = null;

        NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path == null)
                return;

            // Option 1: Just copy the original file to persistent storage (fastest, no texture issues)
            string savePath = Path.Combine(Application.persistentDataPath, savedFileName);
            File.Copy(path, savePath, true);
            Debug.Log("Saved image to: " + savePath);

            // Load as Texture2D
            Texture2D texture = LoadTextureFromFile(savePath);

            if (texture == null)
            {
                Debug.LogError("Failed to load texture from saved file.");
                return;
            }

            picture = texture;

            // Create Sprite and assign to UI Image
            pictureRenderer.sprite = Sprite.Create(
                picture,
                new Rect(0, 0, picture.width, picture.height),
                new Vector2(0.5f, 0.5f)
            );

            pictureRenderer.gameObject.SetActive(true);

        }, maxSize);
    }

    /// <summary>
    /// Copies a non-readable texture to a new readable Texture2D using RenderTexture (Android-safe)
    /// </summary>
    private Texture2D CopyTexture(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTexture = new Texture2D(
            source.width,
            source.height,
            TextureFormat.RGBA32,
            false
        );

        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTexture;
    }

    /// <summary>
    /// Load a PNG from persistent storage and return a Texture2D
    /// </summary>
    private Texture2D LoadTextureFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("File not found: " + filePath);
            return null;
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (!texture.LoadImage(imageBytes))
        {
            Debug.LogError("Failed to load image bytes into texture.");
            return null;
        }

        return texture;
    }

    /// <summary>
    /// Load previously saved picture (if exists) on app start
    /// </summary>
    private void LoadSavedPicture()
    {
        string savePath = Path.Combine(Application.persistentDataPath, savedFileName);
        Texture2D texture = LoadTextureFromFile(savePath);

        if (texture == null)
            return;

        picture = texture;
        pictureRenderer.sprite = Sprite.Create(
            picture,
            new Rect(0, 0, picture.width, picture.height),
            new Vector2(0.5f, 0.5f)
        );

        pictureRenderer.gameObject.SetActive(true);
    }

    private void TakePictureAndShow(int maxSize)
    {
        NativeCamera.TakePicture((path) =>
        {
            if (path == null) return;

            Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
            if (texture == null) return;

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
            quad.transform.forward = Camera.main.transform.forward;
            quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

            Renderer renderer = quad.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Unlit/Texture"));
            renderer.material = mat;
            mat.mainTexture = texture;

            Destroy(quad, 5f);
            Destroy(texture, 5f);
        }, maxSize);
    }

    private void RecordVideo()
    {
        NativeCamera.RecordVideo((path) =>
        {
            if (path != null)
            {
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        });
    }
}