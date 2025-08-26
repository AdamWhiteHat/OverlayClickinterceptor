using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OverlayClickinterceptor.Win32;
using Gma.System.MouseKeyHook;
using System.Windows.Forms.VisualStyles;

namespace OverlayClickinterceptor.Subordinant
{
    public enum ResizeOrigin
    {
        None = 0,
        NW, N, NE,
        W, /**/ E,
        SW, S, SE
    }

    public partial class OverlayMouseInfo : Form
    {
        /// <summary>
        /// Fired when a mouse click occurs within an overlay
        /// </summary>
        public EventHandler<MouseEventArgs> OverlayClicked;

        [Browsable(true)]
        [Category(nameof(CategoryAttribute.Appearance))]
        [DefaultValue(HatchStyle.Percent10)]
        public HatchStyle HatchingStyle
        {
            get { return _hatchingStyle; }
            set
            {
                if (_hatchingStyle != value)
                {
                    _hatchingStyle = value;

                    _thatchedBrush = null;

                    OnStyleChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }
        private HatchStyle _hatchingStyle;

        #region Private and Protected Members

        protected Brush ThatchedBrush
        {
            get
            {
                if (_thatchedBrush == null)
                {
                    _thatchedBrush = new HatchBrush(HatchingStyle, this.ForeColor, this.TransparencyKey);
                }
                return _thatchedBrush;
            }
            private set
            {
                if (_thatchedBrush != null)
                {
                    _thatchedBrush.Dispose();
                }
                _thatchedBrush = value;
            }
        }
        private Brush _thatchedBrush = null;

        private Point lastMouseClickLocation = Point.Empty;

        private bool isDragging = false;
        private Point draggingStartLocation = Point.Empty;

        private bool isResizing = false;
        private Point resizingStartLocation = Point.Empty;
        private ResizeOrigin resizingOrigin = ResizeOrigin.None;

        private static int ResizeBorderSize = 12;
        private static Rectangle TextInvalidateRegion = new Rectangle(ResizeBorderSize,ResizeBorderSize,110 + ResizeBorderSize, 75 + ResizeBorderSize);
        private static string TextFormat_MouseInformation = "{0}, {1}\nClick.X: {2}\nClick.Y: {3}";

        #endregion

        public OverlayMouseInfo()
        {
            InitializeComponent();

            this.MinimumSize = new Size(TextInvalidateRegion.Size.Width, TextInvalidateRegion.Size.Height);

            //this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.Opaque, false);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);

            this.TransparencyKey = Color.FromArgb(0, 0, 255);
            this.BackColor = this.TransparencyKey;
            this.AllowTransparency = true;

            HatchingStyle = HatchStyle.Percent10;

            GlobalHooks.GlobalHookEvents.MouseDownExt += OverlayMouseInfo_MouseDown;
            GlobalHooks.GlobalHookEvents.MouseMoveExt += OverlayMouseInfo_MouseMove;
            GlobalHooks.GlobalHookEvents.MouseUpExt += OverlayMouseInfo_MouseUp;

            this.FormClosing += OverlayMouseInfo_FormClosing;
        }

        private void OverlayMouseInfo_FormClosing(object? sender, FormClosingEventArgs e)
        {
            GlobalHooks.GlobalHookEvents.MouseDownExt -= OverlayMouseInfo_MouseDown;
            GlobalHooks.GlobalHookEvents.MouseMoveExt -= OverlayMouseInfo_MouseMove;
            GlobalHooks.GlobalHookEvents.MouseUpExt -= OverlayMouseInfo_MouseUp;
            ThatchedBrush = null;
        }

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WindowStyles.WS_EX_TRANSPARENT;
                cp.ExStyle |= (int)WindowStyles.WS_EX_TOPMOST;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.ClipRectangle;

            if (this.Visible)
            {
                g.Clear(this.TransparencyKey);
                g.FillRectangle(ThatchedBrush, bounds);

                ControlPaint.DrawBorder(g, this.ClientRectangle,
                    Color.LightGray, ResizeBorderSize, ButtonBorderStyle.Dotted,
                    Color.LightGray, ResizeBorderSize, ButtonBorderStyle.Dotted,
                    Color.LightGray, ResizeBorderSize, ButtonBorderStyle.Dotted,
                    Color.LightGray, ResizeBorderSize, ButtonBorderStyle.Dotted);

                if (!string.IsNullOrWhiteSpace(this.Text))
                {
                    // Here we decide to render our own text instead of placing it in a Label control, as the label control will not have the thatching.
                    TextRenderer.DrawText(g, this.Text, this.Font, new Point(ResizeBorderSize + 1, ResizeBorderSize + 1), this.ForeColor);
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region Mouse Events

        protected void RaiseOverlayClicked(MouseEventArgs args)
        {
            OverlayClicked?.Invoke(this, args);
        }

        private void OverlayMouseInfo_MouseDown(object sender, MouseEventExtArgs e)
        {
            if (WindowState != FormWindowState.Normal)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                lastMouseClickLocation = e.Location;

                if (this.Bounds.Contains(e.Location))
                {
                    if (isDragging || isResizing)
                    {
                        return;
                    }

                    Point clientPoint = this.PointToClient(e.Location);

                    resizingOrigin = ResizeBorderHitTest(clientPoint);
                    if (resizingOrigin != ResizeOrigin.None)
                    {
                        isResizing = true;
                        resizingStartLocation = e.Location;
                    }
                    else
                    {
                        isDragging = true;
                        draggingStartLocation = e.Location;
                    }

                    e.Handled = true;

                    RaiseOverlayClicked(new MouseEventArgs(e.Button, e.Clicks, e.Location.X, e.Location.Y, e.Delta));
                }

                UpdateControlText(e.Location);
            }
        }

        private void OverlayMouseInfo_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (isDragging)
            {
                Size delta = CalculatePointDelta(e.Location, draggingStartLocation);
                draggingStartLocation = e.Location;

                Point newLocation = new Point(
                    (this.Location.X + delta.Width),
                    (this.Location.Y + delta.Height)
                );

                this.Location = newLocation;
                this.Update();
            }
            else if (isResizing)
            {
                Size delta = CalculatePointDelta(e.Location, resizingStartLocation);
                resizingStartLocation = e.Location;

                this.Bounds = CalculateNewResizedBounds(this.Bounds, resizingOrigin, delta);
            }

            UpdateControlText(e.Location);
        }

        private void OverlayMouseInfo_MouseUp(object sender, MouseEventExtArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                draggingStartLocation = Point.Empty;
            }
            else if (isResizing)
            {
                isResizing = false;
                resizingStartLocation = Point.Empty;
                resizingOrigin = ResizeOrigin.None;
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateControlText(Point mouseLocation)
        {
            this.Text = string.Format(TextFormat_MouseInformation, mouseLocation.X, mouseLocation.Y, lastMouseClickLocation.X, lastMouseClickLocation.Y);
            this.Invalidate(TextInvalidateRegion);
        }

        private Size CalculatePointDelta(Point from, Point to)
        {
            return new Size(
                (from.X - to.X),
                (from.Y - to.Y)
            );
        }

        private static Rectangle Shrink(Rectangle rectangle, int amount = 1) => new Rectangle(rectangle.X + amount, rectangle.Y + amount, rectangle.Width - (amount * 2), rectangle.Height - (amount * 2));

        #endregion

        #region Sizable Form Methods

        private ResizeOrigin ResizeBorderHitTest(Point clientPoint)
        {
            Rectangle innerClient = Shrink(this.ClientRectangle, ResizeBorderSize);

            VerticalAlignment vert = VerticalAlignment.Center;
            HorizontalAlignment horiz = HorizontalAlignment.Center;

            if (clientPoint.Y <= innerClient.Top) vert = VerticalAlignment.Top;
            else if (clientPoint.Y >= innerClient.Bottom) vert = VerticalAlignment.Bottom;

            if (clientPoint.X <= innerClient.Left) horiz = HorizontalAlignment.Left;
            else if (clientPoint.X >= innerClient.Right) horiz = HorizontalAlignment.Right;

            // Now convert vert and horiz into ResizeOrigin
            if (vert != VerticalAlignment.Center && horiz != HorizontalAlignment.Center)
            {
                if (vert == VerticalAlignment.Top)
                {
                    if (horiz == HorizontalAlignment.Left) return ResizeOrigin.NW;
                    else if (horiz == HorizontalAlignment.Right) return ResizeOrigin.NE;
                }
                else if (vert == VerticalAlignment.Bottom)
                {
                    if (horiz == HorizontalAlignment.Left) return ResizeOrigin.SW;
                    else if (horiz == HorizontalAlignment.Right) return ResizeOrigin.SE;
                }
            }
            else if (vert != VerticalAlignment.Center)
            {
                if (vert == VerticalAlignment.Top) return ResizeOrigin.N;
                else if (vert == VerticalAlignment.Bottom) return ResizeOrigin.S;
            }
            else if (horiz != HorizontalAlignment.Center)
            {
                if (horiz == HorizontalAlignment.Left) return ResizeOrigin.W;
                else if (horiz == HorizontalAlignment.Right) return ResizeOrigin.E;
            }
            return ResizeOrigin.None;
        }

        private Rectangle CalculateNewResizedBounds(Rectangle bound, ResizeOrigin origin, Size delta)
        {
            Rectangle result = new Rectangle(bound.Location, bound.Size);

            switch (origin)
            {
                case ResizeOrigin.N:
                    result.Y += delta.Height;
                    result.Height += (-delta.Height);
                    break;
                case ResizeOrigin.S:
                    result.Height += delta.Height;
                    break;
                case ResizeOrigin.W:
                    result.X += delta.Width;
                    result.Width += (-delta.Width);
                    break;
                case ResizeOrigin.E:
                    result.Width += delta.Width;
                    break;
                case ResizeOrigin.NW:
                    result.Y += delta.Height;
                    result.Height += (-delta.Height);
                    result.X += delta.Width;
                    result.Width += (-delta.Width);
                    break;
                case ResizeOrigin.NE:
                    result.Y += delta.Height;
                    result.Height += (-delta.Height);
                    result.Width += delta.Width;
                    break;
                case ResizeOrigin.SW:
                    result.Height += delta.Height;
                    result.X += delta.Width;
                    result.Width += (-delta.Width);
                    break;
                case ResizeOrigin.SE:
                    result.Height += delta.Height;
                    result.Width += delta.Width;
                    break;
            }

            return result;
        }

        #endregion

    }
}
