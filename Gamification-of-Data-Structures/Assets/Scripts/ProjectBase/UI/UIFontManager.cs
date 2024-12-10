using UnityEngine;

public class UIFontManager : BaseManager<UIFontManager>
{
    private Font font;

    public Font GetFont()
    {
        if (font == null)
        {
            font = Resources.Load<Font>("Font/DefaultFont");
            
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (font == null)
                {
                    Debug.LogError("Failed to load both custom and built-in fonts!");
                }
            }
        }
        return font;
    }
} 