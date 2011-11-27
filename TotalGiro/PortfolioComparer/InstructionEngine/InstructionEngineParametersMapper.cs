using System;
using System.Collections;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This class is used to retrieve InstructionEngineParameters data. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class InstructionEngineParametersMapper
    {
        /// <summary>
        /// Retrieves the InstructionEngineParameters
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>InstructionEngineParameters object</returns>      
        public static InstructionEngineParameters GetParameters(IDalSession session)
        {
            InstructionEngineParameters engineParams = null;
            IList list = session.GetList(typeof(InstructionEngineParameters));
            if (list != null && list.Count == 1)
            {
                engineParams = (InstructionEngineParameters)list[0];
            }
            return engineParams;
        }
    }
}
