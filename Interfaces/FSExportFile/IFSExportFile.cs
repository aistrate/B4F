using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.FSInterface
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Communicator.FSInterface.FSExportFile">FSExportFile</see> class
    /// </summary>
    public interface IFSExportFile
	{
		int Key { get; set; }
		string FilePath { get; }
		string FileName { get; }
		string FileExt { get; }
		string FSNumber { get; }
		DateTime CreationDate { get; }
		DateTime SentDate { get; }
		char Seperator { get; }
        IList Orders { get; }
	}
}
