using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionableEmailsTestApi.Models
{
    public class CommentModel
    {

        public Dictionary<string, string> Data { get; set; }
        public string ContainerId { get; set; }
        public object Card { get; set; }
    }
}