using System;
namespace com.protapi.Exceptions
{
	public class HTTPException : Exception
	{
		string m_Message = "";
		public HTTPException (string message)
		{
			m_Message = message;
			
		}
		
		public override string Message{
			get {
				return m_Message;	
			}
		}
	}
	
	public class ProtoAPIException : Exception
	{
		string m_Message = "";
		public ProtoAPIException (string message)
		{
			m_Message = message;
			
		}
		
		public override string Message{
			get {
				return m_Message;	
			}
		}
	}
}

