using System;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace B4F.TotalGiro.Utils
{
	/// <summary>
    /// This class implements a table-driven finite state machine.
    /// The table is defined by an XML document. The System.Xml.XmlTextReader 
    /// class is used for fast scanning of the table and allows larger tables 
    /// to be used as opposed to System.Xml.XmlDocument.
    /// </summary>
    public class XMLStateMachine 
    {
        /// <summary>
        /// The bool SetStatus method gets the next valid state given the current state and the supplied input.
        /// </summary>
        /// <param name="currentStateID">The current status.</param>
        /// <returns>A int that identifies the next state</returns>
        public int SetStatus(Stream stateTable, int currentStateID, int eventID, IStateMachineClient client) 
        {
            XmlTextReader tableParser = null;
            int nextState = 0;
            bool hitState = false;
            bool hitEvent = false;
            string clientResult = string.Empty;
            
            if (currentStateID != 0 && eventID != 0) 
            {
				try 
                {
                    tableParser = new XmlTextReader(stateTable);
                    if (tableParser == null)
                        throw new ApplicationException("The XMLStateMachine was not initialised with a stream");

                    while (tableParser.Read() && !hitState) 
                    {
                        if (tableParser.NodeType == XmlNodeType.Element && tableParser.Name == "state") 
                        {
							if ( tableParser.HasAttributes ) 
                            {
								int state = int.Parse(tableParser.GetAttribute("id"));
								if ( state == currentStateID ) 
                                {
                                    hitState = true;
                                    XmlReader eventData = tableParser.ReadSubtree();
                                    // Get event data
                                    while (eventData.Read() && !hitEvent)
                                    {
                                        if (eventData.NodeType == XmlNodeType.Element && eventData.Name == "event") 
                                        {
                                            if (eventData.HasAttributes) 
                                            {
                                                int nodeEventID = int.Parse(eventData.GetAttribute("id"));
                                                if (nodeEventID == eventID)
                                                {
                                                    hitEvent = true;
                                                    // Is there a condition
                                                    if (eventData.GetAttribute("condition") != null)
                                                    {
                                                        int nodeConditionID = int.Parse(eventData.GetAttribute("condition"));
                                                        clientResult = client.CheckCondition(nodeConditionID);
                                                    }
                                        
                                                    // Get transition data
                                                    XmlReader transitionData = eventData.ReadSubtree();
                                                    while (transitionData.Read()) 
                                                    {
                                                        if (transitionData.NodeType == XmlNodeType.Element && transitionData.Name == "transition") 
                                                        {
                                                            if (transitionData.HasAttributes) 
                                                            {
                                                                string nodeResult = transitionData.GetAttribute("result");
                                                                if (nodeResult.Equals(clientResult, StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    nextState = int.Parse(transitionData.GetAttribute("next"));
                                                                    if (transitionData.GetAttribute("action") != null)
                                                                    {
                                                                        int action = int.Parse(transitionData.GetAttribute("action"));
                                                                        if (client.RunAction(action))
                                                                            return nextState;
                                                                    }
                                                                    else
                                                                        return nextState;
                                                                }
											                }
										                }
									                }
								                }
							                }
						                }
                                    }
                                }
							}
						}
					}
				}
				
                catch (XmlException e) 
                {
					// Eliminate default trace listener
					Trace.Listeners.RemoveAt(0);
					// Add console trace listener
					TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
	 				Trace.Listeners.Add(myWriter);
					Trace.WriteLine("[XMLStateMachine] Could not load state table definition.");
					Trace.Indent();
					Trace.WriteLine(e.Message);
					Trace.Unindent();
					// 	Clean up object
					if (tableParser != null)
                        tableParser.Close();
				}
			}

			return nextState;
        }

        #region PrivateVariables

        //private XmlTextReader tableParser;

        #endregion
    }
}