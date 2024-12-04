using UnityEngine;

namespace Sokoban
{
    public class UIFontManager
    {
        private static Font defaultFont;

        public static Font DefaultFont
        {
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return defaultFont;
            }
        }
    }
} 