using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("Questionnaire")]
    public class WSQuestionnaire
    {
        [System.Xml.Serialization.XmlArray("Questions")]
        [System.Xml.Serialization.XmlArrayItem("Question",typeof(WSQuestion))]
        public ArrayList listQuestions;

        public WSQuestionnaire()
        {
            listQuestions = new ArrayList();
        }

        public int AddItem( String item ) 
        {
            return listQuestions.Add(item);
        }

        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            string result = "";

            foreach (WSQuestion question in listQuestions)
            {
                if (result.Length > 0)
                    result += "|";
                result += question.WriteToString();
            }

            return result;
        }	

    }


}
