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
        if (pictureRenderer == null)
        {
            Debug.LogError("Picture Renderer (UI Image) is not assigned.");
            return;
        }
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

            string savePath = Path.Combine(Application.persistentDataPath, savedFileName);
            File.Copy(path, savePath, true);
            Debug.Log("Saved image to: " + savePath);

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


    private Sprite LoadSavedPicture()
    {
        string savePath = Path.Combine(Application.persistentDataPath, savedFileName);
        Texture2D texture = LoadTextureFromFile(savePath);

        if (texture == null)
            return null;

        picture = texture;
        Sprite sprite = Sprite.Create(
            picture,
            new Rect(0, 0, picture.width, picture.height),
            new Vector2(0.5f, 0.5f)
        );
        if (pictureRenderer == null)
        {
            Debug.LogError("Picture Renderer (UI Image) is not assigned.");
            return sprite;
        }

        pictureRenderer.sprite = sprite;
        pictureRenderer.gameObject.SetActive(true);
        return sprite;
    }


}