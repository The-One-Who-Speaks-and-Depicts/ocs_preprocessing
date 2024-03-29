﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace CorpusDraftCSharp
{
    [Serializable]
    public class Clause
    {
        

        #region objectValues
        [JsonProperty]
        public string documentID;
        [JsonProperty]
        public string filePath;
        [JsonProperty]
        public string textID;
        [JsonProperty]
        protected List<Dictionary<string, List<IValue>>> clauseFields = new List<Dictionary<string, List<IValue>>>();
        [JsonProperty]
        public string clauseID;
        [JsonProperty]
        public string clauseText;
        [JsonProperty]
        public List<Realization> realizations = new List<Realization>();
        #endregion


        #region Constructors
        [JsonConstructor]
        public Clause(string _documentID, string _textID, string _filePath, string _clauseID, string _clauseText, List<Dictionary<string, List<IValue>>> _clauseFields, List<Realization> _realizations)
        {
            this.documentID = _documentID;
            this.filePath = _filePath;
            this.textID = _textID;
            this.clauseID = _clauseID;
            this.clauseText = _clauseText;
            this.clauseFields = _clauseFields;
            this.realizations = _realizations;
        }
        public Clause(string _documentID, string _textID, string _filePath, string _clauseID, string _clauseText)
        {
            this.documentID = _documentID;
            this.filePath = _filePath;
            this.textID = _textID;
            this.clauseID = _clauseID;
            this.clauseText = _clauseText;
        }

        public Clause(Text text, string _clauseID, string _clauseText)
        {
            this.documentID = text.documentID;
            this.filePath = text.filePath;
            this.textID = text.textID;
            this.clauseID = _clauseID;
            this.clauseText = _clauseText;
        }
        public Clause()
        {

        }

        #endregion

        #region publicMethods
        public string Output()
        {
            Func<string> tokens = () =>
            {
                string collected = "";
                foreach (var r in realizations.OrderBy(realization => Convert.ToInt32(realization.documentID)).ThenBy(realization => Convert.ToInt32(realization.textID)).ThenBy(realization => Convert.ToInt32(realization.clauseID)).ThenBy(realization => Convert.ToInt32(realization.realizationID)))
                {
                    collected += r.Output();
                }
                return collected;
            };
            try
            {
                Func<List<Dictionary<string, List<IValue>>>, string> clauseInRawText = (List<Dictionary<string, List<IValue>>> fields) =>
                {
                    string result = "";
                    foreach (var optional_tagging in fields)
                    {
                        foreach (var field in optional_tagging)
                        {
                            result += field.Key;
                            result += ":";
                            foreach (var fieldValue in field.Value)
                            {
                                result += fieldValue.name;
                                result += ";";
                            }
                            result += "||";
                        }
                        result += "\n";
                    }
                    return result;
                };
                Func<List<Dictionary<string, List<IValue>>>, string> clauseInHTML = (List<Dictionary<string, List<IValue>>> fields) =>
                {
                    return clauseInRawText.Invoke(fields).Replace("\n", "<br />");
                };
                return "<span title=\"" + clauseInRawText.Invoke(clauseFields) + "\" data-content=\"" + clauseInHTML.Invoke(clauseFields) + "\" class=\"clause\" id=\"" + this.documentID + "|" + this.textID + "|" + this.clauseID + "\"> " + tokens.Invoke() + "</span><br />";
            }
            catch
            {
                return "<span title= \"\" data-content=\"\" class=\"clause\" id=\"" + this.documentID + "|" + this.clauseID  + "\"> " + tokens.Invoke() + "</span><br />";
            }
        }
        public string Jsonize()
        {
            string jsonedClause = JsonConvert.SerializeObject(this);
            return jsonedClause;
        }
        #endregion
    }
}
