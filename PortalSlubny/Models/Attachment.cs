using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Attachment
    {
        public Attachment(string Id, string fileName, string path, long bytes)
        {
            this.Id = Id;
            this.Title = fileName;
            this.Path = path;
            this.Bytes = bytes;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public long Bytes { get; set; }
    }
}