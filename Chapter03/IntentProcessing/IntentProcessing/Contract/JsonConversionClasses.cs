using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntentProcessing.Contract
{
    public class RootObject
    {
        public string analyzerId { get; set; }
        public List<object> result { get; set; }
    }

    public class Token
    {
        public int Len { get; set; }
        public string NormalizedToken { get; set; }
        public int Offset { get; set; }
        public string RawToken { get; set; }
    }

    public class TokenRootObject
    {
        public int Len { get; set; }
        public int Offset { get; set; }
        public List<Token> Tokens { get; set; }
    }

    public class Tree
    {
        public List<string> Nodes { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
    }

    public class LuisResponse
    {
        public string query { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
    }

    enum EtityType
    {
        Location,
        Name,
        Company
    }
}
