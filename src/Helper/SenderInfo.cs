﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace Helper
{
    internal class SenderInfo : IDisposable
    {
        private string _tempDirectoryHelper;
        private string _tempDirectory;

        internal string Subject { get; set; }
        internal string Text { get; set; }
        internal List<string> Screens { get; private set; }

        public SenderInfo()
        {
            _tempDirectoryHelper = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Helper");

            _tempDirectory = Path.Combine(
                _tempDirectoryHelper,
                "Screens");

            CreateDirectory(_tempDirectoryHelper);
            CreateDirectory(_tempDirectory);

            Screens = new List<string>();
        }

        public void Dispose()
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_tempDirectory);
                if (directoryInfo.Exists)
                    directoryInfo.Delete(true);
            }
            catch (Exception ex)
            {

            }
        }

        internal void GetScreens()
        {
            int screenLeft = (int)SystemParameters.VirtualScreenLeft;
            int screenTop = (int)SystemParameters.VirtualScreenTop;
            int screenWidth = (int)SystemParameters.VirtualScreenWidth;
            int screenHeight = (int)SystemParameters.VirtualScreenHeight;

            Screens.Clear();

            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap.Size);
                    
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 25L);

                    string fileName = "screen-" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png";
                    string fullName = Path.Combine(_tempDirectory, fileName);
                    bitmap.Save(
                        fullName,
                        GetEncoderInfo("image/png"),
                        encoderParameters);

                    Screens.Add(fullName);
                }
            }
        }

        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int i;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (i = 0; i < encoders.Length; ++i)
            {
                if (encoders[i].MimeType == mimeType)
                    return encoders[i];
            }
            return null;
        }

        private void CreateDirectory(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }
    }
}
