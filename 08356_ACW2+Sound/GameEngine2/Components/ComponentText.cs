using GameEngine.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentText : IComponent
    {
        private string _text;
        private Vector2 _sheetColRows;

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_TEXT;
            }
        }

        public Texture Font
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                if (BoundString != null)
                {
                    var str = BoundString.Invoke();
                    if (str != _text)
                    {
                        _text = str;
                        CalculateOffsets();
                    }
                }
                return _text;
            }

            set
            {
                _text = value;
                CalculateOffsets();
            }
        }

        public delegate string StringBinder();

        public StringBinder BoundString
        {
            get;
            private set;
        }

        public List<Vector2> CharOffsets;

        /// <summary>
        /// Text Component to display
        /// </summary>
        /// <param name="pText">Text to display</param>
        /// <param name="pFont">Sprite of characters</param>
        public ComponentText(string pText, Texture pFont)
            : this(pText, pFont, new Vector2(16,14))
        {

        }

        /// <summary>
        /// Text Component to display
        /// </summary>
        /// <param name="pText">Text to display</param>
        /// <param name="pFont">Sprite of characters</param>
        /// <param name="charDimensions">Number of collums and rows of charracters on sprite sheet</param>
        public ComponentText(string pText, Texture pFont, Vector2 charDimensions)
        {
            CharOffsets = new List<Vector2>();
            _sheetColRows = charDimensions;
            Text = pText;
            Font = pFont;
        }

        public ComponentText(StringBinder pBind, Texture pFont, Vector2 charDimensions, string pDefault = "")
            : this(pDefault, pFont, charDimensions)
        {
            BoundString = pBind;
        }

        private Vector2 FindTextOffset(char c)
        {
            int i = (int)c;
            i -= 31;

            int y = (int)(i / _sheetColRows.X);
            int x = i % (int)_sheetColRows.X - 1;
            if (x == -1)
            {
                y--;
            }
            return new Vector2(x * (1.0f / _sheetColRows.X), y * (1.0f / _sheetColRows.Y));
        }

        private void CalculateOffsets()
        {
            CharOffsets.Clear();

            for(int i = 0; i < _text.Length; i++)
            {
                CharOffsets.Add(FindTextOffset(_text[i]));
            }
        }

    }
}
