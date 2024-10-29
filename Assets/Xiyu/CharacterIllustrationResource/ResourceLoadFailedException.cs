using System;

namespace Xiyu.CharacterIllustrationResource
{
    public class ResourceLoadFailedException : Exception
    {
        public ResourceLoadFailedException(string message) : base(message)  
        {  
        } 
    }
}