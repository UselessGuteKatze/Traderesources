using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yoda.Interfaces;

namespace TradeResourcesPlugin.Helpers {
    public class FileReaderHelper {

        public static string SaveFileAndGetId(IYodaRequestContext requestContext, byte[] fileContent)
        {
            var fileName = Guid.NewGuid().ToString("N");
            var filePath = Path.Combine(GetTempDir(requestContext), fileName);
            File.WriteAllBytes(filePath, fileContent);
            return fileName;
        }
        public static string GetTempDir(IYodaRequestContext requestContext)
        {
            var tempDir = requestContext.ServerMapPath("~/temp");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            return tempDir;
        }
        public static string GetTempFilePath(IYodaRequestContext requestContext, string fileId)
        {
            return Path.Combine(GetTempDir(requestContext), fileId);
        }

    }
}
