using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;

namespace Edubase.Common.IO;

public sealed class FileHelper
{
    public enum ePriority
    {
        /// <summary>
        /// Attempts to derive the extension from the mime type in the first instance
        /// </summary>
        FromMimeType,

        /// <summary>
        /// Attempts to get the extension from the filename, but then from the mime type as a backup
        /// </summary>
        FromFileName
    }

    private static readonly Dictionary<string, string> _mimeTypesDictionary = new()
    {
        {"ai", "application/postscript"},
        {"aif", "audio/x-aiff"},
        {"aifc", "audio/x-aiff"},
        {"aiff", "audio/x-aiff"},
        {"asc", "text/plain"},
        {"atom", "application/atom+xml"},
        {"au", "audio/basic"},
        {"avi", "video/x-msvideo"},
        {"bcpio", "application/x-bcpio"},
        {"bin", "application/octet-stream"},
        {"bmp", "image/bmp"},
        {"cdf", "application/x-netcdf"},
        {"cgm", "image/cgm"},
        {"class", "application/octet-stream"},
        {"cpio", "application/x-cpio"},
        {"cpt", "application/mac-compactpro"},
        {"csh", "application/x-csh"},
        {"css", "text/css"},
        {"dcr", "application/x-director"},
        {"dif", "video/x-dv"},
        {"dir", "application/x-director"},
        {"djv", "image/vnd.djvu"},
        {"djvu", "image/vnd.djvu"},
        {"dll", "application/octet-stream"},
        {"dmg", "application/octet-stream"},
        {"dms", "application/octet-stream"},
        {"doc", "application/msword"},
        {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {"docm","application/vnd.ms-word.document.macroEnabled.12"},
        {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
        {"dtd", "application/xml-dtd"},
        {"dv", "video/x-dv"},
        {"dvi", "application/x-dvi"},
        {"dxr", "application/x-director"},
        {"eps", "application/postscript"},
        {"etx", "text/x-setext"},
        {"exe", "application/octet-stream"},
        {"ez", "application/andrew-inset"},
        {"gif", "image/gif"},
        {"gram", "application/srgs"},
        {"grxml", "application/srgs+xml"},
        {"gtar", "application/x-gtar"},
        {"hdf", "application/x-hdf"},
        {"hqx", "application/mac-binhex40"},
        {"htm", "text/html"},
        {"html", "text/html"},
        {"ice", "x-conference/x-cooltalk"},
        {"ico", "image/x-icon"},
        {"ics", "text/calendar"},
        {"ief", "image/ief"},
        {"ifb", "text/calendar"},
        {"iges", "model/iges"},
        {"igs", "model/iges"},
        {"jnlp", "application/x-java-jnlp-file"},
        {"jp2", "image/jp2"},
        {"jpg", "image/jpeg"},
        {"js", "application/x-javascript"},
        {"kar", "audio/midi"},
        {"latex", "application/x-latex"},
        {"lha", "application/octet-stream"},
        {"lzh", "application/octet-stream"},
        {"m3u", "audio/x-mpegurl"},
        {"m4a", "audio/mp4a-latm"},
        {"m4b", "audio/mp4a-latm"},
        {"m4p", "audio/mp4a-latm"},
        {"m4u", "video/vnd.mpegurl"},
        {"m4v", "video/x-m4v"},
        {"mac", "image/x-macpaint"},
        {"man", "application/x-troff-man"},
        {"mathml", "application/mathml+xml"},
        {"me", "application/x-troff-me"},
        {"mesh", "model/mesh"},
        {"mid", "audio/midi"},
        {"midi", "audio/midi"},
        {"mif", "application/vnd.mif"},
        {"mov", "video/quicktime"},
        {"movie", "video/x-sgi-movie"},
        {"mp2", "audio/mpeg"},
        {"mp3", "audio/mpeg"},
        {"mp4", "video/mp4"},
        {"mpe", "video/mpeg"},
        {"mpeg", "video/mpeg"},
        {"mpg", "video/mpeg"},
        {"mpga", "audio/mpeg"},
        {"ms", "application/x-troff-ms"},
        {"msh", "model/mesh"},
        {"mxu", "video/vnd.mpegurl"},
        {"nc", "application/x-netcdf"},
        {"oda", "application/oda"},
        {"ogg", "application/ogg"},
        {"pbm", "image/x-portable-bitmap"},
        {"pct", "image/pict"},
        {"pdb", "chemical/x-pdb"},
        {"pdf", "application/pdf"},
        {"pgm", "image/x-portable-graymap"},
        {"pgn", "application/x-chess-pgn"},
        {"pic", "image/pict"},
        {"pict", "image/pict"},
        {"png", "image/png"},
        {"pnm", "image/x-portable-anymap"},
        {"pnt", "image/x-macpaint"},
        {"pntg", "image/x-macpaint"},
        {"ppm", "image/x-portable-pixmap"},
        {"ppt", "application/vnd.ms-powerpoint"},
        {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
        {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {"ps", "application/postscript"},
        {"qt", "video/quicktime"},
        {"qti", "image/x-quicktime"},
        {"qtif", "image/x-quicktime"},
        {"ra", "audio/x-pn-realaudio"},
        {"ram", "audio/x-pn-realaudio"},
        {"ras", "image/x-cmu-raster"},
        {"rdf", "application/rdf+xml"},
        {"rgb", "image/x-rgb"},
        {"rm", "application/vnd.rn-realmedia"},
        {"roff", "application/x-troff"},
        {"rtf", "text/rtf"},
        {"rtx", "text/richtext"},
        {"sgm", "text/sgml"},
        {"sgml", "text/sgml"},
        {"sh", "application/x-sh"},
        {"shar", "application/x-shar"},
        {"silo", "model/mesh"},
        {"sit", "application/x-stuffit"},
        {"skd", "application/x-koan"},
        {"skm", "application/x-koan"},
        {"skp", "application/x-koan"},
        {"skt", "application/x-koan"},
        {"smi", "application/smil"},
        {"smil", "application/smil"},
        {"snd", "audio/basic"},
        {"so", "application/octet-stream"},
        {"spl", "application/x-futuresplash"},
        {"src", "application/x-wais-source"},
        {"sv4cpio", "application/x-sv4cpio"},
        {"sv4crc", "application/x-sv4crc"},
        {"svg", "image/svg+xml"},
        {"swf", "application/x-shockwave-flash"},
        {"t", "application/x-troff"},
        {"tar", "application/x-tar"},
        {"tcl", "application/x-tcl"},
        {"tex", "application/x-tex"},
        {"texi", "application/x-texinfo"},
        {"texinfo", "application/x-texinfo"},
        {"tif", "image/tiff"},
        {"tiff", "image/tiff"},
        {"tr", "application/x-troff"},
        {"tsv", "text/tab-separated-values"},
        {"txt", "text/plain"},
        {"ustar", "application/x-ustar"},
        {"vcd", "application/x-cdlink"},
        {"vrml", "model/vrml"},
        {"vxml", "application/voicexml+xml"},
        {"wav", "audio/x-wav"},
        {"wbmp", "image/vnd.wap.wbmp"},
        {"wbmxl", "application/vnd.wap.wbxml"},
        {"wml", "text/vnd.wap.wml"},
        {"wmlc", "application/vnd.wap.wmlc"},
        {"wmls", "text/vnd.wap.wmlscript"},
        {"wmlsc", "application/vnd.wap.wmlscriptc"},
        {"wrl", "model/vrml"},
        {"xbm", "image/x-xbitmap"},
        {"xht", "application/xhtml+xml"},
        {"xhtml", "application/xhtml+xml"},
        {"xls", "application/vnd.ms-excel"},
        {"xml", "application/xml"},
        {"xpm", "image/x-xpixmap"},
        {"xsl", "application/xml"},
        {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
        {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
        {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
        {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {"xslt", "application/xslt+xml"},
        {"xul", "application/vnd.mozilla.xul+xml"},
        {"xwd", "image/x-xwindowdump"},
        {"xyz", "chemical/x-xyz"},
        {"zip", "application/zip"},
        {"csv", "text/csv"}
      };


    /// <summary>
    /// Gets the (dotless, lower-cased) file extension either from the filename or the mime type, depending on the said priority.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public string GetExtension(string fileName, string mimeType, ePriority priority)
    {
        return (priority == ePriority.FromFileName)
            ? (GetExtensionFromFileName(fileName) ?? GetExtensionFromMimeType(mimeType))
            : (GetExtensionFromMimeType(mimeType) ?? GetExtensionFromFileName(fileName));
    }

    /// <summary>
    /// Returns the dotless, lower-cased file extension based purely on MIME type.
    /// </summary>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    private string GetExtensionFromMimeType(string mimeType)
    {
        mimeType = mimeType.Clean()?.ToLower();
        string retVal = null;
        if (mimeType.Clean() != null)
        {
            if (_mimeTypesDictionary.Count(x => x.Value == mimeType) > 0)
            {
                var mapping = _mimeTypesDictionary.FirstOrDefault(x => x.Value == mimeType);
                retVal = mapping.Key;
            }
        }
        return retVal;
    }

    /// <summary>
    /// Returns the filename's dotless, lower-cased file extension
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private string GetExtensionFromFileName(string fileName)
    {
        fileName = fileName.Clean();
        string retVal = null;
        if (fileName != null)
        {
            retVal = Path.GetExtension(fileName).Clean()?.ToLower()?.RemoveSubstring(".");
        }
        return retVal;
    }

    /// <summary>
    /// Gets the (dotless, lower-cased) file extension from the filename passed in.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetExtension(string fileName)
    {
        return GetExtension(fileName, null, ePriority.FromFileName);
    }

    /// <summary>
    /// Gets the (dotless, lower-cased) file extension from the mime type, and if that's not possible, then from the filename.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    public string GetExtension(string fileName, string mimeType)
    {
        return GetExtension(fileName, mimeType, ePriority.FromMimeType);
    }

    /// <summary>
    /// Generates a random filename based on the passed-in original filename and mime type.
    /// </summary>
    /// <param name="originalFileName"></param>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    public string GenerateRandomFileName(string originalFileName, string mimeType)
    {
        var extension = GetExtension(originalFileName, mimeType) ?? "dat";
        return string.Concat(Guid.NewGuid().ToString("N").ToLower(), ".", extension);
    }

    public static string GetTempFileName(string extension)
    {
        if (!extension.StartsWith(".")) extension = string.Concat(".", extension);
        return Path.Combine(Path.GetTempPath(), string.Concat(Guid.NewGuid().ToString("N"), extension));
    }

    /// <summary>
    /// Generates a random filename based on the passed-in original filename
    /// </summary>
    /// <param name="originalFileName"></param>
    /// <returns></returns>
    public string GenerateRandomFileName(string originalFileName)
    {
        return GenerateRandomFileName(originalFileName, null);
    }

    /// <summary>
    /// Returns the MIME type of a file based on its filename.  It'll look at its own internal mapping before using System.Web.MimeMapping
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetMimeType(string fileName)
    {
        var extension = GetExtension(fileName);
        if (_mimeTypesDictionary.TryGetValue(extension, out var value))
        {
            return value;
        }

        FileExtensionContentTypeProvider provider = new();
        return provider.TryGetContentType(fileName, out var mimeType)
            ? mimeType
            : "application/octet-stream"; // Upgrade from NET Framework 4.8 fallback to what System.Web.MimeMapping.GetMimeMapping(fileName) returned if no matches
    }
}
