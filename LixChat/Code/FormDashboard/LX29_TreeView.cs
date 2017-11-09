using System.ComponentModel;
using System.Windows.Forms;

namespace LX29_ChatClient.Dashboard
{
    public class LX29_TreeView : TreeView
    {
        private bool _horizontalScrollbar = true;

        [Category("Appearance")]
        [Description("Whether to enable a horizontal scrollbar")]
        public bool HorizontalScrollbar
        {
            get { return _horizontalScrollbar; }
            set
            {
                _horizontalScrollbar = value;
                if (DesignMode)
                {
                    RecreateHandle();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams ret = base.CreateParams;
                if (!_horizontalScrollbar)
                {
                    ret.Style |= 0x8000; // TVS_NOHSCROLL
                }
                return ret;
            }
        }

        protected override void InitLayout()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);

            base.InitLayout();
            this.MouseWheel += LX29_TreeView_MouseWheel;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void LX29_TreeView_MouseWheel(object sender, MouseEventArgs e)
        {
        }
    }
}