using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Input;

namespace PAK_Editor
{
    public class WinMain : Form
    {
        private ToolStrip toolBox;
        private ToolStripDropDownButton file;
        private Panel PictureView;
        private Panel RectangleDataView;
        private TextBox RectangleDataBox;
        private Image SelectedSprite;
        private int SelectedRect = 0;
        private int SelectedParentNode = 0;
        private Rectangle[] _sprRectangles;

        private TreeView SpriteList;

        private struct Sprite
        {
            public int Index;
            public byte[] ImageBytes;
            public List<SpriteRectangles> spriteRects;
        };

        private struct SpriteRectangles
        {
            public int Index;
            public Rectangle spriteRect;
        };

        private List<Sprite> SpriteArray;

        public WinMain()
        {
            InitWindowParameters();
            InitFormComponents();
        }

        private void InitWindowParameters()
        {
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.SizeChanged += WinMain_SizeChanged;
            this.Text = "PAK Editor";
        }

        private void WinMain_SizeChanged(object sender, EventArgs e)
        {
            PictureView.Width = this.Width - 266;
            PictureView.Refresh();
        }

        private void InitFormComponents()
        {
            SpriteArray = new List<Sprite>();

            PictureView = new Panel();
            {
                PictureView.BackColor = Color.FromArgb(240, 240, 240);
                PictureView.Dock = DockStyle.Left;
                PictureView.BorderStyle = BorderStyle.FixedSingle;
                PictureView.Width = this.Width - 266;
                PictureView.Paint += PictureView_Paint;
                PictureView.MouseClick += PictureView_MouseClick;
            }
            this.Controls.Add(PictureView);

            RectangleDataView = new Panel();
            {
                RectangleDataView.BackColor = Color.FromArgb(225, 225, 225);
                RectangleDataView.Dock = DockStyle.Bottom;
                RectangleDataView.Height = 150;
                RectangleDataView.Paint += RectangleDataView_Paint;
                RectangleDataView.Name = "RectangleDataView";
            }
            this.Controls.Add(RectangleDataView);

            RectangleDataBox = new TextBox();
            {
                RectangleDataBox.Width = 100;
                RectangleDataBox.Location = new Point(75, 25);
                RectangleDataBox.Enabled = false;
                RectangleDataBox.Name = "txtBox_X";
                RectangleDataBox.KeyPress += RectangleDataBox_KeyPress;
            }
            RectangleDataView.Controls.Add(RectangleDataBox);

            RectangleDataBox = new TextBox();
            {
                RectangleDataBox.Width = 100;
                RectangleDataBox.Location = new Point(75, 75);
                RectangleDataBox.Enabled = false;
                RectangleDataBox.Name = "txtBox_Y";
                RectangleDataBox.KeyPress += RectangleDataBox_KeyPress;
            }
            RectangleDataView.Controls.Add(RectangleDataBox);

            RectangleDataBox = new TextBox();
            {
                RectangleDataBox.Width = 100;
                RectangleDataBox.Location = new Point(300, 25);
                RectangleDataBox.Enabled = false;
                RectangleDataBox.Name = "txtBox_W";
                RectangleDataBox.KeyPress += RectangleDataBox_KeyPress;
            }
            RectangleDataView.Controls.Add(RectangleDataBox);

            RectangleDataBox = new TextBox();
            {
                RectangleDataBox.Width = 100;
                RectangleDataBox.Location = new Point(300, 75);
                RectangleDataBox.Enabled = false;
                RectangleDataBox.Name = "txtBox_H";
                RectangleDataBox.KeyPress += RectangleDataBox_KeyPress;
            }
            RectangleDataView.Controls.Add(RectangleDataBox);

            SpriteList = new TreeView();
            {
                SpriteList.BackColor = Color.FromArgb(225, 225, 225);
                SpriteList.Dock = DockStyle.Left;
                SpriteList.Width = 250;
                SpriteList.AfterSelect += SpriteList_AfterSelect;
                SpriteList.NodeMouseClick += SpriteList_NodeMouseClick;
                SpriteList.Scrollable = true;
            }
            this.Controls.Add(SpriteList);

            toolBox = new ToolStrip();
            {
                toolBox.GripStyle = ToolStripGripStyle.Hidden;
                toolBox.BackColor = Color.FromArgb(230, 230, 230);
                toolBox.Dock = DockStyle.Top;

                file = new ToolStripDropDownButton("File");
                file.DropDownItems.Add("New", null, fileNew_Click);
                file.DropDownItems.Add("Open", null, fileOpen_Click);
                file.DropDownItems.Add(new ToolStripSeparator());
                ToolStripItem add = new ToolStripMenuItem();
                add.Enabled = false;
                add.Text = "Add Sprite";
                add.Click += Add_Click;
                file.DropDownItems.Add(add);

                ToolStripItem Save = new ToolStripMenuItem();
                Save.Enabled = false;
                Save.Text = "Save As";
                Save.Click += fileSaveAs_Click;
                file.DropDownItems.Add(Save);
                file.DropDownItems.Add(new ToolStripSeparator());
                file.DropDownItems.Add("Exit", null, fileExit_Click);

                toolBox.Items.Add(file);
            }
            this.Controls.Add(toolBox);
        }

