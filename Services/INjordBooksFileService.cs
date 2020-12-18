using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NjordBooks.BLL.Services
{
    public interface INjordBooksFileService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync( IFormFile file );

        public string ConvertByteArrayToFile( byte[] fileData, string extension );

        public string GetFileIcon( string file );

        public string FormatFileSize( long bytes );

        public string ShortenFileName( string name, int length );

        public string GetDefaultAvatarFileName( );

        public Task<byte[]> GetDefaultAvatarFileBytesAsync( );
    }
}
