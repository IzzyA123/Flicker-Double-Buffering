using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Flicker
{
    public partial class Flicker : Form
    {
        // ====== TOGGLE THIS FOR PART 2 ======
        // false = noticeable flicker (Part 1 behaviour)
        // true  = reduced flicker using back buffer (Part 2)
        private const bool USE_DOUBLE_BUFFERING = true;

        // Square state
        private Rectangle rect;
        private int x = 0;
        private int y = 200;          // bottom half start (as per brief)
        private int dx = 2;           // movement per step (x direction)
        private int dy = 2;           // movement per step (y direction)

        // Animation timer
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Flicker()
        {
            InitializeComponent();

            // Form setup (matches the typical Canvas file behaviour)
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Width = 400;
            this.Height = 400;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // PART 2: back-buffer drawing (markedly reduces flicker)
            // This is the WinForms equivalent of drawing to a back buffer then presenting.
            this.DoubleBuffered = USE_DOUBLE_BUFFERING;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
                          USE_DOUBLE_BUFFERING);
            this.UpdateStyles();

            // Create the moving square
            rect = new Rectangle(x, y, 50, 50);

            // Timer = infinite loop (Part 1 requirement)
            timer.Interval = 10; // speed
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // ====== PART 1: infinite loop + bouncing ======
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update position
            x += dx;
            y += dy;

            // Bounce off left/right edges at right angles
            if (x <= 0)
            {
                x = 0;
                dx = -dx;
            }
            else if (x + rect.Width >= this.ClientSize.Width)
            {
                x = this.ClientSize.Width - rect.Width;
                dx = -dx;
            }

            // Bounce off top/bottom edges at right angles
            if (y <= 0)
            {
                y = 0;
                dy = -dy;
            }
            else if (y + rect.Height >= this.ClientSize.Height)
            {
                y = this.ClientSize.Height - rect.Height;
                dy = -dy;
            }

            rect.Location = new Point(x, y);

            // Force repaint each iteration (frame)
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // NOTE:
            // If USE_DOUBLE_BUFFERING is false, you will see flicker because the form is
            // repeatedly cleared and re-drawn to the front buffer.

            Graphics g = e.Graphics;

            // Create drawing tools (simple + clear)
            using (Pen blackPen = new Pen(Color.Black))
            using (Brush redBrush = new SolidBrush(Color.Red))
            using (Brush whiteBrush = new SolidBrush(Color.White))
            using (Font myFont = new Font("Helvetica", 9))
            {
                // Clear background (this is what causes visible flicker when not double buffered)
                g.FillRectangle(whiteBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

                // Draw square
                g.DrawRectangle(blackPen, rect);

                // Draw message in centre
                string msg = "Moving rectangle";
                SizeF textSize = g.MeasureString(msg, myFont);
                float cx = (this.ClientSize.Width - textSize.Width) / 2f;
                float cy = (this.ClientSize.Height - textSize.Height) / 2f;
                g.DrawString(msg, myFont, redBrush, cx, cy);
            }

            // Make flicker more noticeable in Part 1 (optional but matches brief feel)
            // When double buffering is ON, this won't look flickery.
            if (!USE_DOUBLE_BUFFERING)
            {
                Thread.Sleep(5);
            }
        }
    }
}
