using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
public class TextureEditor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image colorPreview;
    private Texture2D texture2D;
    [SerializeField] RawImage rawImage;


    public enum Mode { Picker, Draw, Erase }
    public Mode currentMode = Mode.Picker;
    public Color drawColor = Color.red;
    public Color eraseColor = Color.white;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        texture2D = rawImage.texture as Texture2D;

        if (texture2D == null)
        {
            Debug.LogError("El RawImage no tiene una Texture2D asignada.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        // Convert the local point to texture coordinates
        Rect rect = rectTransform.rect;
        float x = (localCursor.x - rect.x) * texture2D.width / rect.width;
        float y = (localCursor.y - rect.y) * texture2D.height / rect.height;

        // Ensure coordinates are within texture boundaries
        if (x < 0 || x >= texture2D.width || y < 0 || y >= texture2D.height)
            return;

        int pixelX = (int)x;
        int pixelY = (int)y;

        switch (currentMode)
        {
            case Mode.Picker:
                Color pickedColor = texture2D.GetPixel(pixelX, pixelY);
                colorPreview.color = pickedColor;
                Debug.Log($"Color at pixel ({pixelX}, {pixelY}): {pickedColor} - RGB({pickedColor.r * 255}, {pickedColor.g * 255}, {pickedColor.b * 255})");
                break;
            case Mode.Draw:
                texture2D.SetPixel(pixelX, pixelY, colorPreview.color);
                texture2D.Apply();
                break;
            case Mode.Erase:
                texture2D.SetPixel(pixelX, pixelY, Color.clear);
                texture2D.Apply();
                break;
        }
    }

    public void SetModePicker()
    {
        currentMode = Mode.Picker;
    }

    public void SetModeDraw()
    {
        currentMode = Mode.Draw;
    }

    public void SetModeErase()
    {
        currentMode = Mode.Erase;
    }

    public void SetDrawColor(Color color)
    {
        drawColor = color;
    }

    public void SetEraseColor(Color color)
    {
        eraseColor = color;
    }

    public void Save()
    {
        byte[] bytes = texture2D.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var timeStamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        File.WriteAllBytes(dirPath + "image_" + timeStamp + ".png", bytes);
    }
}
