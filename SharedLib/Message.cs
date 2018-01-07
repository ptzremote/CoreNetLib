using System;

namespace SharedLib
{
    [Serializable]
    public class Message
    {
        public string Author { get; set; }
        public string Text { get; set; }
    }
}
