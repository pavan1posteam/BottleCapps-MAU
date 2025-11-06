using System;
using System.Drawing;               // ✅ Needed for Font, Brush, Color, PointF
using System.Windows.Forms;          // ✅ Needed for TextBox, PaintEventArgs

namespace Global_MAU.Controls
{
    public class PlaceholderTextBox : TextBox
    {
        private string _placeholder = "";
        private bool _showingPlaceholder = true;

        /// <summary>
        /// The placeholder text to display when the TextBox is empty.
        /// </summary>
        public string Placeholder
        {
            get { return _placeholder; }
            set { _placeholder = value; Invalidate(); }
        }

        public PlaceholderTextBox()
        {
            // Allow custom drawing
            this.SetStyle(ControlStyles.UserPaint, true);

            // Track when text changes
            this.TextChanged += (s, e) =>
            {
                _showingPlaceholder = string.IsNullOrEmpty(this.Text);
                Invalidate();
            };

            // Redraw when resized
            this.Resize += (s, e) => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_showingPlaceholder && !string.IsNullOrEmpty(_placeholder))
            {
                // Start with the control's current font size
                float fontSize = this.Font.Size;
                SizeF textSize;

                // Reduce font size until the placeholder fits within the textbox width
                do
                {
                    using (Font testFont = new Font(this.Font.FontFamily, fontSize, FontStyle.Italic))
                    {
                        textSize = e.Graphics.MeasureString(_placeholder, testFont);
                        if (textSize.Width > this.ClientSize.Width - 4) // 4px padding
                            fontSize -= 0.5f;
                        else
                            break;
                    }
                } while (fontSize > 6); // Minimum readable size

                // Draw placeholder text
                using (Font placeholderFont = new Font(this.Font.FontFamily, fontSize, FontStyle.Italic))
                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    e.Graphics.DrawString(
                        _placeholder,
                        placeholderFont,
                        brush,
                        new PointF(2, (this.Height - placeholderFont.Height) / 2) // Vertically centered
                    );
                }
            }
            else
            {
                // Draw actual text immediately so it appears while typing
                TextRenderer.DrawText(
                    e.Graphics,
                    this.Text,
                    this.Font,
                    this.ClientRectangle,
                    this.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left
                );
            }
        }
    }
}
