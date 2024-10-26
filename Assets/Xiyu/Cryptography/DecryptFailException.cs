namespace Xiyu.Cryptography
{
    public sealed class DecryptFailException : System.Security.Cryptography.CryptographicException
    {
        public string ErrorTitle { get; }

        public DecryptFailException(string message, string errorTitle) : base(message)
        {
            ErrorTitle = errorTitle;
        }
    }
}