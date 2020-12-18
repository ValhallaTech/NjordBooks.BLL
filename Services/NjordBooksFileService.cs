using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Services
{
    public class NjordBooksFileService : INjordBooksFileService
    {
        private readonly DefaultSettings defaultSettings;

        public NjordBooksFileService( IOptions<DefaultSettings> defaultSettings ) =>
            this.defaultSettings = defaultSettings.Value;

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public async Task<byte[]> ConvertFileToByteArrayAsync( IFormFile file )
        {
            MemoryStream memoryStream = new MemoryStream( );
            await file.CopyToAsync( memoryStream ).ConfigureAwait( false );
            byte[] byteFile = memoryStream.ToArray( );
            memoryStream.Close( );
            await memoryStream.DisposeAsync( ).ConfigureAwait( false );

            return byteFile;
        }

        public string ConvertByteArrayToFile( byte[] fileData, string extension )
        {
            string imageBase64Data = Convert.ToBase64String( fileData );

            return string.Format( $"data:image/{extension};base64,{imageBase64Data}" );
        }

        public string GetFileIcon( string file )
        {
            string ext = Path.GetExtension( file ).Replace( ".", "" );

            return $"/img/png/{ext}.png";
        }

        public string FormatFileSize( long bytes )
        {
            int counter = 0;
            decimal number = bytes;

            while ( Math.Round( number / 1024 ) >= 1 )
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1}{this.suffixes[counter]}";
        }

        public string ShortenFileName( string name, int length )
        {
            name = Path.GetFileNameWithoutExtension( name );
            int len = name.Length > length ? length : name.Length;
            string result = name.Substring( 0, len );
            return $"{result}...";
        }

        public string GetDefaultAvatarFileName( ) => this.defaultSettings.Avatar;

        public async Task<byte[]> GetDefaultAvatarFileBytesAsync( )
        {
            try
            {
                string path = Path.Combine( Directory.GetCurrentDirectory( ), @"wwwroot/assets/img",
                                            this.defaultSettings.Avatar );

                return await File.ReadAllBytesAsync( path ).ConfigureAwait( false );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex );

                return null;
            }
        }
    }
}
