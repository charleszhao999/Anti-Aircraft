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
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true); // ��ֹ��������.
            //SetStyle(ControlStyles.DoubleBuffer, true); // ˫����
            RotateFormCenter(pictureBox1, 0);
            x = pictureBox1.Location.X;
            y = pictureBox1.Location.Y;
            label1.Visible = false;
            label1.Text = angle.ToString();
            //����0-200�������
            Random random = new Random();
            int randomNum = random.Next(0, 200);
            pictureBox3.Location = new Point(-pictureBox3.Width, randomNum);
            //����һ��ʹpictureBox3ˮƽ�ƶ������߳�
            ThreadStart Move = new ThreadStart(ThreadStartMove);
            Thread thread = new Thread(new ThreadStart(Move));
            thread.Start();
            //����һ���ж�ʤ���������߳�
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
                //��ԭpictureBox1.Location
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
                //��pictureBoxȫ������λ�ڴ�����ʱ����inRangeΪfalse
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
            //���û����µ���a��ʱ
            if (e.KeyCode == Keys.A)
            {
                rotateLeftStart();
            }
            //���û����µ���d��ʱ
            if (e.KeyCode == Keys.D)
            {
                rotateRightStart();
            }
            //���û����µ���w��ʱ
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (!fired)
                {
                    //����fire����
                    ThreadStart Fire = new ThreadStart(ThreadStartFire);
                    Thread thread = new Thread(new ThreadStart(Fire));
                    thread.Start();
                    fired = true;
                }
            }
        }

        private void button4_KeyUp(object sender, KeyEventArgs e)
        {
            //���û��ɿ�����a��ʱ
            if (e.KeyCode == Keys.A)
            {
                rotateLeftStop();
            }
            //���û��ɿ�����d��ʱ
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
            //װ��ͼƬ
            Bitmap image = new Bitmap(pb.Image);

            //��ȡ��ǰ���ڵ����ĵ�
            Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
            PointF center = new PointF(rect.Width / 2, rect.Height / 2);
            float offsetX = 0;
            float offsetY = 0;
            offsetX = center.X - image.Width / 2;
            offsetY = center.Y - image.Height / 2;
            //����ͼƬ��ʾ����:��ͼƬ�����ĵ��봰�ڵ����ĵ�һ��
            RectangleF picRect = new RectangleF(offsetX, offsetY, image.Width, image.Height);
            PointF Pcenter = new PointF(picRect.X + picRect.Width / 2,
                picRect.Y + picRect.Height / 2);
            // ��ͼƽ����ͼƬ�����ĵ���ת
            graphics.TranslateTransform(Pcenter.X, Pcenter.Y);
            graphics.RotateTransform(angle);
            //�ָ���ͼƽ����ˮƽ�ʹ�ֱ�����ƽ��
            graphics.TranslateTransform(-Pcenter.X, -Pcenter.Y);
            //��#FEFEFE����ɫ���picturebox1
            graphics.Clear(Color.FromArgb(240, 240, 240));
            //����ͼƬ
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
                //����fire����
                Fire = new ThreadStart(ThreadStartFire);
                threadF = new Thread(new ThreadStart(Fire));
                threadF.Start();
                fired = true;
            }
        }
    }
}

