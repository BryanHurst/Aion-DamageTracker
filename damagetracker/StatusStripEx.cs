using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DamageTracker
{
    public class StatusStripEx : StatusStrip
    {
        private bool clickThrough = true;

        public StatusStripEx()
        {
            this.RenderMode = ToolStripRenderMode.Professional;
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
        }

        /// <summary>
        /// Enables click-through on buttons when the form is not activated
        /// </summary>
        public bool ClickThrough 
        {
            get { return this.clickThrough; }
            set { this.clickThrough = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.clickThrough && m.Msg == WinConst.WM_MOUSEACTIVATE && m.Result == (IntPtr)WinConst.MA_ACTIVATEANDEAT)
                m.Result = (IntPtr)WinConst.MA_ACTIVATE;
        }
    }

    public class WinConst
    {
        public const uint WM_MOUSEACTIVATE = 0x21;
        public const uint MA_ACTIVATE = 1;
        public const uint MA_ACTIVATEANDEAT = 2;
        public const uint MA_NOACTIVATE = 3;
        public const uint MA_NOACTIVATEANDEAT = 4;
    }
}
