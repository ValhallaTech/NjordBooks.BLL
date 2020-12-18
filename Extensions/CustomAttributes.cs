using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NjordBooks.BLL.Extensions
{
    [AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int maxFileSize;
        public MaxFileSizeAttribute( int maxFileSize ) => this.maxFileSize = maxFileSize;

        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if ( !( value is IFormFile file ) ) return ValidationResult.Success;

            return file.Length > this.maxFileSize ? new ValidationResult( this.GetErrorMessage( ) ) : ValidationResult.Success;
        }

        public string GetErrorMessage( ) => $"Maximum allowed file size is {this.maxFileSize} bytes.";
    }

    [AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;
        public AllowedExtensionsAttribute( string[] extensions ) => this.extensions = extensions;

        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if ( !( value is IFormFile file ) ) return ValidationResult.Success;

            string extension = Path.GetExtension( file.FileName );

            return !this.extensions.Contains( extension?.ToLower( ) ) ? new ValidationResult( GetErrorMessage( extension ) ) : ValidationResult.Success;
        }

        public static string GetErrorMessage( string ext ) => $"The file extension {ext} is not allowed!";
    }
}
