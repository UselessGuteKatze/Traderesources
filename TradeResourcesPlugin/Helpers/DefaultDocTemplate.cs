﻿using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using TradeResourcesPlugin.Helpers.Fonts;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;

namespace TradeResourcesPlugin.Helpers {
    public abstract class DefaultDocTemplate {
        public virtual string[] GetUIPackages() {
            return new string[] { "default-doc-package" };
        }

        public string DocNumber = "";
        public void SetDocNumber(string docNumber) {
            DocNumber = docNumber;
        }

        public abstract string GetContent();
        public Panel GetPanelHtml() {
            var panel = new Panel("pre-render p-10-mm");
            panel.AddComponent(new UiPackages(GetUIPackages()));
            panel.AddComponent(new HtmlText(GetContent()));
            return panel;
        }

        public class QrGenerator {

            private class QRCode : AbstractQRCode, IDisposable {
                /// <summary>
                /// Constructor without params to be used in COM Objects connections
                /// </summary>
                public QRCode() { }

                public QRCode(QRCodeData data) : base(data) { }

                public Bitmap GetGraphic(int pixelsPerModule) {
                    return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
                }

                public Bitmap GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true) {
                    return this.GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(darkColorHtmlHex), ColorTranslator.FromHtml(lightColorHtmlHex), drawQuietZones);
                }

                public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true) {
                    var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
                    var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

                    var bmp = new Bitmap(size, size);
                    using (var gfx = Graphics.FromImage(bmp))
                    using (var lightBrush = new SolidBrush(lightColor))
                    using (var darkBrush = new SolidBrush(darkColor)) {
                        for (var x = 0; x < size + offset; x = x + pixelsPerModule) {
                            for (var y = 0; y < size + offset; y = y + pixelsPerModule) {
                                var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                                if (module) {
                                    gfx.FillRectangle(darkBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                                }
                                else {
                                    gfx.FillRectangle(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                                }
                            }
                        }

                        gfx.Save();
                    }

                    return bmp;
                }

                public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true, Color? iconBackgroundColor = null) {
                    var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
                    var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

                    var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using (var gfx = Graphics.FromImage(bmp))
                    using (var lightBrush = new SolidBrush(lightColor))
                    using (var darkBrush = new SolidBrush(darkColor)) {
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.CompositingQuality = CompositingQuality.HighQuality;
                        gfx.Clear(lightColor);
                        var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

                        for (var x = 0; x < size + offset; x = x + pixelsPerModule) {
                            for (var y = 0; y < size + offset; y = y + pixelsPerModule) {
                                var moduleBrush = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1] ? darkBrush : lightBrush;
                                gfx.FillRectangle(moduleBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                            }
                        }

                        if (drawIconFlag) {
                            float iconDestWidth = iconSizePercent * bmp.Width / 100f;
                            float iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
                            float iconX = (bmp.Width - iconDestWidth) / 2;
                            float iconY = (bmp.Height - iconDestHeight) / 2;
                            var centerDest = new RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                            var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                            var iconBgBrush = iconBackgroundColor != null ? new SolidBrush((Color)iconBackgroundColor) : lightBrush;
                            //Only render icon/logo background, if iconBorderWith is set > 0
                            if (iconBorderWidth > 0) {
                                using (GraphicsPath iconPath = CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2)) {
                                    gfx.FillPath(iconBgBrush, iconPath);
                                }
                            }
                            gfx.DrawImage(icon, iconDestRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                        }

                        gfx.Save();
                    }

                    return bmp;
                }

                internal GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius) {
                    var roundedRect = new GraphicsPath();
                    roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                    roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
                    roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                    roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
                    roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                    roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
                    roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                    roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
                    roundedRect.CloseFigure();
                    return roundedRect;
                }
            }

            public static byte[] GenerateQr(string content) {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.L);

                using (var qrCode = new QRCode(qrCodeData)) {
                    var bitmap = qrCode.GetGraphic(10, "#000", "#f3f3f4", false);
                    using (var memoryStream = new MemoryStream()) {
                        bitmap.Save(memoryStream, ImageFormat.Png);
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}