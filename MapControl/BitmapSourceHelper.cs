﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2017 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace MapControl
{
    /// <summary>
    /// Creates frozen BitmapSources from Stream or Uri.
    /// </summary>
    public static class BitmapSourceHelper
    {
        public static BitmapSource FromStream(Stream stream)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        public static BitmapSource FromUri(Uri uri)
        {
            try
            {
				var request = (HttpWebRequest)WebRequest.Create(uri);
				//request.Referer = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.86 Safari/537.36";

				using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    responseStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return FromStream(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
