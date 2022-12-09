namespace Anti_Aircraft
{
    public partial class Form1 : Form
    {
        int angle = 0;
        bool alive = true;
        bool inRange = true;
        bool fired = false;
        ThreadStart Fire;
        Thread threadF;
        double x, y;
        public Form1()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            //SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            RotateFormCenter(pictureBox1, 0);
            x = pictureBox1.Location.X;
            y = pictureBox1.Location.Y;
            label1.Visible = false;
            label1.Text = angle.ToString();
            //创建0-200的随机数
            Random random = new Random();
            int randomNum = random.Next(0, 200);
            pictureBox3.Location = new Point(-pictureBox3.Width, randomNum);
            //创建一个使pictureBox3水平移动的子线程
            ThreadStart Move = new ThreadStart(ThreadStartMove);
            Thread thread = new Thread(new ThreadStart(Move));
            thread.Start();
            //创建一个判断胜利条件的线程
            ThreadStart Win = new ThreadStart(ThreadStartWin);
            Thread threadW = new Thread(new ThreadStart(Win));
            threadW.Start();

        }
        void ThreadStartWin()
        {
            while (true)
            {
                if (!alive)
                {
                    MessageBox.Show("You Win!");
                    Application.Exit();
                    break;
                }
                if (!inRange)
                {
                    reset();
                }
            }
        }
        delegate void resetCallBack();
        void reset()
        {
            if (this.InvokeRequired)
            {
                resetCallBack r = new resetCallBack(reset);
                this.Invoke(r);
            }
            else
            {
                angle = 0;
                //复原pictureBox1.Location
                pictureBox1.Location = new Point(130, 287);
                x = pictureBox1.Location.X;
                y = pictureBox1.Location.Y;
                RotateFormCenter(pictureBox1, 0);
                fired = false;
                inRange = true;

            }
        }
        void ThreadStartMove()
        {
            while (alive)
            {
                pharahMove();
                Thread.Sleep(20);
            }
        }
        delegate void pharahMoveCallBack();
        void pharahMove()
        {
            if (this.pictureBox3.InvokeRequired)
            {
                pharahMoveCallBack pM = new pharahMoveCallBack(pharahMove);
                this.Invoke(pM);
            }
            else
            {
                if (pictureBox3.Location.X >= this.Width)
                {
                    pictureBox3.Location = new Point(-pictureBox3.Width, pictureBox3.Location.Y);
                }
                else
                {
                    pictureBox3.Location = new Point(pictureBox3.Location.X + 1, pictureBox3.Location.Y);
                }
            }
        }
        void ThreadStartFire()
        {
            while (alive && inRange)
            {
                hookFly();
                //Thread.Sleep(100);
            }
        }
        delegate void FireCallBack();
        void hookFly()
        {
            if (this.pictureBox1.InvokeRequired)
            {
                FireCallBack f = new FireCallBack(hookFly);
                this.Invoke(f);
            }
            else
            {
                if (((pictureBox1.Location.X + pictureBox1.Width / 2) < pictureBox3.Location.X + pictureBox3.Width) && ((pictureBox1.Location.X + pictureBox1.Width / 2) > pictureBox3.Location.X) && ((pictureBox1.Location.Y + pictureBox1.Height / 2) < pictureBox3.Location.Y + pictureBox3.Height) && ((pictureBox1.Location.Y + pictureBox1.Height / 2) > pictureBox3.Location.Y))
                {
                    alive = false;
                }
                //当pictureBox全部像素位于窗口外时，令inRange为false
                else if ((pictureBox1.Location.X) > this.Width || pictureBox1.Location.Y > this.Height || (pictureBox1.Location.X + pictureBox1.Width) < 0 || (pictureBox1.Location.Y + pictureBox1.Height) < 0)
                {
                    inRange = false;
                }
                else
                {
                    x += Math.Sin((double)angle / 180 * 3.141592653589793);
                    y -= Math.Cos((double)angle / 180 * 3.141592653589793);
                    pictureBox1.Location = new Point(((int)x), ((int)y));
                    RotateFormCenter(pictureBox1, angle);
                }
            }
        }
        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            rotateLeftStart();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (fired)
            {
                rotateRightStop();
            }
            else if (angle <= -90)
            {
                angle = -90;
                rotateLeftStop();
            }
            else
            {
                angle -= 1;
                label1.Text = angle.ToString();
            }
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            rotateLeftStop();
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            rotateRightStart();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (fired)
            {
                rotateRightStop();
            }
            else if (angle >= 90)
            {
                angle = 90;
                rotateRightStop();
            }
            else
            {
                angle += 1;
                label1.Text = angle.ToString();
            }
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            rotateRightStop();
        }

        private void button4_KeyDown(object sender, KeyEventArgs e)
        {
            //当用户按下的是a键时
            if (e.KeyCode == Keys.A)
            {
                rotateLeftStart();
            }
            //当用户按下的是d键时
            if (e.KeyCode == Keys.D)
            {
                rotateRightStart();
            }
            //当用户按下的是w键时
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (!fired)
                {
                    //开启fire进程
                    ThreadStart Fire = new ThreadStart(ThreadStartFire);
                    Thread thread = new Thread(new ThreadStart(Fire));
                    thread.Start();
                    fired = true;
                }
            }
        }

        private void button4_KeyUp(object sender, KeyEventArgs e)
        {
            //当用户松开的是a键时
            if (e.KeyCode == Keys.A)
            {
                rotateLeftStop();
            }
            //当用户松开的是d键时
            if (e.KeyCode == Keys.D)
            {
                rotateRightStop();
            }
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.Focus();
        }
        private void RotateFormCenter(PictureBox pb, float angle)
        {
            Graphics graphics = pb.CreateGraphics();
            //graphics.Clear(pb.BackColor);
            //装入图片
            Bitmap image = new Bitmap(pb.Image);

            //获取当前窗口的中心点
            Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
            PointF center = new PointF(rect.Width / 2, rect.Height / 2);
            float offsetX = 0;
            float offsetY = 0;
            offsetX = center.X - image.Width / 2;
            offsetY = center.Y - image.Height / 2;
            //构造图片显示区域:让图片的中心点与窗口的中心点一致
            RectangleF picRect = new RectangleF(offsetX, offsetY, image.Width, image.Height);
            PointF Pcenter = new PointF(picRect.X + picRect.Width / 2,
                picRect.Y + picRect.Height / 2);
            // 绘图平面以图片的中心点旋转
            graphics.TranslateTransform(Pcenter.X, Pcenter.Y);
            graphics.RotateTransform(angle);
            //恢复绘图平面在水平和垂直方向的平移
            graphics.TranslateTransform(-Pcenter.X, -Pcenter.Y);
            //用#FEFEFE的颜色填充picturebox1
            graphics.Clear(Color.FromArgb(240, 240, 240));
            //绘制图片
            graphics.DrawImage(image, picRect);

        }
        void rotateLeftStart()
        {
            if (!fired)
            {
                timer1.Interval = 50;
                timer1.Enabled = true;
                RotateFormCenter(pictureBox1, angle);
            }
        }
        void rotateLeftStop()
        {
            timer1.Enabled = false;
            RotateFormCenter(pictureBox1, angle);
        }

        void rotateRightStart()
        {
            if (!fired)
            {
                timer2.Interval = 50;
                timer2.Enabled = true;
                RotateFormCenter(pictureBox1, angle);
            }
        }

        void rotateRightStop()
        {
            timer2.Enabled = false;
            RotateFormCenter(pictureBox1, angle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!fired)
            {
                //开启fire进程
                Fire = new ThreadStart(ThreadStartFire);
                threadF = new Thread(new ThreadStart(Fire));
                threadF.Start();
                fired = true;
            }
        }
    }
}

