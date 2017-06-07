using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEnrolBot
{
    

    public class Value
    {
        public string entity { get; set; }
        public string type { get; set; }
        public Resolution resolution { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public bool required { get; set; }
        public List<Value> value { get; set; }
    }

    public class Action
    {
        public bool triggered { get; set; }
        public string name { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
        public List<Action> actions { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
        public Resolution resolution { get; set; }
    }

    public class Dialog
    {
        public string prompt { get; set; }
        public string parameterName { get; set; }
        public string parameterType { get; set; }
        public string contextId { get; set; }
        public string status { get; set; }

    }

    public class LUIS
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Entity> entities { get; set; }
        public Dialog dialog { get; set; }
    }

    public class Resolution
    {
    }
    public class Query
    {
        public string Question { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
    }
}
