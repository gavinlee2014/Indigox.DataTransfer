﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Indigox.DataTransfer.QRCode
{
    class QRCodeUtil
    {
        public static Bitmap GenerateMyQCCode(string text)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            QCwriter.Options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                ErrorCorrection = ErrorCorrectionLevel.H,
                CharacterSet = "UTF-8",
                Width = 250,
                Height = 250,
            };
            return QCwriter.Write(text);


            //var barcodeBitmap = new Bitmap(result);

            //return barcodeBitmap;

            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //return qrCodeImage;

            //return null;
        }
    }
}
