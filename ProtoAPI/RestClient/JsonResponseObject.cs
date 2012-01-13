using System;

namespace com.protapi.RestClient
{
	class JsonResponseObject<T>
	{
		T m_Data;
		
		public JsonResponseObject ()
		{
			m_Data = default(T);
		}
		
		public T data
		{
			get{
				return m_Data;
			}
			set{
				m_Data = value;
			}
		}
	}
}

