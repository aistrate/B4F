using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("Question")]
    public class WSQuestion
    {
        public WSQuestion()
        {
            possibleanswers = new ArrayList();
        }

        [XmlElement("ValueQ")]
        public string Question
        {
            get { return question; }
            set { question = value; }
        }

        [XmlElement("ValueA")]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        [System.Xml.Serialization.XmlArray("PossibleAnswers")]
        [System.Xml.Serialization.XmlArrayItem("Answer", typeof(String))]
        public ArrayList possibleanswers;

        string question = "";
        string answer = "";

        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            string result = "";

            foreach (String tmpanswer in possibleanswers)
            {
                if (result.Length > 0)
                    result += ",";
                result += tmpanswer;
            }

            return string.Format("{0}|{1}|{2}", question, answer, result);
        }	

    }
}
