using System;
using System.Collections.Generic;

namespace com.protapi
{
	public class ResponseStatus<T>
	{
		int m_StatusCode = 0;
		string m_ErrorMessage = null;
		object m_ReturnedData = null;
		
		public ResponseStatus(){
			
		}
		
		
		public ResponseStatus(int code, object data){
			m_StatusCode = code;	
			m_ReturnedData = data;
		}
		
		public ResponseStatus(int code, string msg){
			m_StatusCode = code;
			m_ErrorMessage = msg;
		}
		
		
		public string errorMessage{
			get{
				return m_ErrorMessage;
			}
			set {
				m_ErrorMessage = value;	
			}
		}
		
		public bool hasFailed{
			get{
				if(m_StatusCode >= 200 && m_StatusCode < 300)
					return false;
				return true;
			}
		}
		
		public int statusCode{
			get{
				return m_StatusCode;
			} 
			set {
				m_StatusCode = value;	
			}
		}
		
		public object responseData{
			get {
				return m_ReturnedData;	
			}
			set {
				m_ReturnedData = value;	
			}
		}
		
		public IList<T> GetResponseList(){
			return m_ReturnedData as IList<T>;
		}
		
		public string GetResponseId(){
			return m_ReturnedData as string;	
		}
	}
}

