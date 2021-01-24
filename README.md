# PAK-Editor

How to read the data into a XNA program

        private static byte[] DecryptPAKFile(byte[] buffer)
        {
            string str = Encoding.ASCII.GetString(buffer);
            return Convert.FromBase64String(str);
        }

        public static MapTiles.Tiles LoadMapTiles(string FileLocation, GraphicsDevice graphics)
        {
            MapTiles.Tiles _mapTiles = new MapTiles.Tiles();
            _mapTiles.TileTexture = new Texture2D[2000];
            _mapTiles.TileBmp = new Bitmap[2000];
            _mapTiles.tileBounds = new Microsoft.Xna.Framework.Rectangle[2000];

            using (MemoryStream mS = new MemoryStream(DecryptPAKFile(File.ReadAllBytes(FileLocation))))
            {
                int RectDataLength = 0;
                int ImageSize = 0;
                int currentRead = 0;

                int CurrentSprite = 0;
                while (mS.Position < mS.Length)
                {
                    byte[] _nMS = new byte[(mS.ToArray().Length - currentRead)];
                    Array.Copy(mS.ToArray(), currentRead, _nMS, 0, _nMS.Length);

                    string _byteString = Encoding.ASCII.GetString(_nMS);

                    bool isPAK = _byteString.Substring(0, 5) == "<PAK>" ? true : false;
                    if (isPAK)
                    {
                        _byteString = _byteString.Substring(5, _byteString.Length - 5);
                    }

                    RectDataLength = Encoding.ASCII.GetByteCount(_byteString.Substring(0, _byteString.IndexOf('?')));
                    string rectBuffer = _byteString.Substring(0, _byteString.IndexOf('?'));
                    ImageSize = Convert.ToInt32(rectBuffer.Split('~').ElementAt(rectBuffer.Split('~').Count() - 2));

                    byte[] ImageBuff = new byte[ImageSize];
                    Array.Copy(_nMS, RectDataLength + 5, ImageBuff, 0, ImageSize);

                    currentRead += RectDataLength + ImageSize + 5;
                    mS.Position = currentRead;
                    if (CurrentSprite == 0)
                    {
                        int iter = 0;
                        foreach (string s in rectBuffer.Split('~'))
                        {
                            string[] rectData = s.Split('|');
                            if (rectData.Count() == 1) break;

                            _mapTiles.tileBounds[iter] = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(rectData[0]), Convert.ToInt32(rectData[1]),
                                Convert.ToInt32(rectData[2]), Convert.ToInt32(rectData[3]));
                            _mapTiles.TileBmp[iter] = CropBitmapSource(new Bitmap(new MemoryStream(ImageBuff)), _mapTiles.tileBounds[iter]);
                            using (MemoryStream m = new MemoryStream(GetBitmapMemoryStream(_mapTiles.TileBmp[iter])))
                            {
                                _mapTiles.TileTexture[iter] = new Texture2D(graphics, _mapTiles.tileBounds[iter].Width, _mapTiles.tileBounds[iter].Height);
                                _mapTiles.TileTexture[iter] = Texture2D.FromStream(graphics, m);
                            }
                            iter++;
                        }
                        break;
                    }
                    CurrentSprite++;
                }
            }
            return _mapTiles;
        }

        public static Sprite.SprEntity LoadEntitySprite(Sprite.SprEntity curSpr, string FileLocation, int Sprite, int Image, GraphicsDevice graphics)
        {
            Sprite.SprEntity _sprite = curSpr;

            using (MemoryStream mS = new MemoryStream(DecryptPAKFile(File.ReadAllBytes(FileLocation))))
            {
                int RectDataLength = 0;
                int ImageSize = 0;
                int currentRead = 0;

                int CurrentSprite = 0;
                while (mS.Position < mS.Length)
                {
                    byte[] _nMS = new byte[(mS.ToArray().Length - currentRead)];
                    Array.Copy(mS.ToArray(), currentRead, _nMS, 0, _nMS.Length);

                    string _byteString = Encoding.ASCII.GetString(_nMS);

                    bool isPAK = _byteString.Substring(0, 5) == "<PAK>" ? true : false;
                    if (isPAK)
                    {
                        _byteString = _byteString.Substring(5, _byteString.Length - 5);
                    }

                    RectDataLength = Encoding.ASCII.GetByteCount(_byteString.Substring(0, _byteString.IndexOf('?')));
                    string rectBuffer = _byteString.Substring(0, _byteString.IndexOf('?'));
                    ImageSize = Convert.ToInt32(rectBuffer.Split('~').ElementAt(rectBuffer.Split('~').Count() - 2));

                    byte[] ImageBuff = new byte[ImageSize];
                    Array.Copy(_nMS, RectDataLength + 5, ImageBuff, 0, ImageSize);

                    currentRead += RectDataLength + ImageSize + 5;
                    mS.Position = currentRead;
                    if (CurrentSprite == Sprite)
                    {
                        string[] rectData = rectBuffer.Split('~')[Image].Split('|');

                        _sprite.spriteBounds[Image] = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(rectData[0]), Convert.ToInt32(rectData[1]),
                            Convert.ToInt32(rectData[2]), Convert.ToInt32(rectData[3]));
                        if (rectData.Length <= 4) _sprite.xOffset[Image] = 0;
                         else _sprite.xOffset[Image] = Convert.ToInt32(rectData[4]);
                        _sprite.spriteBmp[Image] = CropBitmapSource(new Bitmap(new MemoryStream(ImageBuff)), _sprite.spriteBounds[Image]);
                        using (MemoryStream m = new MemoryStream(GetBitmapMemoryStream(_sprite.spriteBmp[Image])))
                        {
                            _sprite.Sprite[Image] = new Texture2D(graphics, _sprite.spriteBounds[Image].Width, _sprite.spriteBounds[Image].Height);
                            _sprite.Sprite[Image] = Texture2D.FromStream(graphics, m);
                        }
                        break;
                    }
                    CurrentSprite++;
                }
            }

            return _sprite;
        }

        public static Sprite.Spr LoadSprite(string FileLocation, int Sprite, int Image, GraphicsDevice graphics)
        {
            Sprite.Spr _sprite = new Sprite.Spr();

            using (MemoryStream mS = new MemoryStream(DecryptPAKFile(File.ReadAllBytes(FileLocation))))
            {
                int RectDataLength = 0;
                int ImageSize = 0;
                int currentRead = 0;

                int CurrentSprite = 0;
                while (mS.Position < mS.Length)
                {
                    byte[] _nMS = new byte[(mS.ToArray().Length - currentRead)];
                    Array.Copy(mS.ToArray(), currentRead, _nMS, 0, _nMS.Length);

                    string _byteString = Encoding.ASCII.GetString(_nMS);

                    bool isPAK = _byteString.Substring(0, 5) == "<PAK>" ? true : false;
                    if (isPAK)
                    {
                        _byteString = _byteString.Substring(5, _byteString.Length - 5);
                    }

                    RectDataLength = Encoding.ASCII.GetByteCount(_byteString.Substring(0, _byteString.IndexOf('?')));
                    string rectBuffer = _byteString.Substring(0, _byteString.IndexOf('?'));
                    ImageSize = Convert.ToInt32(rectBuffer.Split('~').ElementAt(rectBuffer.Split('~').Count() - 2));

                    byte[] ImageBuff = new byte[ImageSize];
                    Array.Copy(_nMS, RectDataLength + 5, ImageBuff, 0, ImageSize);

                    currentRead += RectDataLength + ImageSize + 5;
                    mS.Position = currentRead;
                    if (CurrentSprite == Sprite)
                    {
                        string[] rectData = rectBuffer.Split('~')[Image].Split('|');

                        _sprite.spriteBounds = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(rectData[0]), Convert.ToInt32(rectData[1]),
                            Convert.ToInt32(rectData[2]), Convert.ToInt32(rectData[3]));
                        _sprite.spriteBmp = CropBitmapSource(new Bitmap(new MemoryStream(ImageBuff)), _sprite.spriteBounds);
                        using (MemoryStream m = new MemoryStream(GetBitmapMemoryStream(_sprite.spriteBmp)))
                        {
                            _sprite.Sprite = new Texture2D(graphics, _sprite.spriteBounds.Width, _sprite.spriteBounds.Height);
                            _sprite.Sprite = Texture2D.FromStream(graphics, m);
                        }
                        break;
                    }
                    CurrentSprite++;
                }
            }

            return _sprite;
        }

        public static byte[] GetBitmapMemoryStream(Bitmap bmp)
        {
            using (MemoryStream mS = new MemoryStream())
            {
                bmp.Save(mS, System.Drawing.Imaging.ImageFormat.Png);
                return mS.ToArray();
            }
        }

        private static Bitmap CropBitmapSource(Bitmap bmp, Microsoft.Xna.Framework.Rectangle srcRectangle)
        {
            Rectangle r = new Rectangle(srcRectangle.X, srcRectangle.Y, srcRectangle.Width, srcRectangle.Height);
            FastImageCroper fic = new FastImageCroper(bmp);
            Bitmap _bmp = fic.Crop(r);
            fic.Dispose();
            return _bmp;
        }
    }

    internal unsafe sealed class FastImageCroper : IDisposable
    {
        private readonly Bitmap _srcImg;
        private readonly BitmapData _srcImgBitmapData;
        private readonly int _bpp;
        private readonly byte* _srtPrt;

        public FastImageCroper(Bitmap srcImg)
        {
            _srcImg = srcImg;
            _srcImgBitmapData = srcImg.LockBits(new Rectangle(0, 0, srcImg.Width, srcImg.Height), ImageLockMode.ReadOnly, srcImg.PixelFormat);
            _bpp = _srcImgBitmapData.Stride / _srcImgBitmapData.Width; // == 4
            _srtPrt = (byte*)_srcImgBitmapData.Scan0.ToPointer();
        }

        public Bitmap Crop(Rectangle rectangle)
        {
            Bitmap dstImg = new Bitmap(rectangle.Width, rectangle.Height, _srcImg.PixelFormat);
            BitmapData dstImgBitmapData = dstImg.LockBits(new Rectangle(0, 0, dstImg.Width, dstImg.Height), ImageLockMode.WriteOnly, dstImg.PixelFormat);
            byte* dstPrt = (byte*)dstImgBitmapData.Scan0.ToPointer();
            byte* srcPrt = _srtPrt + rectangle.Y * _srcImgBitmapData.Stride + rectangle.X * _bpp;

            for (int y = 0; y < rectangle.Height; y++)
            {
                int srcIndex = y * _srcImgBitmapData.Stride;
                int croppedIndex = y * dstImgBitmapData.Stride;
                memcpy(dstPrt + croppedIndex, srcPrt + srcIndex, dstImgBitmapData.Stride);
            }

            dstImg.UnlockBits(dstImgBitmapData);
            return dstImg;
        }


        public void Dispose()
        {
            _srcImg.UnlockBits(_srcImgBitmapData);
        }


        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcpy(byte* dest, byte* src, long count);
    }
