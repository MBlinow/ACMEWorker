using System;
using System.Collections.Generic;
using System.Text;

namespace ACMEWorker.Models
{
    class Post
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
