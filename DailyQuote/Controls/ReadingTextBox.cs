using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DailyQuote.Controls
{
    /// <summary>
    /// A read-only, borderless RichTextBox that grows to fit its text (no internal scroll bars),
    /// supports extra line spacing, and passes mouse-wheel scrolling to its parent panel.
    /// </summary>
    public class ReadingTextBox : RichTextBox
    {
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int EM_SETPARAFORMAT = 0x0447;
        private const uint PFM_LINESPACING = 0x00000100;
        private const byte LineSpacingRuleMultiple = 5; // dyLineSpacing / 20 = line multiple

        private const int EM_REQUESTRESIZE = 0x0441;

        private float _lineSpacing = 1.0f;
        private int _lastWidth = -1;

        public ReadingTextBox()
        {
            ReadOnly = true;
            BorderStyle = BorderStyle.None;
            ScrollBars = RichTextBoxScrollBars.None;
            WordWrap = true;
            DetectUrls = false;
            TabStop = false;
            ShortcutsEnabled = true; // allow Ctrl+C
            Cursor = Cursors.Arrow;
            BackColor = SystemColors.Window;
            ContentsResized += OnContentsResized;
        }

        /// <summary>Line spacing as a multiple of single spacing (1.0 = normal, 1.5 = one and a half).</summary>
        [DefaultValue(1.0f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float LineSpacing
        {
            get => _lineSpacing;
            set
            {
                _lineSpacing = Math.Max(1.0f, value);
                ApplyLineSpacing();
            }
        }

        /// <summary>Empty space (in pixels at 96 DPI) added below the text.</summary>
        [DefaultValue(0)]
        public int BottomGap { get; set; }

        /// <summary>Replaces the text and re-applies paragraph formatting.</summary>
        public void SetText(string text)
        {
            Text = text;
            ApplyLineSpacing();
            Select(0, 0);
        }

        private void OnContentsResized(object? sender, ContentsResizedEventArgs e)
        {
            int gap = (int)Math.Round(BottomGap * DeviceDpi / 96.0);
            Height = e.NewRectangle.Height + gap + 4;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyLineSpacing();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                RequestResize(); // text may have changed while hidden
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            int width = Width;
            base.OnSizeChanged(e);
            if (width != _lastWidth)
            {
                _lastWidth = width;
                RequestResize(); // re-wrap at the new width
            }
        }

        /// <summary>Asks the control to recalculate its height for the current text and width.</summary>
        public void RequestResize()
        {
            if (IsHandleCreated)
            {
                SendMessage(Handle, EM_REQUESTRESIZE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Without scroll bars the box would swallow the wheel; let the parent panel scroll instead.
            if (m.Msg == WM_MOUSEWHEEL && Parent != null && Parent.IsHandleCreated)
            {
                SendMessage(Parent.Handle, m.Msg, m.WParam, m.LParam);
                return;
            }
            base.WndProc(ref m);
        }

        private void ApplyLineSpacing()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            var format = new PARAFORMAT2
            {
                cbSize = Marshal.SizeOf<PARAFORMAT2>(),
                dwMask = PFM_LINESPACING,
                bLineSpacingRule = LineSpacingRuleMultiple,
                dyLineSpacing = (int)Math.Round(_lineSpacing * 20),
                rgxTabs = new int[32]
            };

            int start = SelectionStart, length = SelectionLength;
            SelectAll();
            SendMessage(Handle, EM_SETPARAFORMAT, IntPtr.Zero, ref format);
            Select(start, length);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref PARAFORMAT2 lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
    }
}