        private void RectangleDataBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Rectangle rect = new Rectangle()
                {
                    X = Convert.ToInt32((sender as TextBox).Parent.Controls["txtBox_X"].Text),
                    Y = Convert.ToInt32((sender as TextBox).Parent.Controls["txtBox_Y"].Text),
                    Width = Convert.ToInt32((sender as TextBox).Parent.Controls["txtBox_W"].Text),
                    Height = Convert.ToInt32((sender as TextBox).Parent.Controls["txtBox_H"].Text)
                };
                SpriteRectangles sprR = SpriteArray[SelectedParentNode].spriteRects[SelectedRect];
                sprR.spriteRect = rect;
                SpriteArray[SelectedParentNode].spriteRects[SelectedRect] = sprR;
                List<Rectangle> _rect = new List<Rectangle>();
                foreach (SpriteRectangles s in SpriteArray[SelectedParentNode].spriteRects)
                {
                    _rect.Add(s.spriteRect);
                }
                _sprRectangles = _rect.ToArray();
                PictureView.Refresh();
            }
        }

        private void PictureView_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SelectedSprite == null) return;
            float x = (PictureView.Width - SelectedSprite.Width) / 2;
            float y = (PictureView.Height - SelectedSprite.Height) / 2;

            if (_sprRectangles != null)
            {
                foreach (Rectangle r in _sprRectangles)
                {
                    if (r.Contains(new Point((int)(e.X - x), (int)(e.Y - y))))
                    {
                        for(int i=0;i< SpriteArray[SelectedParentNode].spriteRects.Count;i++)
                        {
                            if (SpriteArray[SelectedParentNode].spriteRects[i].spriteRect == r)
                            {
                                if (SelectedRect == i) return;
                                SelectedRect = i;
                                SpriteList.SelectedNode = SpriteList.Nodes[SelectedParentNode].Nodes[i];
                                PictureView.Refresh();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void SpriteList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu cm = new ContextMenu();
                if (e.Node.Parent == null)
                {
                    int SpriteIndex = e.Node.Index;
                    {   // Add rectangle
                        MenuItem addRect = new MenuItem("Add Rectangle");
                        cm.MenuItems.Add(addRect);
                        addRect.Click += (send, EventArg) => { addRectangle_Click(send, SpriteIndex, e.Node.Nodes.Count - 1); };
                    }
                    {   // Save PNG
                        MenuItem savePNG = new MenuItem("Save PNG");
                        cm.MenuItems.Add(savePNG);
                        savePNG.Click += (send, EventArgs) => { SavePNG_Click(send, SpriteIndex); };
                    }
                    {   // Delete PNG
                        MenuItem deleteSprite = new MenuItem("Delete Sprite");
                        cm.MenuItems.Add(deleteSprite);
                        deleteSprite.Click += (send, EventArgs) => { deleteSprite_Click(send, SpriteIndex); };
                    }
                }
                else
                {
                    int SpriteIndex = e.Node.Parent.Index;
                    int SpriteChildIndex = e.Node.Index;
                    MenuItem deleteChildNode = new MenuItem("Remove Rectangle");
                    cm.MenuItems.Add(deleteChildNode);
                    deleteChildNode.Click += (send, EventArgs) => { deleteChildNode_Click(send, SpriteIndex, SpriteChildIndex); };

                }
                SpriteList.ContextMenu = cm;
            }
        }

        private void addRectangle_Click(object send, int spriteIndex, int v)
        {
            SpriteList.Nodes[spriteIndex].Nodes.Add((v + 1).ToString());
            SpriteRectangles sprR = new SpriteRectangles();
            sprR.Index = v + 1;
            sprR.spriteRect = new Rectangle(0, 0, 0, 0);
            SpriteArray[spriteIndex].spriteRects.Add(sprR);
        }

        private void deleteChildNode_Click(object send, int spriteIndex, int spriteChildIndex)
        {
            SpriteArray[spriteIndex].spriteRects.RemoveAt(spriteChildIndex);
            SpriteList.Nodes[spriteIndex].Nodes.RemoveAt(spriteChildIndex);
            SpriteList.Refresh();
            PictureView.Refresh();
        }

        private void deleteSprite_Click(object send, int spriteIndex)
        {
            if (spriteIndex < SpriteList.Nodes.Count - 1)
            {
                for (int i = spriteIndex + 1; i < SpriteList.Nodes.Count; i++)
                {
                    SpriteList.Nodes[i].Text = "Sprite " + (i - 1);
                    Sprite spr = SpriteArray[i];
                    spr.Index -= 1;
                    SpriteArray[i] = spr;
                }
            }
            SpriteArray.RemoveAt(spriteIndex);
            SpriteList.Nodes[spriteIndex].Nodes.Clear();
            SpriteList.Nodes.RemoveAt(spriteIndex);
            SpriteList.SelectedNode = null;
            SpriteList.Refresh();
            PictureView.Refresh();
        }

        private void SavePNG_Click(object sender, int SpriteIndex)
        {
            Image imgToSave = null;
            using (MemoryStream ms = new MemoryStream(SpriteArray[SpriteIndex].ImageBytes, false))
            {
                imgToSave = Image.FromStream(ms);
            }
            if (imgToSave == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = ".PNG Files (*.png)|*.png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string savePath = sfd.FileName;
                string ext = Path.GetExtension(sfd.FileName);
                if (ext != ".png")
                {
                    savePath = sfd.FileName.Replace(ext, ".png");
                }
                imgToSave.Save(savePath);
            }
        }

        private void RectangleDataView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawString("X:", new Font("Century Gothic", 9.0f, FontStyle.Bold), Brushes.Black, new Point(50, 26));
            g.DrawString("Y:", new Font("Century Gothic", 9.0f, FontStyle.Bold), Brushes.Black, new Point(50, 76));
            g.DrawString("Width:", new Font("Century Gothic", 9.0f, FontStyle.Bold), Brushes.Black, new Point(250, 26));
            g.DrawString("Height:", new Font("Century Gothic", 9.0f, FontStyle.Bold), Brushes.Black, new Point(250, 76));
        }

        private void fileSaveAs_Click(object sender, EventArgs e)
        {
            byte[] ImageBytes = null;
            byte[] dataBuffer = null;

            foreach (Sprite _spr in SpriteArray)
            {
                string SpriteData = String.Empty;
                ImageBytes = _spr.ImageBytes;
                List<Rectangle> ImageRectangles = new List<Rectangle>();
                foreach (SpriteRectangles sr in _spr.spriteRects)
                {
                    ImageRectangles.Add(sr.spriteRect);
                }

                foreach (Rectangle r in ImageRectangles)
                {
                    SpriteData += r.X + "|" + r.Y + "|" + r.Width + "|" + r.Height + "~";
                }

                SpriteData += ImageBytes.Length + "~";

                using (MemoryStream mS = new MemoryStream())
                {
                    if (dataBuffer != null)
                    {
                        mS.Write(dataBuffer, 0, dataBuffer.Count());
                    }
                    byte[] _spriteDataBytes = Encoding.ASCII.GetBytes(SpriteData);
                    mS.Write(Encoding.ASCII.GetBytes("<PAK>"), 0, 5);
                    mS.Write(_spriteDataBytes, 0, _spriteDataBytes.Count());
                    mS.Write(ImageBytes, 0, ImageBytes.Count());
                    dataBuffer = mS.ToArray();
                }
            }

            SaveFileDialog sFD = new SaveFileDialog();
            sFD.Filter = ".PAK Data Files (*.pak)|*.pak";
            sFD.RestoreDirectory = true;
            byte[] EncryptedDataBuffer = EncryptPAKFile(dataBuffer);

            if (sFD.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sFD.FileName, EncryptedDataBuffer);
            }
        }

        private byte[] EncryptPAKFile(byte[] buffer)
        {
            string mDB = Convert.ToBase64String(buffer, Base64FormattingOptions.InsertLineBreaks);
            return Encoding.ASCII.GetBytes(mDB);
        }

        private byte[] DecryptPAKFile(byte[] buffer)
        {
            string str = Encoding.ASCII.GetString(buffer);
            return Convert.FromBase64String(str);
        }

        private void PictureView_Paint(object sender, PaintEventArgs e)
        {
            if (SelectedSprite == null) return;

            float x = (PictureView.Width - SelectedSprite.Width) / 2;
            float y = (PictureView.Height - SelectedSprite.Height) / 2;

            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            e.Graphics.FillRectangle(Brushes.LightGray, x, y, SelectedSprite.Width, SelectedSprite.Height);
            e.Graphics.DrawImage(SelectedSprite, x, y, SelectedSprite.Width, SelectedSprite.Height);

            if (_sprRectangles == null) return;

            foreach (Rectangle r in _sprRectangles)
            {
                Color color = Color.Yellow;
                if (SelectedRect >= 0)
                {
                    if (SpriteArray.Find(iX => iX.Index == SelectedParentNode).spriteRects[SelectedRect].spriteRect == r)
                    {
                        color = Color.Red;
                        SetRectangleTextBox("txtBox_X", r.X.ToString());
                        SetRectangleTextBox("txtBox_Y", r.Y.ToString());
                        SetRectangleTextBox("txtBox_W", r.Width.ToString());
                        SetRectangleTextBox("txtBox_H", r.Height.ToString());
                    }
                }
                Pen pen = new Pen(color, 2);
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Outset;
                e.Graphics.DrawRectangle(pen, new Rectangle(r.X + (int)x, r.Y + (int)y, r.Width, r.Height));
            }
        }

        private void SetRectangleTextBox(string ControlName, string text)
        {
            Control[] c = RectangleDataView.Controls.Find(ControlName, true);
            c[0].Text = text;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "PNG Image Files (*.png)|*.png";
            open.RestoreDirectory = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                string FileName = open.FileName;
                string fileExtension = FileName.Substring(FileName.Length - 3);

                Sprite _spr = new Sprite();
                _spr.Index = SpriteList.GetNodeCount(false);
                _spr.ImageBytes = File.ReadAllBytes(open.FileName);
                _spr.spriteRects = new List<SpriteRectangles>();

                SpriteList.Nodes.Add("Sprite " + SpriteList.GetNodeCount(false));
                SpriteArray.Add(_spr);
            }
            else
            {
                ToggleMenuItem(3, false);
                ToggleMenuItem(4, false);
            }
        }

        private void SpriteList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Image img;
            Sprite _spr;
            if (e.Node.Parent == null)
            {
                _spr = SpriteArray.Find(iX => iX.Index == SpriteList.SelectedNode.Index);
                if (_spr.ImageBytes == null) return;
                Bitmap bmp = new Bitmap(new MemoryStream(_spr.ImageBytes));
                img = bmp;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_X"].Enabled = false;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_Y"].Enabled = false;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_W"].Enabled = false;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_H"].Enabled = false;
                ((sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_X"] as TextBox).Text = "";
                ((sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_Y"] as TextBox).Text = "";
                ((sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_W"] as TextBox).Text = img.Width.ToString();
                ((sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_H"] as TextBox).Text = img.Height.ToString();
                SelectedRect = -1;
            }
            else
            {
                SelectedParentNode = e.Node.Parent.Index;
                SelectedRect = SpriteList.SelectedNode.Index;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_X"].Enabled = true;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_Y"].Enabled = true;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_W"].Enabled = true;
                (sender as TreeView).Parent.Controls["RectangleDataView"].Controls["txtBox_H"].Enabled = true;
                _spr = SpriteArray.Find(iX => iX.Index == e.Node.Parent.Index);
                if (_spr.ImageBytes == null) return;
                Bitmap bmp = new Bitmap(new MemoryStream(_spr.ImageBytes));
                img = bmp;
            }

            List<Rectangle> _rects = new List<Rectangle>();
            foreach (SpriteRectangles s_rects in _spr.spriteRects)
            {
                _rects.Add(s_rects.spriteRect);
            }
            _sprRectangles = _rects.ToArray();
            SelectedSprite = img;
            PictureView.Refresh();
        }

        private void fileNew_Click(object sender, EventArgs e)
        {
            ClearTable();
            Add_Click(sender, e);
            ToggleMenuItem(3, true);
            ToggleMenuItem(4, true);
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ClearTable()
        {
            SpriteArray.Clear();
            SpriteList.Nodes.Clear();
            SelectedRect = 0;
            SelectedParentNode = 0;
            SelectedSprite = null;
            ToggleMenuItem(3, false);
            ToggleMenuItem(4, false);
            PictureView.Refresh();
        }

        private void fileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog();
            oFD.Filter = ".PAK Data Files (*.pak)|*.pak";
            int dataLength = 0;
            int imageSize = 0;
            if (oFD.ShowDialog() == DialogResult.OK)
            {
                ClearTable();
                ToggleMenuItem(3, true);
                ToggleMenuItem(4, true);
                using (MemoryStream mS = new MemoryStream(DecryptPAKFile(File.ReadAllBytes(oFD.FileName))))
                //using (MemoryStream mS = new MemoryStream(File.ReadAllBytes(oFD.FileName)))
                {
                    int currentRead = 0;
                    while (mS.Position < mS.Length)
                    {
                        // Set current position of reading
                        byte[] _nMS = new byte[(mS.ToArray().Length - currentRead)];
                        Array.Copy(mS.ToArray(), currentRead, _nMS, 0, _nMS.Length);

                        string _str = Encoding.ASCII.GetString(_nMS);

                        bool isPAK = _str.Substring(0, 5) == "<PAK>" ? true : false;
                        if (isPAK)
                        {
                            _str = _str.Substring(5, _str.Length - 5);
                        }

                        // Get rectangle data length
                        dataLength = Encoding.ASCII.GetByteCount(_str.Substring(0, _str.IndexOf('?')));

                        // Get rectangle data
                        string rectBuff = _str.Substring(0, _str.IndexOf('?'));
                        List<Rectangle> rects = new List<Rectangle>();
                        for (int i = 0; i < rectBuff.Split('~').Length - 2; i++)
                        {
                            string _s = rectBuff.Split('~')[i];
                            List<int> _i = new List<int>();
                            foreach (string _tsr in _s.Split('|'))
                            {
                                _i.Add(Convert.ToInt32(_tsr));
                            }
                            int[] rectData = _i.ToArray();
                            rects.Add(new Rectangle(rectData[0], rectData[1], rectData[2], rectData[3]));
                        }

                        // Get Image bytes
                        imageSize = Convert.ToInt32(rectBuff.Split('~').ElementAt(rectBuff.Split('~').Count() - 2));
                        byte[] ImageBuff = new byte[imageSize];
                        Array.Copy(_nMS, dataLength + 5, ImageBuff, 0, imageSize);

                        // Add to user view
                        Sprite _spr = new Sprite();
                        _spr.Index = SpriteList.GetNodeCount(false);
                        _spr.ImageBytes = ImageBuff;

                        SpriteList.Nodes.Add("Sprite " + SpriteList.GetNodeCount(false));

                        // Place rectangles
                        _spr.spriteRects = new List<SpriteRectangles>();
                        foreach (Rectangle r in rects)
                        {
                            SpriteRectangles _sprRects = new SpriteRectangles();
                            _sprRects.Index = SpriteList.Nodes[SpriteList.GetNodeCount(false) - 1].GetNodeCount(false);
                            _sprRects.spriteRect = r;
                            _spr.spriteRects.Add(_sprRects);
                            SpriteList.Nodes[SpriteList.GetNodeCount(false) - 1].Nodes.Add(SpriteList.Nodes[SpriteList.GetNodeCount(false) - 1].GetNodeCount(false).ToString());
                        }

                        SpriteArray.Add(_spr);
                        currentRead += dataLength + imageSize + 5;
                        mS.Position = currentRead;
                    }
                }
            }
        }

        private void ToggleMenuItem(int Index, bool enable)
        {
            ToolStripItem _tmp = file.DropDownItems[Index];
            _tmp.Enabled = enable;
            file.DropDownItems.RemoveAt(Index);
            file.DropDownItems.Insert(Index, _tmp);
        }
    }

    public class MyPanel : Panel
    {
        public MyPanel()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }
}
